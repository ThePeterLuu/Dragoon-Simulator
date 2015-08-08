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
            return ((aaDmg / 33.04 + 1) * (str / 6.92) * (det / 6715 + 1) * multiplier) - (1.5 * aaDelay);
        }

        public static double SkillSpeedMultiplier(double sks)
        {
            return (1 + (sks - 354) * 0.0000852);
        }

        public static double Gcd(double sks)
        {
            return 2.50256 - (0.01 * (sks - 354) / 26.5);
        }

        public static double CritChance(double crt)
        {
            return ((crt - 354) / (858 * 5)) + 0.05;
        }

        public static double CritDmg(double crt)
        {
            return ((crt - 354) / (858 * 5)) + 1.45;
        }
    }
}
