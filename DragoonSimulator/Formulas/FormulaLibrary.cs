using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DragoonSimulator.Formulas
{
    public static class FormulaLibrary
    {
        private const string SkillSpeedRanksFilePath = "SkillSpeedRankings.txt";

        private static List<KeyValuePair<int, double>> _sortedSkillSpeedRanks;

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

        public static string GetSkillSpeedRanksFilePath()
        {
            return SkillSpeedRanksFilePath;
        }

        public static int GetSkillSpeedRank(double sks)
        {
            if (_sortedSkillSpeedRanks == null)
            {
                _sortedSkillSpeedRanks = GetSkillSpeedRanks();
            }

            return _sortedSkillSpeedRanks.FindIndex(kvp => kvp.Key == (int)sks);
        }

        private static List<KeyValuePair<int, double>> GetSkillSpeedRanks()
        {
            using (var file = File.OpenRead(SkillSpeedRanksFilePath))
            using (var reader = new StreamReader(file))
            {
                return JsonConvert.DeserializeObject<Dictionary<int, double>>(reader.ReadToEnd()).OrderBy(kvp => kvp.Value).ToList();
            }
        }
    }
}
