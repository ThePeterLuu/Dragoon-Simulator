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
            player.Str = 935;
            player.Dex = 219;
            player.Vit = 684;
            player.Int = 97;
            player.Mnd = 144;
            player.Pie = 165;
        }

        private static void AssignOffensiveProperties(Player player)
        {
            player.Acc = 604;
            player.Crt = 594;
            player.Det = 461;
            player.Sks = 578;
        }

        private static void AssignWeapon(Player player)
        {
            player.Weapon = new Weapon
            {
                WeaponDamage = 74,
                AutoAttack = 71.04,
                Delay = 2.88
            };
        }
    }
}
