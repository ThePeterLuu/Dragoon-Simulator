using System;
using System.Collections.Generic;
using System.Configuration;
using DragoonSimulator.Entities;
using DragoonSimulator.Factories;
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
            var baseStr = Convert.ToDouble(ConfigurationManager.AppSettings["BASE_STR"]);

            // apply party bonus
            baseStr = (int) (baseStr * 1.03);
            var dps = RunSimulation(false);
            Console.WriteLine($"DPS is: {dps}");
        }

        private static double RunSimulation(bool verbose)
        {
            ResetValues();
            var actors = new List<Actor>();
            var player = PlayerFactory.CreatePlayer();
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
