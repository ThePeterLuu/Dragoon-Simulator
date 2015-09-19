using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DragoonSimulator.Configuration;
using DragoonSimulator.Entities;
using DragoonSimulator.Factories;
using DragoonSimulator.Formulas;
using DragoonSimulator.Parser;
using DragoonSimulator.Skills;

namespace DragoonSimulator
{
    public class Game
    {
        private static long _currentGameTime;
        private static readonly int EncounterLengthMs = (int)TimeSpan.FromSeconds(191).TotalMilliseconds;
        
        public static void Main()
        {
            while (true)
            {
                Console.WriteLine("Press 1 to run the simulation with configured stats, 2 to run the BIS solver with configured inventory, 9 to exit.");
                try
                {
                    switch (Convert.ToInt32(Console.ReadKey().KeyChar.ToString()))
                    {
                        case 1:
                            RunSimulation(true);
                            break;
                        case 2:
                            RunBestInSlotSolver();
                            break;
                        case 9:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("\nInvalid command. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occured: {ex}.");
                }
            }
        }

        private static void ResetValues()
        {
            _currentGameTime = 0;
            RotationParser.Reset();
        }

        private static void RunBestInSlotSolver()
        {
            var numIterations = 0;
            var accFilter = 0;
            var sksFilter = 0;
            var strictFilter = 0;
            var primaryStatFilter = 0;
            var secondaryStatFilter = 0;

            var recordedRuns = new List<RecordedDPS>();

            // for all combinations
            foreach (InventorySection.MainHandElementCollection.ItemElement weapon in Inventory.Config.MainHand)
            {
                foreach (InventorySection.HeadElementCollection.ItemElement head in Inventory.Config.Head)
                {
                    foreach (InventorySection.BodyElementCollection.ItemElement body in Inventory.Config.Body)
                    {
                        foreach (InventorySection.HandsElementCollection.ItemElement hands in Inventory.Config.Hands)
                        {
                            foreach (InventorySection.WaistElementCollection.ItemElement waist in Inventory.Config.Waist)
                            {
                                foreach (InventorySection.LegsElementCollection.ItemElement legs in Inventory.Config.Legs)
                                {
                                    foreach (InventorySection.FeetElementCollection.ItemElement feet in Inventory.Config.Feet)
                                    {
                                        foreach (InventorySection.NeckElementCollection.ItemElement neck in Inventory.Config.Neck)
                                        {
                                            foreach (InventorySection.EarsElementCollection.ItemElement ears in Inventory.Config.Ears)
                                            {
                                                foreach (InventorySection.WristsElementCollection.ItemElement wrists in Inventory.Config.Wrists)
                                                {
                                                    foreach (InventorySection.LeftRingElementCollection.ItemElement leftRing in Inventory.Config.LeftRing)
                                                    {
                                                        foreach (InventorySection.RightRingElementCollection.ItemElement rightRing in Inventory.Config.RightRing)
                                                        {
                                                            // rings are unique
                                                            if (rightRing.Name.Equals(leftRing.Name,
                                                                StringComparison.OrdinalIgnoreCase))
                                                            {
                                                                continue;
                                                            }
                                                            foreach (InventorySection.FoodElementCollection.ItemElement food in Inventory.Config.Food)
                                                            {
                                                                var player = PlayerFactory.CreatePlayer(true);
                                                                // add weapon
                                                                player.Weapon = new Weapon
                                                                {
                                                                    AutoAttack = weapon.Aa,
                                                                    WeaponDamage = weapon.Wd,
                                                                    Delay = weapon.Delay
                                                                };

                                                                // add stats
                                                                player.Str += weapon.Str + head.Str + body.Str +
                                                                              hands.Str + waist.Str + legs.Str +
                                                                              feet.Str + neck.Str + ears.Str +
                                                                              wrists.Str + leftRing.Str + rightRing.Str;
                                                                player.Acc += weapon.Acc + head.Acc + body.Acc +
                                                                              hands.Acc + waist.Acc + legs.Acc +
                                                                              feet.Acc + neck.Acc + ears.Acc +
                                                                              wrists.Acc + leftRing.Acc + rightRing.Acc;
                                                                player.Crt += weapon.Crt + head.Crt + body.Crt +
                                                                              hands.Crt + waist.Crt + legs.Crt +
                                                                              feet.Crt + neck.Crt + ears.Crt +
                                                                              wrists.Crt + leftRing.Crt + rightRing.Crt;
                                                                player.Det += weapon.Det + head.Det + body.Det +
                                                                              hands.Det + waist.Det + legs.Det +
                                                                              feet.Det + neck.Det + ears.Det +
                                                                              wrists.Det + leftRing.Det + rightRing.Det;
                                                                player.Sks += weapon.Sks + head.Sks + body.Sks +
                                                                              hands.Sks + waist.Sks + legs.Sks +
                                                                              feet.Sks + neck.Sks + ears.Sks +
                                                                              wrists.Sks + leftRing.Sks + rightRing.Sks;

                                                                // add food bonus
                                                                var accBonus = (int) (player.Acc * food.AccModifier) - (int)player.Acc;
                                                                player.Acc += accBonus >= food.MaxAccBonus? food.MaxAccBonus : accBonus;

                                                                var crtBonus = (int)(player.Crt * food.CrtModifier) - (int)player.Crt;
                                                                player.Crt += crtBonus >= food.MaxCrtBonus ? food.MaxCrtBonus : crtBonus;

                                                                var detBonus = (int)(player.Det * food.DetModifier) - (int)player.Det;
                                                                player.Det += detBonus >= food.MaxDetBonus ? food.MaxDetBonus : detBonus;

                                                                var sksBonus = (int)(player.Sks * food.SksModifier) - (int)player.Sks;
                                                                player.Sks += sksBonus >= food.MaxSksBonus ? food.MaxSksBonus : sksBonus;

                                                                // add party bonus
                                                                player.Str = (int) (player.Str * 1.03);

                                                                // run heuristics to pre-filter worse combinations
                                                                if (player.Acc < Convert.ToInt32(ConfigurationManager.AppSettings["MIN_ACC"]))
                                                                {
                                                                    accFilter++;
                                                                    continue;
                                                                }
                                                                if (player.Sks < Convert.ToInt32(ConfigurationManager.AppSettings["MIN_SKS"]) ||
                                                                    player.Sks > Convert.ToInt32(ConfigurationManager.AppSettings["MAX_SKS"]))
                                                                {
                                                                    sksFilter++;
                                                                    continue;
                                                                }

                                                                try
                                                                {
                                                                    if (recordedRuns.Count > 0)
                                                                    {
                                                                        var skip = false;
                                                                        foreach (var recordedRun in recordedRuns)
                                                                        {
                                                                            var recordedPlayer = recordedRun.Player;

                                                                            if (player.Weapon.WeaponDamage <= recordedPlayer.Weapon.WeaponDamage &&
                                                                                player.Str <= recordedPlayer.Str &&
                                                                                player.Crt <= recordedPlayer.Crt &&
                                                                                player.Det <= recordedPlayer.Det &&
                                                                                FormulaLibrary.GetSkillSpeedRank(player.Sks) <=
                                                                                FormulaLibrary.GetSkillSpeedRank(recordedPlayer.Sks))
                                                                            {
                                                                                strictFilter++;
                                                                                skip = true;
                                                                                break;
                                                                            }

                                                                            if (player.Weapon.WeaponDamage <= recordedPlayer.Weapon.WeaponDamage &&
                                                                                player.Str <= recordedPlayer.Str &&
                                                                                player.Crt >= recordedPlayer.Crt &&
                                                                                player.Det >= recordedPlayer.Det &&
                                                                                FormulaLibrary.GetSkillSpeedRank(player.Sks) <=
                                                                                FormulaLibrary.GetSkillSpeedRank(recordedPlayer.Sks))
                                                                            {
                                                                                var crtGain = player.Crt - recordedPlayer.Crt;
                                                                                var detGain = player.Det - recordedPlayer.Det;
                                                                                var secondaryStatGain = crtGain * 1.5 + detGain;

                                                                                var strLoss = recordedPlayer.Str - player.Str;
                                                                                var wdLoss = recordedPlayer.Weapon.WeaponDamage - player.Weapon.WeaponDamage;
                                                                                var primaryStatLoss = strLoss + 5 * wdLoss;

                                                                                if (secondaryStatGain < primaryStatLoss)
                                                                                {
                                                                                    primaryStatFilter++;
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                            }

                                                                            if (player.Weapon.WeaponDamage <= recordedPlayer.Weapon.WeaponDamage &&
                                                                                player.Str <= recordedPlayer.Str &&
                                                                                FormulaLibrary.GetSkillSpeedRank(player.Sks) <=
                                                                                FormulaLibrary.GetSkillSpeedRank(recordedPlayer.Sks))
                                                                            {
                                                                                if (player.Crt > recordedPlayer.Crt && player.Det < recordedPlayer.Det)
                                                                                {
                                                                                    var critGain = player.Crt - recordedPlayer.Crt;
                                                                                    var detLoss = recordedPlayer.Det - player.Det;
                                                                                    if (critGain * 1.5 <= detLoss)
                                                                                    {
                                                                                        secondaryStatFilter++;
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                if (player.Crt < recordedPlayer.Crt && player.Det > recordedPlayer.Det)
                                                                                {
                                                                                    var detGain = player.Det - recordedPlayer.Det;
                                                                                    var critLoss = recordedPlayer.Crt - player.Crt;
                                                                                    if (detGain * 1.25 <= critLoss)
                                                                                    {
                                                                                        secondaryStatFilter++;
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        if (skip)
                                                                        {
                                                                            continue;
                                                                        }
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    continue;
                                                                }
                                                                

                                                                // run the sim
                                                                try
                                                                {
                                                                    numIterations++;
                                                                    var dps = RunSimulation(false, player);
                                                                    recordedRuns.Add(new RecordedDPS
                                                                    {
                                                                        Player = player,
                                                                        DPS = dps,
                                                                        EquipmentNames = new List<string>
                                                                        {
                                                                            weapon.Name, head.Name, body.Name, hands.Name, waist.Name,
                                                                            legs.Name, feet.Name, neck.Name, ears.Name, wrists.Name,
                                                                            leftRing.Name, rightRing.Name, food.Name
                                                                        }
                                                                    });
                                                                    
                                                                    Console.WriteLine($"Iteration {numIterations} - DPS is: {dps}");
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    // suppress errors due to SKS
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var counter = 1;
            foreach (var run in recordedRuns.OrderByDescending(r => r.DPS))
            {
                Console.WriteLine($"--Number {counter++}");
                Console.WriteLine("DPS: " + run.DPS);
                Console.WriteLine("Player Equipment:");
                run.EquipmentNames.ForEach(Console.WriteLine);
                Console.WriteLine("Player stats:");
                Console.WriteLine($"Str: {run.Player.Str}");
                Console.WriteLine($"Acc: {run.Player.Acc}");
                Console.WriteLine($"Crt: {run.Player.Crt}");
                Console.WriteLine($"Det: {run.Player.Det}");
                Console.WriteLine($"Sks: {run.Player.Sks}");
            }

            Console.WriteLine("Diagnostics: ");
            Console.WriteLine($"AccFilter: {accFilter}");
            Console.WriteLine($"SksFilter: {sksFilter}");
            Console.WriteLine($"StrictFilter: {strictFilter}");
            Console.WriteLine($"PrimaryStatFilter: {primaryStatFilter}");
            Console.WriteLine($"SecondaryStatFilter: {secondaryStatFilter}");
        }

        private static double RunSimulation(bool verbose, Player providedPlayer = null)
        {
            ResetValues();
            var actors = new List<Actor>();
            var player = providedPlayer ?? PlayerFactory.CreatePlayer();
            var strikingDummy = StrikingDummyFactory.CreateStrikingDummy();
            actors.Add(player);
            actors.Add(strikingDummy);

            var selectedAbility = RotationParser.SelectFirstAbility();

            while (_currentGameTime < EncounterLengthMs)
            {
                strikingDummy.DamageTaken += player.AutoAttack(strikingDummy);

                if (selectedAbility is WeaponSkills)
                {
                    var skillDamage = player.Attack(strikingDummy, (WeaponSkills)selectedAbility);
                    if (skillDamage > 0)
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(
                                $"T: {FormatTimespan(TimeSpan.FromMilliseconds(GetCurrentGameTime()))} | Used {selectedAbility} for {skillDamage} damage!");
                        }
                        strikingDummy.DamageTaken += skillDamage;
                        selectedAbility = RotationParser.SelectNextAbility();
                    }
                }
                if (selectedAbility is Spells)
                {
                    if (SpellLibrary.IsDamageSpell((Spells)selectedAbility))
                    {
                        var spellDamage = player.UseDamageSpell(strikingDummy, (Spells)selectedAbility);
                        if (spellDamage > 0)
                        {
                            if (verbose)
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"Used {selectedAbility} for {spellDamage} damage!");
                            }
                            
                            strikingDummy.DamageTaken += spellDamage;
                            selectedAbility = RotationParser.SelectNextAbility();
                        }
                    }
                    if (SpellLibrary.IsBuffSpell((Spells)selectedAbility))
                    {
                        var castSuccessfully = player.UseBuffSpell(strikingDummy, (Spells)selectedAbility);
                        if (castSuccessfully)
                        {
                            if (verbose)
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"Used {selectedAbility}!");
                            }
                            
                            selectedAbility = RotationParser.SelectNextAbility();
                        }
                    }
                }

                IncrementTimers(actors, verbose);
            }

            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine($"Average Encounter DPS: {strikingDummy.DamageTaken / TimeSpan.FromMilliseconds(EncounterLengthMs).TotalSeconds}");
            }

            return strikingDummy.DamageTaken / TimeSpan.FromMilliseconds(EncounterLengthMs).TotalSeconds;
        }

        private static void IncrementTimers(IEnumerable<Actor> actors, bool verbose)
        {
            _currentGameTime++;

            foreach (var actor in actors)
            {
                var player = actor as Player;
                if (player != null)
                {
                    if (player.AutoAttackDuration > 0)
                    {
                        player.AutoAttackDuration--;
                    }
                    if (player.GcdDuration > 0)
                    {
                        player.GcdDuration--;
                    }
                }

                actor.DecrementDamageOverTimeDuration(verbose);
                actor.DecrementQueuedEffectDuration(verbose);
                actor.DecrementStatusEffectDuration(verbose);
                actor.DecrementCooldownDuration(verbose);
            }
        }

        private static string FormatTimespan(TimeSpan timespan)
        {
            var minutes = timespan.Minutes.ToString();
            if (timespan.Minutes < 10)
            {
                minutes = $"0{minutes}";
            }

            var seconds = timespan.Seconds.ToString();
            if (timespan.Seconds < 10)
            {
                seconds = $"0{seconds}";
            }

            var milliseconds = timespan.Milliseconds.ToString();
            if (timespan.Milliseconds < 10)
            {
                milliseconds = $"0{milliseconds}";
            }
            if (milliseconds.Length > 2)
            {
                milliseconds = milliseconds.Substring(0, 2);
            }

            return $"{minutes}:{seconds}:{milliseconds}";
        }

        public static long GetCurrentGameTime()
        {
            return _currentGameTime;
        }
    }
}
