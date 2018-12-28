using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day24_1 {
    class Program {
        enum Army {
            IMMUNE_SYSTEM = 1,
            INFECTION = 2,
        }
        class UnitGroup {
            public UnitGroup(Army army, int groupId, Int64 numUnits, Int64 unitHealth, string attackType, Int64 attackPower, Int64 initiative, string weakTo, string immuneTo) {
                Army = army;
                GroupId = groupId;
                NumUnits = numUnits;
                UnitHealth = unitHealth;
                AttackType = attackType;
                AttackPower = attackPower;
                Initiative = initiative;
                string[] delim = new string[1] { ", " };
                _weakToList = weakTo.Split(delim, StringSplitOptions.RemoveEmptyEntries).ToList();
                _immuneToList = immuneTo.Split(delim, StringSplitOptions.RemoveEmptyEntries).ToList();
                TargetGroup = null;
                TargetedByGroup = null;
            }
            public Int64 DamageTo(UnitGroup target) {
                if (target.ImmuneTo(AttackType)) {
                    return 0;
                }
                if (target.WeakTo(AttackType)) {
                    return EffectivePower * 2;
                } else {
                    return EffectivePower;
                }
            }
            public void AttackTarget(bool verbose) {
                if (!IsAlive) {
                    return;
                } else if (TargetGroup == null) {
                    return;
                }
                Debug.Assert(TargetGroup.TargetedByGroup == this);
                Int64 damage = DamageTo(TargetGroup);
                Int64 unitsKilled = Math.Min(damage / TargetGroup.UnitHealth, TargetGroup.NumUnits);
                if (verbose) {
                    Console.WriteLine($"{Army.ToString()} group {GroupId} attacks defending group {TargetGroup.GroupId}, killing {unitsKilled} units");
                }
                TargetGroup.NumUnits -= unitsKilled;
            }
            public bool WeakTo(string attackType) {
                return _weakToList.Contains(attackType);
            }
            public bool ImmuneTo(string attackType) {
                return _immuneToList.Contains(attackType);
            }
            public bool IsAlive { get => NumUnits > 0; }
            public Int64 EffectivePower { get => NumUnits * AttackPower; }
            public Army Army { get; private set; }
            public int GroupId { get; private set; }
            public Int64 NumUnits { get; private set;  }
            public Int64 UnitHealth { get; private set; }
            public string AttackType { get; private set; }
            public Int64 AttackPower { get; private set; }
            public Int64 Initiative { get; private set; }
            private List<string> _weakToList;
            private List<string> _immuneToList;
            public UnitGroup TargetGroup { get; set; }
            public UnitGroup TargetedByGroup { get; set; }
        }
        // Sort by decreasing effective power
        static int CompareGroupsByEffectivePower(UnitGroup lhs, UnitGroup rhs) {
            if (lhs.EffectivePower < rhs.EffectivePower) {
                return +1;
            } else if (lhs.EffectivePower > rhs.EffectivePower) {
                return -1;
            } else {
                // break ties with initiative
                if (lhs.Initiative < rhs.Initiative) {
                    return +1;
                } else if (lhs.Initiative > rhs.Initiative) {
                    return -1;
                }
                return 0;
            }
        }
        // Sort by decreasing initiative
        static int CompareGroupsByInitiative(UnitGroup lhs, UnitGroup rhs) {
            if (lhs.Initiative < rhs.Initiative) {
                return +1;
            } else if (lhs.Initiative > rhs.Initiative) {
                return -1;
            } else {
                return 0;
            }
        }
        static string Solve(string inputFilename, bool verbose) {
            List<UnitGroup> immuneGroups = new List<UnitGroup>();
            List<UnitGroup> infectionGroups = new List<UnitGroup>();
            List<UnitGroup> allGroups = new List<UnitGroup>();
            {
                var rx = new Regex(@"^(?<numUnits>\d+) units each with (?<unitHealth>\d+) hit points (\(([a-z,; ]+)\) )?with an attack that does (?<attackPower>\d+) (?<attackType>\w+) damage at initiative (?<initiative>\d+)", RegexOptions.Compiled);
                var rxWeak = new Regex(@"weak to (?<weakTo>[a-z, ]+);?", RegexOptions.Compiled);
                var rxImmune = new Regex(@"immune to (?<immuneTo>[a-z, ]+);?", RegexOptions.Compiled);
                var inStream = new StreamReader(inputFilename);
                string line;
                // Read immune system units
                line = inStream.ReadLine();
                Debug.Assert(line == "Immune System:");
                while ((line = inStream.ReadLine()) != null) {
                    if (line.Length == 0) {
                        break; // advance
                    }
                    var m = rx.Match(line);
                    Debug.Assert(m.Success);
                    var mw = rxWeak.Match(line);
                    var mi = rxImmune.Match(line);
                    var group = new UnitGroup(
                            Army.IMMUNE_SYSTEM,
                            immuneGroups.Count + 1,
                            Int64.Parse(m.Groups["numUnits"].Value),
                            Int64.Parse(m.Groups["unitHealth"].Value),
                            m.Groups["attackType"].Value,
                            Int64.Parse(m.Groups["attackPower"].Value),
                            Int64.Parse(m.Groups["initiative"].Value),
                            mw.Success ? mw.Groups["weakTo"].Value : "",
                            mi.Success ? mi.Groups["immuneTo"].Value : "");
                    immuneGroups.Add(group);
                    allGroups.Add(group);
                }
                // Read infection units
                line = inStream.ReadLine();
                Debug.Assert(line == "Infection:");
                while ((line = inStream.ReadLine()) != null) {
                    var m = rx.Match(line);
                    Debug.Assert(m.Success);
                    var mw = rxWeak.Match(line);
                    var mi = rxImmune.Match(line);
                    var group = new UnitGroup(
                            Army.INFECTION,
                            infectionGroups.Count + 1,
                            Int64.Parse(m.Groups["numUnits"].Value),
                            Int64.Parse(m.Groups["unitHealth"].Value),
                            m.Groups["attackType"].Value,
                            Int64.Parse(m.Groups["attackPower"].Value),
                            Int64.Parse(m.Groups["initiative"].Value),
                            mw.Success ? mw.Groups["weakTo"].Value : "",
                            mi.Success ? mi.Groups["immuneTo"].Value : "");
                    infectionGroups.Add(group);
                    allGroups.Add(group);
                }
                inStream.Dispose();
            }

            // Simulate combat
            while (immuneGroups.Any(g => g.IsAlive) && infectionGroups.Any(g => g.IsAlive)) {
                if (verbose) {
                    Console.WriteLine("Immune System:");
                    foreach (var g in immuneGroups) {
                        if (g.IsAlive) {
                            Console.WriteLine($"Group {g.GroupId} contains {g.NumUnits} units");
                        }
                    }
                    Console.WriteLine("Infection:");
                    foreach (var g in infectionGroups) {
                        if (g.IsAlive) {
                            Console.WriteLine($"Group {g.GroupId} contains {g.NumUnits} units");
                        }
                    }
                }

                // target selection
                foreach (var g in allGroups) {
                    g.TargetGroup = null;
                    g.TargetedByGroup = null;
                }
                allGroups.Sort(CompareGroupsByEffectivePower);
                foreach (var g in allGroups) {
                    if (!g.IsAlive) {
                        continue;
                    }
                    var potentialTargets = (g.Army == Army.IMMUNE_SYSTEM) ? infectionGroups : immuneGroups;
                    UnitGroup target = null;
                    Int64 maxDamage = 0;
                    foreach (var pt in potentialTargets) {
                        if (!pt.IsAlive) {
                            continue; // Can't target dead groups
                        } else if (pt.TargetedByGroup != null) {
                            continue; // t is already being targeted by a different group
                        }
                        Int64 damage = g.DamageTo(pt);
                        if (damage == 0) {
                            continue; // can't target a group we can't damage.
                        }
                        if (damage > maxDamage) {
                            maxDamage = damage;
                            target = pt;
                        } else if (damage == maxDamage) {
                            if (pt.EffectivePower > target.EffectivePower) {
                                maxDamage = damage;
                                target = pt;
                            } else if (pt.EffectivePower == target.EffectivePower) {
                                if (pt.Initiative > target.Initiative) {
                                    maxDamage = damage;
                                    target = pt;
                                }
                            }
                        }
                        if (verbose) {
                            Console.WriteLine($"{g.Army.ToString()} group {g.GroupId} would deal defending group {pt.GroupId} {damage} damage");
                        }
                    }
                    if (target != null) {
                        Debug.Assert(maxDamage > 0);
                        g.TargetGroup = target;
                        target.TargetedByGroup = g;
                    }
                }

                // attacking
                allGroups.Sort(CompareGroupsByInitiative);
                foreach (var g in allGroups) {
                    g.AttackTarget(verbose);
                }

                if (verbose) {
                    Console.WriteLine("");
                }
            }
            Int64 immuneSystemUnitsLeft = immuneGroups.Sum(g => g.NumUnits);
            Int64 infectionUnitsLeft = infectionGroups.Sum(g => g.NumUnits);
            Debug.Assert(immuneSystemUnitsLeft == 0 || infectionUnitsLeft == 0);
            return Math.Max(immuneSystemUnitsLeft, infectionUnitsLeft).ToString();
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(args[0] + @"\day24-example0.txt", false) == "5216");
            Console.WriteLine(Solve(args[0] + @"\day24-input.txt", false));
        }
    }
}
