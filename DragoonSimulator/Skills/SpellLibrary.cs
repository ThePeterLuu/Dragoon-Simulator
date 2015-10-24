using System;
using System.Collections.Generic;
using DragoonSimulator.Entities;
using DragoonSimulator.FFXIVConcepts;
using DragoonSimulator.Formulas;

namespace DragoonSimulator.Skills
{
    public static class SpellLibrary
    {
        private static readonly Dictionary<Spells, double> Potencies = new Dictionary<Spells, double>
        {
            { Spells.LegSweep, 130 },
            { Spells.Jump, 200 },
            { Spells.SpineshatterDive, 170 },
            { Spells.DragonfireDive, 250 },
            { Spells.Geirskogul, 200 }
        };

        private static readonly Dictionary<Spells, long> Cooldowns = new Dictionary<Spells, long>
        {
            { Spells.LegSweep, 20 },
            { Spells.BattleLitany, 180 },
            { Spells.InternalRelease, 60 },
            { Spells.BloodForBlood, 80 },
            { Spells.LifeSurge, 50 },
            { Spells.StrengthPotion, 270 },   
            { Spells.DragonfireDive, 120 },
            { Spells.Jump, 30 },
            { Spells.SpineshatterDive, 60 },
            { Spells.PowerSurge, 60 },
            { Spells.BloodOfTheDragon, 60 },
            { Spells.Geirskogul, 10 },
            { Spells.Delay, 0 }
        };

        private static readonly List<Spells> DamageSpells = new List<Spells>
        {
            Spells.LegSweep,
            Spells.Jump,
            Spells.SpineshatterDive,
            Spells.DragonfireDive,
            Spells.Geirskogul
        };
        private static readonly List<Spells> BuffSpells = new List<Spells>
        {
            Spells.BattleLitany,
            Spells.InternalRelease,
            Spells.BloodForBlood,
            Spells.LifeSurge,
            Spells.StrengthPotion,
            Spells.PowerSurge,
            Spells.BloodOfTheDragon,
            Spells.Delay
        };

        public static bool IsDamageSpell(Spells spell)
        {
            return DamageSpells.Contains(spell);
        }
        public static bool IsBuffSpell(Spells spell)
        {
            return BuffSpells.Contains(spell);
        }

        public static double SpellPotencies(Spells spell)
        {
            return Potencies[spell];
        }

        public static long SpellCooldowns(Spells spell)
        {
            return (long)TimeSpan.FromSeconds(Cooldowns[spell]).TotalMilliseconds;
        }

        public static void QueueEffect(Player player, Spells spell)
        {
            switch (spell)
            {
                case Spells.LegSweep:
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot{ Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                case Spells.BattleLitany:
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    player.QueuedEffects.Add(StatusEffects.BattleLitany, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                case Spells.InternalRelease:
                    player.QueuedEffects.Add(StatusEffects.InternalRelease, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                case Spells.BloodForBlood:
                    player.QueuedEffects.Add(StatusEffects.BloodForBlood, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                case Spells.LifeSurge:
                    player.QueuedEffects.Add(StatusEffects.LifeSurge, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                case Spells.PowerSurge:
                    player.QueuedEffects.Add(StatusEffects.PowerSurge, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                case Spells.StrengthPotion:
                    player.QueuedEffects.Add(StatusEffects.StrengthPotion, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.60).TotalMilliseconds, Target = player });
                    break;
                case Spells.DragonfireDive:
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.60).TotalMilliseconds, Target = player });
                    break;
                case Spells.Jump:
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.60).TotalMilliseconds, Target = player });
                    break;
                case Spells.SpineshatterDive:
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.60).TotalMilliseconds, Target = player });
                    break;
                case Spells.BloodOfTheDragon:
                    player.QueuedEffects.Add(StatusEffects.BloodOfTheDragon, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                case Spells.Geirskogul:
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                case Spells.Delay:
                    player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });
                    break;
                default:
                    throw new Exception($"Unknown spell { spell }!");
            }
        }

        public static void ApplyEffect(Actor target, Spells spell)
        {
            switch (spell)
            {
                case Spells.BattleLitany:
                    ApplyEffect(target, StatusEffects.BattleLitany, 20);
                    break;
                case Spells.InternalRelease:
                    ApplyEffect(target, StatusEffects.InternalRelease, 15);
                    break;
                case Spells.BloodForBlood:
                    ApplyEffect(target, StatusEffects.BloodForBlood, 20);
                    break;
                case Spells.LifeSurge:
                    ApplyEffect(target, StatusEffects.LifeSurge, 10);
                    break;
                case Spells.PowerSurge:
                    ApplyEffect(target, StatusEffects.PowerSurge, 10);
                    break;
                case Spells.StrengthPotion:
                    ApplyEffect(target, StatusEffects.StrengthPotion, 15);
                    break;
                case Spells.BloodOfTheDragon:
                    ApplyEffect(target, StatusEffects.BloodOfTheDragon, 15);
                    break;
            }
        }
        
        private static void ApplyEffect(Actor actor, StatusEffects statusEffect, long durationSeconds)
        {
            if (!(actor.StatusEffects.ContainsKey(statusEffect)))
            {
                actor.StatusEffects.Add(statusEffect, (long)TimeSpan.FromSeconds(durationSeconds).TotalMilliseconds);
            }
            else
            {
                if (statusEffect == StatusEffects.BloodOfTheDragon &&
                    actor.StatusEffects[statusEffect] > (long) TimeSpan.FromSeconds(durationSeconds).TotalMilliseconds)
                {
                    return;
                }
                actor.StatusEffects[statusEffect] = (long)TimeSpan.FromSeconds(durationSeconds).TotalMilliseconds;
            }
        }
    }
}
