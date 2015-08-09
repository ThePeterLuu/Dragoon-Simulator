using System;
using System.Collections.Generic;
using System.Linq;
using DragoonSimulator.FFXIVConcepts;
using DragoonSimulator.Formulas;
using DragoonSimulator.Skills;

namespace DragoonSimulator.Entities
{
    public abstract class Actor
    {
        public Dictionary<StatusEffects, EffectSnapshot> DamageOverTimeEffects = new Dictionary<StatusEffects, EffectSnapshot>();
        public Dictionary<StatusEffects, long> StatusEffects = new Dictionary<StatusEffects, long>();
        public Dictionary<StatusEffects, EffectSnapshot> QueuedEffects = new Dictionary<StatusEffects, EffectSnapshot>();
        public Dictionary<Spells, long> Cooldowns = new Dictionary<Spells, long>();
        public double DamageTaken { get; set; }

        public void DecrementCooldownDuration()
        {
            foreach (var cooldown in Cooldowns.ToList())
            {
                Cooldowns[cooldown.Key] = cooldown.Value - 1;
                if (Cooldowns[cooldown.Key] <= 0)
                {
                    Cooldowns.Remove(cooldown.Key);
                }
            }
        }

        public void DecrementStatusEffectDuration()
        {
            if (StatusEffects.ContainsKey(Skills.StatusEffects.BloodOfTheDragon) &&
                StatusEffects[Skills.StatusEffects.BloodOfTheDragon] - 1 <= 0)
            {
                if (Game.CurrentTrial == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{ Skills.StatusEffects.BloodOfTheDragon } has fallen off!");
                }
                
                StatusEffects.Remove(Skills.StatusEffects.BloodOfTheDragon);
                if (StatusEffects.ContainsKey(Skills.StatusEffects.SharperFangAndClaw))
                {
                    if (Game.CurrentTrial == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{Skills.StatusEffects.SharperFangAndClaw} has fallen off!");
                    }

                    StatusEffects.Remove(Skills.StatusEffects.SharperFangAndClaw);
                }
            }

            foreach (var effect in StatusEffects.ToList())
            {
                StatusEffects[effect.Key] = effect.Value - 1;
                if (StatusEffects[effect.Key] <= 0)
                {
                    if (Game.CurrentTrial == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{effect.Key} has fallen off!");
                    }

                    StatusEffects.Remove(effect.Key);
                }
            }
        }

        public void DecrementQueuedEffectDuration()
        {
            foreach (var queuedEffect in QueuedEffects.ToList())
            {
                var effect = QueuedEffects[queuedEffect.Key];
                effect.Duration = effect.Duration - 1;
                QueuedEffects[queuedEffect.Key] = effect;

                if (effect.Duration <= 0)
                {
                    try
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        var effectName = Enum.GetName(typeof(StatusEffects), queuedEffect.Key);

                        WeaponSkills weaponSkill;
                        Spells spell;
                        if (Enum.TryParse(effectName, true, out weaponSkill))
                        {
                            WeaponLibrary.ApplyEffect(effect.Target, weaponSkill, queuedEffect.Value);
                        }

                        if (Enum.TryParse(effectName, true, out spell))
                        {
                            SpellLibrary.ApplyEffect(effect.Target, spell);
                        }
                    }
                    catch
                    {
                        // ignored if it's another non-actionable type such as AnimationLocked
                    }
                    finally
                    {
                        QueuedEffects.Remove(queuedEffect.Key);
                    }
                }
            }
        }

        public void DecrementDamageOverTimeDuration()
        {
            foreach (var dot in DamageOverTimeEffects.ToList())
            {
                var dotEffect = DamageOverTimeEffects[dot.Key];

                if (Game.GetCurrentGameTime() % (long)TimeSpan.FromSeconds(3).TotalMilliseconds == 0)
                {
                    var damage = FormulaLibrary.WeaponSkills(dotEffect.Potency, dotEffect.WeaponDamage, dotEffect.Str, dotEffect.Det, dotEffect.Multiplier);

                    // Crit
                    if (Game.Rng.NextDouble() < dotEffect.CritChance)
                    {
                        damage *= FormulaLibrary.CritDmg(dotEffect.Crt);
                    }

                    if (Game.CurrentTrial == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{dot.Key} ticks for {(long)damage}!");
                    }

                    dotEffect.Target.DamageTaken += (long)damage;
                }

                dotEffect.Duration--;
                if (dotEffect.Duration <= 0)
                {
                    if (Game.CurrentTrial == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{dot.Key} has fallen off!");
                    }
                    DamageOverTimeEffects.Remove(dot.Key);
                }
                else
                {
                    DamageOverTimeEffects[dot.Key] = dotEffect;
                }
            }
        }
    }
}
