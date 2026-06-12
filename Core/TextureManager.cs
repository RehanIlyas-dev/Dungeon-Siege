using System.Drawing;
using System.IO;

namespace Dungeon_Siege
{
    public static class TextureManager
    {
        public static Image? PlayerSprite { get; private set; }
        public static Image? FloorImage { get; private set; }
        public static Image? WallImage { get; private set; }
        public static Image? PreviewImage { get; private set; }
        public static Image? ProjectileSprite { get; private set; }
        public static Image? GoblinSprite { get; private set; }
        public static Image? OrcSprite { get; private set; }
        public static Image? DarkMageSprite { get; private set; }

        public static void LoadAll()
        {
            PlayerSprite = Load("Resources/player.png");
            FloorImage = Load("Resources/floor.png");
            WallImage = Load("Resources/wall.png");
            PreviewImage = Load("Resources/preview.png");
            ProjectileSprite = Load("Resources/projectile.png");
            GoblinSprite = Load("Resources/goblin.png");
            OrcSprite = Load("Resources/orc.png");
            DarkMageSprite = Load("Resources/dark_mage.png");
        }

        private static Image? Load(string path)
        {
            if (File.Exists(path))
            {
                return Image.FromFile(path);
            }
            return null;
        }
    }
}
