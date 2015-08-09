using System;
using System.Configuration;
using DragoonSimulator.Entities;
namespace DragoonSimulator.Factories
{
    public class PlayerFactory
    {
        public static Player CreatePlayer()
        {
            var player = new Player();
            AssignStats(player);
            AssignOffensiveProperties(player);
            AssignWeapon(player);
            return player;
        }

        private static void AssignStats(Player player) 
        {
            player.Str = Convert.ToDouble(ConfigurationManager.AppSettings["STR"]);
        }

        private static void AssignOffensiveProperties(Player player)
        {
            player.Crt = Convert.ToDouble(ConfigurationManager.AppSettings["CRT"]);
            player.Det = Convert.ToDouble(ConfigurationManager.AppSettings["DET"]);
            player.Sks = Convert.ToDouble(ConfigurationManager.AppSettings["SKS"]);
        }

        private static void AssignWeapon(Player player)
        {
            player.Weapon = new Weapon
            {
                WeaponDamage = Convert.ToDouble(ConfigurationManager.AppSettings["WD"]),
                AutoAttack = Convert.ToDouble(ConfigurationManager.AppSettings["AA"]),
                Delay = Convert.ToDouble(ConfigurationManager.AppSettings["AA_DELAY"])
            };
        }
    }
}
