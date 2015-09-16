using System;
using System.IO;
using DragoonSimulator.Skills;

namespace DragoonSimulator.Parser
{
    public class RotationParser
    {
        private static readonly string[] LoadedRotation = File.ReadAllLines("Rotation3.txt");
        public static bool CompletedOpener;
        private static int _currentAbility;
        private const string OpenerMarker = "--OPENER";
        private const string RotationMarker = "--ROTATION";

        public static void Reset()
        {
            CompletedOpener = false;
        }

        public static Enum SelectFirstAbility()
        {
            if (!CompletedOpener)
            {
                _currentAbility = Array.IndexOf(LoadedRotation, OpenerMarker) + 1;
            }
            else
            {
                _currentAbility = Array.IndexOf(LoadedRotation, RotationMarker) + 1;
            }
            
            return SelectNextAbility();
        }

        public static Enum SelectNextAbility()
        {
            var selectedSkill = ParseAbility(LoadedRotation[_currentAbility]);

            if (_currentAbility >= Array.IndexOf(LoadedRotation, RotationMarker))
            {
                CompletedOpener = true;
            }

            _currentAbility++;
            if (_currentAbility == Array.IndexOf(LoadedRotation, RotationMarker))
            {
                _currentAbility++;
            }

            if (_currentAbility == LoadedRotation.Length)
            {
                _currentAbility = Array.IndexOf(LoadedRotation, RotationMarker) + 1;
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
                default:
                    throw new Exception($"Unrecognized ability: { ability }");
            }
        }
    }
}
