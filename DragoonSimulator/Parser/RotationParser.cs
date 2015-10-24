using System;
using System.Configuration;
using System.IO;
using DragoonSimulator.Skills;

namespace DragoonSimulator.Parser
{
    public class RotationParser
    {
        private static string[] _loadedRotation;
        private static int _currentAbility;
        private static readonly bool LoopRotationInfinitely = Convert.ToBoolean(ConfigurationManager.AppSettings["LoopRotationInfinitely"]);

        public static void LoadRotation(int sks)
        {
            _loadedRotation = File.ReadAllLines(ConfigurationManager.AppSettings["StandardRotation"]);
        }

        public static Enum SelectFirstAbility()
        {
            _currentAbility = 0;
            return SelectNextAbility();
        }

        public static Enum SelectNextAbility()
        {
            var selectedSkill = ParseAbility(_loadedRotation[_currentAbility]);

            _currentAbility++;

            if (_currentAbility == _loadedRotation.Length && LoopRotationInfinitely)
            {
                _currentAbility = 0;
            }

            return selectedSkill;
        }

        private static Enum ParseAbility(string ability)
        {
            switch (ability)
            {
                case "HT":
                    return WeaponSkills.HeavyThrust;
                case "TT":
                    return WeaponSkills.TrueThrust;
                case "VT":
                    return WeaponSkills.VorpalThrust;
                case "FT":
                    return WeaponSkills.FullThrust;
                case "ID":
                    return WeaponSkills.ImpulseDrive;
                case "DIS":
                    return WeaponSkills.Disembowel;
                case "CT":
                    return WeaponSkills.ChaosThrust;
                case "PH":
                    return WeaponSkills.Phlebotomize;
                case "LEG":
                    return Spells.LegSweep;
                case "BL":
                    return Spells.BattleLitany;
                case "IR":
                    return Spells.InternalRelease;
                case "B4B":
                    return Spells.BloodForBlood;
                case "LS":
                    return Spells.LifeSurge;
                case "STRPOT":
                    return Spells.StrengthPotion;
                case "DFD":
                    return Spells.DragonfireDive;
                case "JMP":
                    return Spells.Jump;
                case "SSD":
                    return Spells.SpineshatterDive;
                case "PS":
                    return Spells.PowerSurge;
                case "BOTD":
                    return Spells.BloodOfTheDragon;
                case "4TH":
                    return WeaponSkills.SharperFangAndClaw;
                case "GK":
                    return Spells.Geirskogul;
                case "DELAY":
                    return Spells.Delay;
                default:
                    throw new Exception($"Unrecognized ability: { ability }");
            }
        }
    }
}
