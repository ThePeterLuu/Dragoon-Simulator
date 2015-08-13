using System;
using System.Collections.Generic;
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
            try
            {
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
                        var skillDamage = player.Attack(strikingDummy, (WeaponSkills) selectedAbility);
                        if (skillDamage > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(
                                $"T: {FormatTimespan(TimeSpan.FromMilliseconds(GetCurrentGameTime()))} | Used {selectedAbility} for {skillDamage} damage!");

                            strikingDummy.DamageTaken += skillDamage;
                            selectedAbility = RotationParser.SelectNextAbility();
                        }
                    }
                    if (selectedAbility is Spells)
                    {
                        if (SpellLibrary.IsDamageSpell((Spells) selectedAbility))
                        {
                            var spellDamage = player.UseDamageSpell(strikingDummy, (Spells) selectedAbility);
                            if (spellDamage > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"Used {selectedAbility} for {spellDamage} damage!");

                                strikingDummy.DamageTaken += spellDamage;
                                selectedAbility = RotationParser.SelectNextAbility();
                            }
                        }
                        if (SpellLibrary.IsBuffSpell((Spells) selectedAbility))
                        {
                            var castSuccessfully = player.UseBuffSpell(strikingDummy, (Spells) selectedAbility);
                            if (castSuccessfully)
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"Used {selectedAbility}!");

                                selectedAbility = RotationParser.SelectNextAbility();
                            }
                        }
                    }

                    IncrementTimers(actors);
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine($"Average Encounter DPS: {strikingDummy.DamageTaken / TimeSpan.FromMilliseconds(EncounterLengthMs).TotalSeconds}");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.ResetColor();
                Console.WriteLine($"Error: {e.Message}. Program terminated.");
                Console.ReadKey();
            }
        }

        private static void IncrementTimers(IEnumerable<Actor> actors)
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

                actor.DecrementDamageOverTimeDuration();
                actor.DecrementQueuedEffectDuration();
                actor.DecrementStatusEffectDuration();
                actor.DecrementCooldownDuration();
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
