using System.Collections.Generic;
using System.Drawing;

namespace Dungeon_Siege
{
    public class ProjectileManager
    {
        private readonly List<Projectile> projectiles = new();

        public IReadOnlyList<Projectile> All
        {
            get { return projectiles; }
        }

        public void Add(Projectile projectile)
        {
            projectiles.Add(projectile);
        }

        public void RemoveAt(int index)
        {
            projectiles.RemoveAt(index);
        }

        public void Clear()
        {
            projectiles.Clear();
        }

        public void Update(int screenWidth, int screenHeight)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Move();
                if (projectiles[i].IsOutOfBounds(screenWidth, screenHeight))
                    projectiles.RemoveAt(i);
            }
        }

        public void Draw(Graphics g)
        {
            foreach (Projectile projectile in projectiles)
                projectile.Draw(g);
        }
    }
}
