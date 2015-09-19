using System;

namespace DragoonSimulator.Formulas
{
    public static class FormulaLibrary
    {
        public static double WeaponSkills(double potency, double weaponDamage, double str, double det, double multiplier)
        {
            return ((potency / 100) * (weaponDamage / 25 + 1) * (str / 9) * (det / 7290 + 1) * multiplier) - 1;
        }

        public static double AutoAttack(double aaDmg, double str, double det, double aaDelay, double multiplier)
        {
            return ((aaDmg / 34 + 1) * (str / 6.8) * (det / 6795 + 1) * multiplier) - 1;
        }

        public static double SkillSpeedMultiplier(double sks)
        {
            return ((sks - 354)/7722 + 1);
        }

        public static double Gcd(double sks)
        {
            return 2.50245 - ((sks - 354) * 0.0003776);
        }

        public static double CritChance(double crt)
        {
            return ((crt - 354) / (858 * 5)) + 0.05;
        }

        public static double CritDmg(double crt)
        {
            return ((crt - 354) / (858 * 5)) + 1.45;
        }

        public static int GetSkillSpeedRank(double sks)
        {
            if (sks < 487)
            {
                return 1;
            }
            if (sks < 489)
            {
                return 2;
            }
            if (sks < 497)
            {
                return 3;
            }
            if (sks < 516)
            {
                return 4;
            }
            if (sks < 542)
            {
                return 5;
            }
            if (sks < 556)
            {
                return 6;
            }
            if (sks < 564)
            {
                return 7;
            }
            if (sks < 577)
            {
                return 8;
            }
            if (sks < 579)
            {
                return 9;
            }
            if (sks < 587)
            {
                return 11; // These two values are inverted due to some magic in how the game works.
            }
            if (sks < 601)
            {
                return 10; // These two values are inverted due to some magic in how the game works.
            }
            if (sks < 632)
            {
                return 12;
            }
            if (sks < 643)
            {
                return 14;
            }
            if (sks < 651)
            {
                return 13;
            }
            if (sks < 659)
            {
                return 15;
            }
            if (sks < 670)
            {
                return 16;
            }

            throw new Exception("Outside of accepted SKS range.");
        }
    }
}
