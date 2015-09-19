using System;
using System.Configuration;
using DragoonSimulator.Entities;
namespace DragoonSimulator.Factories
{
    public class PlayerFactory
    {
        public static Player CreatePlayer(bool baseStatsOnly = false)
        {
            var player = new Player();
            if (baseStatsOnly)
            {
                player.Str = Convert.ToDouble(ConfigurationManager.AppSettings["BASE_STR"]);
                player.Acc = Convert.ToDouble(ConfigurationManager.AppSettings["BASE_ACC"]);
                player.Crt = Convert.ToDouble(ConfigurationManager.AppSettings["BASE_CRT"]);
                player.Det = Convert.ToDouble(ConfigurationManager.AppSettings["BASE_DET"]);
                player.Sks = Convert.ToDouble(ConfigurationManager.AppSettings["BASE_SKS"]);
            }
            else
            {
                player.Str = Convert.ToDouble(ConfigurationManager.AppSettings["STR"]);
                player.Crt = Convert.ToDouble(ConfigurationManager.AppSettings["CRT"]);
                player.Det = Convert.ToDouble(ConfigurationManager.AppSettings["DET"]);
                player.Sks = Convert.ToDouble(ConfigurationManager.AppSettings["SKS"]);
                player.Weapon = new Weapon
                {
                    WeaponDamage = Convert.ToDouble(ConfigurationManager.AppSettings["WD"]),
                    AutoAttack = Convert.ToDouble(ConfigurationManager.AppSettings["AA"]),
                    Delay = Convert.ToDouble(ConfigurationManager.AppSettings["AA_DELAY"])
                };
            }
            
            return player;
        }
    }
}
