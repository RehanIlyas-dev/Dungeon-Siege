using System;
using System.Drawing;

namespace Dungeon_Siege
{
    public static class Utils
    {
        public const int EntitySize = 40;
        public const int DefaultWallThickness = 32;

        public static double DistanceTo(this Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static int GetWallThickness()
        {
            return TextureManager.WallImage?.Width ?? DefaultWallThickness;
        }

        public static Rectangle GetPlayableBounds(int clientWidth, int clientHeight)
        {
            int wall = GetWallThickness();
            return new Rectangle(wall, wall, clientWidth - wall * 2 - EntitySize, clientHeight - wall * 2 - EntitySize);
        }
    }
}
