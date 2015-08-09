using System;
using System.Collections.Generic;
using System.Diagnostics;
using DragoonSimulator.Entities;
using DragoonSimulator.FFXIVConcepts;
using DragoonSimulator.Formulas;

namespace DragoonSimulator.Skills
{
    public static class WeaponLibrary
    {
        private static readonly Dictionary<WeaponSkills, double> Potencies = new Dictionary<WeaponSkills, double>
        {
            { WeaponSkills.TrueThrust, 150 },
            { WeaponSkills.VorpalThrust, 100 },
            { WeaponSkills.FullThrust, 100 },
            { WeaponSkills.ImpulseDrive, 180 },
            { WeaponSkills.HeavyThrust, 170 },
            { WeaponSkills.Disembowel, 100 },
            { WeaponSkills.ChaosThrust, 150 },
            { WeaponSkills.Phlebotomize, 170 },
            { WeaponSkills.SharperFangAndClaw, 290 }
        };
        
        public static double WeaponPotencies(WeaponSkills weaponSkill, Stack<WeaponSkills> lastSkills)
        {
            if (weaponSkill == WeaponSkills.VorpalThrust)
            {
                if (lastSkills.Peek() == WeaponSkills.TrueThrust)
                {
                    return 200;
                }
            }

            if (weaponSkill == WeaponSkills.FullThrust)
            {
                var lastSkill = lastSkills.Pop();
                if (lastSkill == WeaponSkills.VorpalThrust && lastSkills.Peek() == WeaponSkills.TrueThrust)
                {
                    lastSkills.Push(lastSkill);
                    return 360;
                }
                lastSkills.Push(lastSkill);
            }

            if (weaponSkill == WeaponSkills.Disembowel)
            {
                if (lastSkills.Peek() == WeaponSkills.ImpulseDrive)
                {
                    return 220;
                }
            }

            if (weaponSkill == WeaponSkills.ChaosThrust)
            {
                var lastSkill = lastSkills.Pop();
                if (lastSkill == WeaponSkills.Disembowel && lastSkills.Peek() == WeaponSkills.ImpulseDrive)
                {
                    lastSkills.Push(lastSkill);
                    return 250;
                }
                lastSkills.Push(lastSkill);
            }

            return Potencies[weaponSkill];
        }

        public static void QueueEffect(Player player, StrikingDummy strikingDummy, WeaponSkills weaponSkill)
        {
            player.QueuedEffects.Add(StatusEffects.AnimationLocked, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.33).TotalMilliseconds, Target = player });

            switch (weaponSkill)
            {
                case WeaponSkills.HeavyThrust:
                    player.QueuedEffects.Add(StatusEffects.HeavyThrust, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.5).TotalMilliseconds, Target = player });
                    break;
                case WeaponSkills.Disembowel:
                    if (player.LastSkills.Peek() == WeaponSkills.ImpulseDrive)
                    {
                        strikingDummy.QueuedEffects.Add(StatusEffects.Disembowel, new EffectSnapshot { Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.75).TotalMilliseconds, Target = strikingDummy });
                    }
                    break;
                case WeaponSkills.ChaosThrust:
                    var lastSkill = player.LastSkills.Pop();
                    if (lastSkill == WeaponSkills.Disembowel && player.LastSkills.Peek() == WeaponSkills.ImpulseDrive)
                    {
                        strikingDummy.QueuedEffects.Add(StatusEffects.ChaosThrust, new EffectSnapshot
                        {
                            Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.5).TotalMilliseconds,
                            CritChance = player.CalculateCritChance(),
                            Crt = player.Crt,
                            Det = player.Det,
                            Multiplier = player.CalculateDamageOverTimeMultiplier(),
                            Str = player.GetStrength(),
                            Potency = 35,
                            Target = strikingDummy,
                            WeaponDamage = player.Weapon.WeaponDamage
                        });
                    }
                    player.LastSkills.Push(lastSkill);
                    break;
                case WeaponSkills.Phlebotomize:
                    strikingDummy.QueuedEffects.Add(StatusEffects.Phlebotomize, new EffectSnapshot
                    {
                        Duration = (long)TimeSpan.FromSeconds(FormulaLibrary.Gcd(player.Sks) * 0.5).TotalMilliseconds,
                        CritChance = player.CalculateCritChance(),
                        Crt = player.Crt,
                        Det = player.Det,
                        Multiplier = player.CalculateDamageOverTimeMultiplier(),
                        Str = player.GetStrength(),
                        Potency = 30,
                        Target = strikingDummy,
                        WeaponDamage = player.Weapon.WeaponDamage
                    });
                    break;
            }
        }

        public static void ApplyEffect(Actor target, WeaponSkills weaponSkill, EffectSnapshot effectSnapshot = null)
        {
            switch (weaponSkill)
            {
                case WeaponSkills.HeavyThrust:
                    ApplyEffect(target, StatusEffects.HeavyThrust, 24);
                    break;
                case WeaponSkills.Disembowel:
                    ApplyEffect(target, StatusEffects.Disembowel, 30);
                    break;
                case WeaponSkills.ChaosThrust:
                    Debug.Assert(effectSnapshot != null, "effectSnapshot != null");
                    effectSnapshot.Duration = (long) TimeSpan.FromSeconds(30).TotalMilliseconds;
                    ApplyDamageOverTime(target, StatusEffects.ChaosThrust, effectSnapshot);
                    break;
                case WeaponSkills.Phlebotomize:
                    Debug.Assert(effectSnapshot != null, "effectSnapshot != null");
                    effectSnapshot.Duration = (long)TimeSpan.FromSeconds(24).TotalMilliseconds;
                    ApplyDamageOverTime(target, StatusEffects.Phlebotomize, effectSnapshot);
                    break;
            }
        }

        private static void ApplyEffect(Actor actor, StatusEffects statusEffect, long durationSeconds)
        {
            if (!(actor.StatusEffects.ContainsKey(statusEffect)))
            {
                if (Game.CurrentTrial == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{statusEffect} applied!");
                }
                actor.StatusEffects.Add(statusEffect, (long)TimeSpan.FromSeconds(durationSeconds).TotalMilliseconds);
            }
            else
            {
                if (Game.CurrentTrial == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{statusEffect} refreshed!");
                }
                actor.StatusEffects[statusEffect] = (long)TimeSpan.FromSeconds(durationSeconds).TotalMilliseconds;
            }
        }

        private static void ApplyDamageOverTime(Actor target, StatusEffects statusEffect, EffectSnapshot effectSnapshot)
        {
            if (!(target.DamageOverTimeEffects.ContainsKey(statusEffect)))
            {
                if (Game.CurrentTrial == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{statusEffect} applied!");
                }
                target.DamageOverTimeEffects.Add(statusEffect, effectSnapshot);
            }
            else
            {
                if (Game.CurrentTrial == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{statusEffect} refreshed!");
                }
                target.DamageOverTimeEffects[statusEffect] = effectSnapshot;
            }
        }
    }
}
