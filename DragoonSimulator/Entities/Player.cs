using System;
using System.Collections.Generic;
using DragoonSimulator.Formulas;
using DragoonSimulator.Skills;

namespace DragoonSimulator.Entities
{
    public class Player : Actor
    {
        // Constructor
        public Player()
        {
            LastSkills = new Stack<WeaponSkills>();
        }

        // GCD
        public long AutoAttackDuration { get; set; }
        public long GcdDuration { get; set; }

        // Weapon Properties
        public Weapon Weapon { get; set; }

        // Attributes
        public double Str { get; set; }
        public double Dex { get; set; }
        public double Vit { get; set; }
        public double Int { get; set; }
        public double Mnd { get; set; }
        public double Pie { get; set; }

        // Offensive Properties
        public double Acc { get; set; }
        public double Crt { get; set; }
        public double Det { get; set; }

        // Physical Properties
        public double Sks { get; set; }

        // Misc
        public Stack<WeaponSkills> LastSkills { get; set; }

        // -- Methods

        public double AutoAttack(StrikingDummy target)
        {
            if (AutoAttackDuration > 0)
            {
                return 0;
            }

            var damage = FormulaLibrary.AutoAttack(Weapon.AutoAttack, GetStrength(), Det, Weapon.Delay, CalculateMultiplier(target));

            damage = (damage * CalculateCritChance() * FormulaLibrary.CritDmg(Crt)) +
                             (damage * (1 - CalculateCritChance()));

            AutoAttackDuration = (int) TimeSpan.FromSeconds(Weapon.Delay).TotalMilliseconds;
            return damage;
        }

        public double Attack(StrikingDummy target, WeaponSkills weaponSkill)
        {
            var animationLocked = QueuedEffects.ContainsKey(Skills.StatusEffects.AnimationLocked);
            if (GcdDuration > 0)
            {
                return 0;
            }
            if (animationLocked)
            {
                throw new Exception("Warning! GCD is clipped in this rotation! Are you using an off-gcd skill too early?");
            }

            if (weaponSkill == WeaponSkills.SharperFangAndClaw)
            {
                if (!StatusEffects.ContainsKey(Skills.StatusEffects.SharperFangAndClaw))
                {
                    throw new Exception("Error! Cannot use 4th combo without Blood of the Dragon/Enchanced Combo Buff!");
                }
                StatusEffects[Skills.StatusEffects.BloodOfTheDragon] = Math.Min(
                    StatusEffects[Skills.StatusEffects.BloodOfTheDragon] + (long)TimeSpan.FromSeconds(15).TotalMilliseconds,
                    (long)TimeSpan.FromSeconds(30).TotalMilliseconds);
            }
            if (StatusEffects.ContainsKey(Skills.StatusEffects.SharperFangAndClaw))
            {
                StatusEffects.Remove(Skills.StatusEffects.SharperFangAndClaw);
            }

            var potency = WeaponLibrary.WeaponPotencies(weaponSkill, LastSkills);
            var damage = FormulaLibrary.WeaponSkills(potency, Weapon.WeaponDamage, GetStrength(), Det, CalculateMultiplier(target));

            if (StatusEffects.ContainsKey(Skills.StatusEffects.BloodOfTheDragon))
            {
                if (weaponSkill == WeaponSkills.ChaosThrust)
                {
                    var lastSkill = LastSkills.Pop();
                    if (lastSkill == WeaponSkills.Disembowel && LastSkills.Peek() == WeaponSkills.ImpulseDrive)
                    {
                        StatusEffects.Add(Skills.StatusEffects.SharperFangAndClaw, (long)TimeSpan.FromSeconds(10).TotalMilliseconds);
                    }
                    LastSkills.Push(lastSkill);
                }
                if (weaponSkill == WeaponSkills.FullThrust)
                {
                    var lastSkill = LastSkills.Pop();
                    if (lastSkill == WeaponSkills.VorpalThrust && LastSkills.Peek() == WeaponSkills.TrueThrust)
                    {
                        StatusEffects.Add(Skills.StatusEffects.SharperFangAndClaw, (long)TimeSpan.FromSeconds(10).TotalMilliseconds);
                    }
                    LastSkills.Push(lastSkill);
                }
            }

            if (StatusEffects.ContainsKey(Skills.StatusEffects.LifeSurge))
            {
                damage *= FormulaLibrary.CritDmg(Crt);
                StatusEffects.Remove(Skills.StatusEffects.LifeSurge);
            }
            else
            {
                damage = (damage * CalculateCritChance() * FormulaLibrary.CritDmg(Crt)) +
                        (damage * (1 - CalculateCritChance()));
            }

            WeaponLibrary.QueueEffect(this, target, weaponSkill);
            GcdDuration = (int) TimeSpan.FromSeconds(FormulaLibrary.Gcd(Sks)).TotalMilliseconds;
            LastSkills.Push(weaponSkill);

            return damage;
        }

        public double UseDamageSpell(StrikingDummy target, Spells spell)
        {
            if (GcdDuration <= 0)
            {
                var remainingCd = Cooldowns.ContainsKey(spell) ? Cooldowns[spell] : 0;
                throw new Exception($"Warning! Using off-gcd ability { spell } when GCD is available! Remaining cooldown on { spell }: { remainingCd }");
            }

            if (Cooldowns.ContainsKey(spell) || QueuedEffects.ContainsKey(Skills.StatusEffects.AnimationLocked))
            {
                return 0;
            }

            if (spell == Spells.Geirskogul)
            {
                if (!StatusEffects.ContainsKey(Skills.StatusEffects.BloodOfTheDragon))
                {
                    throw new Exception("Cannot use Geirskogul unless under Blood of the Dragon!");
                }

                StatusEffects[Skills.StatusEffects.BloodOfTheDragon] = StatusEffects[Skills.StatusEffects.BloodOfTheDragon] - (long)TimeSpan.FromSeconds(10).TotalMilliseconds;
                if (StatusEffects[Skills.StatusEffects.BloodOfTheDragon] <= 0)
                {
                    StatusEffects.Remove(Skills.StatusEffects.BloodOfTheDragon);
                    if (StatusEffects.ContainsKey(Skills.StatusEffects.SharperFangAndClaw))
                    {
                        StatusEffects.Remove(Skills.StatusEffects.SharperFangAndClaw);
                    }
                }
            }

            var potency = SpellLibrary.SpellPotencies(spell);
            var multiplier = CalculateMultiplier(target);

            if (spell == Spells.Jump || spell == Spells.SpineshatterDive)
            {
                if (StatusEffects.ContainsKey(Skills.StatusEffects.BloodOfTheDragon))
                {
                    multiplier *= 1.30;
                }

                if (StatusEffects.ContainsKey(Skills.StatusEffects.PowerSurge))
                {
                    multiplier *= 1.50;
                    StatusEffects.Remove(Skills.StatusEffects.PowerSurge);
                }
            }

            var damage = FormulaLibrary.WeaponSkills(potency, Weapon.WeaponDamage, GetStrength(), Det, multiplier);

            damage = (damage * CalculateCritChance() * FormulaLibrary.CritDmg(Crt)) +
                    (damage * (1 - CalculateCritChance()));

            SpellLibrary.QueueEffect(this, spell);
            Cooldowns.Add(spell, SpellLibrary.SpellCooldowns(spell));

            return damage;
        }

        public bool UseBuffSpell(StrikingDummy target, Spells spell)
        {
            if (GcdDuration <= 0)
            {
                throw new Exception($"Warning! Using off-gcd ability { spell } when GCD is available! Remaining cooldown on { spell }: { Cooldowns[spell]}");
            }

            if (Cooldowns.ContainsKey(spell) || QueuedEffects.ContainsKey(Skills.StatusEffects.AnimationLocked))
            {
                return false;
            }

            SpellLibrary.QueueEffect(this, spell);
            Cooldowns.Add(spell, SpellLibrary.SpellCooldowns(spell));
            return true;
        }

        private double CalculateMultiplier(Actor target)
        {
            var multiplier = 1.00;

            if (StatusEffects.ContainsKey(Skills.StatusEffects.HeavyThrust))
            {
                multiplier *= 1.15;
            }

            if (StatusEffects.ContainsKey(Skills.StatusEffects.BloodForBlood))
            {
                multiplier *= 1.30;
            }

            if (target.StatusEffects.ContainsKey(Skills.StatusEffects.Disembowel))
            {
                multiplier *= 1.10;
            }

            return multiplier;
        }

        public double CalculateCritChance()
        {
            var critChance = FormulaLibrary.CritChance(Crt);

            if (StatusEffects.ContainsKey(Skills.StatusEffects.BattleLitany))
            {
                critChance += 0.15;
            }

            if (StatusEffects.ContainsKey(Skills.StatusEffects.InternalRelease))
            {
                critChance += 0.10;
            }

            return critChance;
        }

        public double CalculateDamageOverTimeMultiplier()
        {
            var multiplier = FormulaLibrary.SkillSpeedMultiplier(Sks);

            if (StatusEffects.ContainsKey(Skills.StatusEffects.HeavyThrust))
            {
                multiplier *= 1.15;
            }

            if (StatusEffects.ContainsKey(Skills.StatusEffects.BloodForBlood))
            {
                multiplier *= 1.30;
            }

            return multiplier;
        }

        public double GetStrength()
        {
            return StatusEffects.ContainsKey(Skills.StatusEffects.StrengthPotion)
                ? Str + Math.Min(Str * 0.20, 105)
                : Str;
        }
    }
}
