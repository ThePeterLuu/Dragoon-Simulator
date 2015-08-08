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
        private const int NumTrials = 100;
        public static Random Rng = new Random();
        
        public static void Main()
        {
            var maxParse = int.MinValue;
            var minParse = int.MaxValue;

            var totalDamageDealt = 0;
            var totalEncounterDuration = 0;
            var totalOpenerDamageDealt = 0;
            var totalOpenerDuration = 0;

            for (var i = 0; i < NumTrials; i++)
            {
                _currentGameTime = 0;
                RotationParser.CompletedOpener = false;
                var savedOpenerDamageDealt = false;

                var actors = new List<Actor>();
                var player = PlayerFactory.CreatePlayer();
                var strikingDummy = StrikingDummyFactory.CreateStrikingDummy();
                actors.Add(player);
                actors.Add(strikingDummy);

                var selectedAbility = RotationParser.SelectFirstAbility();

                while (_currentGameTime < EncounterLengthMs)
                {
                    if (!savedOpenerDamageDealt && RotationParser.CompletedOpener)
                    {
                        totalOpenerDamageDealt += (int)strikingDummy.DamageTaken;
                        totalOpenerDuration += (int)TimeSpan.FromMilliseconds(_currentGameTime).TotalSeconds;
                        savedOpenerDamageDealt = true;
                    }

                    strikingDummy.DamageTaken += player.AutoAttack(strikingDummy);

                    if (selectedAbility is WeaponSkills)
                    {
                        var skillDamage = player.Attack(strikingDummy, (WeaponSkills)selectedAbility);
                        if (skillDamage > 0)
                        {
                            strikingDummy.DamageTaken += skillDamage;
                            selectedAbility = RotationParser.SelectNextAbility();
                        }
                    }
                    if (selectedAbility is Spells)
                    {
                        if (SpellLibrary.IsDamageSpell((Spells) selectedAbility))
                        {
                            var spellDamage = player.UseDamageSpell(strikingDummy, (Spells)selectedAbility);
                            if (spellDamage > 0)
                            {
                                strikingDummy.DamageTaken += spellDamage;
                                selectedAbility = RotationParser.SelectNextAbility();
                            }
                        }
                        if (SpellLibrary.IsBuffSpell((Spells) selectedAbility))
                        {
                            var castSuccessfully = player.UseBuffSpell(strikingDummy, (Spells) selectedAbility);
                            if (castSuccessfully)
                            {
                                selectedAbility = RotationParser.SelectNextAbility();
                            }
                        }
                    }

                    IncrementTimers(actors);
                }

                totalDamageDealt += (int)strikingDummy.DamageTaken;
                totalEncounterDuration += (int)TimeSpan.FromMilliseconds(_currentGameTime).TotalSeconds;

                var encDps = (int) (strikingDummy.DamageTaken / TimeSpan.FromMilliseconds(_currentGameTime).TotalSeconds);
                maxParse = Math.Max(encDps, maxParse);
                minParse = Math.Min(encDps, minParse);
            }

            Console.WriteLine($"Total Trials: { NumTrials }");
            Console.WriteLine($"Average Opener DPS: { totalOpenerDamageDealt / totalOpenerDuration }");
            Console.WriteLine($"Average Encounter DPS: { totalDamageDealt / totalEncounterDuration }");
            Console.WriteLine($"Max Parse: { maxParse }");
            Console.WriteLine($"Min Parse: { minParse }");
            Console.ReadKey();
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

        public static long GetCurrentGameTime()
        {
            return _currentGameTime;
        }
    }
}
