using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dungeon_Siege
{
    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver
    }

    public partial class Form1 : Form
    {
        private GameManger gameManager;
        private System.Windows.Forms.Timer gameTimer;
        private GameState currentState = GameState.MainMenu;
        private bool hasActiveGame;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            WindowState = FormWindowState.Maximized;
            TextureManager.LoadAll();

            Rectangle startBounds = Utils.GetPlayableBounds(ClientSize.Width, ClientSize.Height);
            Point startPos = new Point(startBounds.Left + startBounds.Width / 2, startBounds.Top + startBounds.Height / 2);
            gameManager = new GameManger(new Player(100, startPos, 7, TextureManager.PlayerSprite), new List<Enemy>(), 20);

            gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
            gameTimer.Tick += GameTimer_Tick;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            Paint += Form1_Paint;
            MouseDown += Form1_MouseDown;
            gameTimer.Start();
        }

        private Rectangle ScreenBounds
        {
            get
            {
                return new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            }
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            if (currentState == GameState.Playing)
            {
                gameManager.UpdateGame(ScreenBounds);
                if (gameManager.IsGameOver)
                {
                    gameTimer.Stop();
                    if (gameManager.player.Health <= 0)
                    {
                        using (DefeatForm defeatForm = new DefeatForm())
                        {
                            if (defeatForm.ShowDialog(this) == DialogResult.Retry)
                            {
                                StartPlaying();
                            }
                            else
                            {
                                gameManager.player.ResetMovement();
                                currentState = GameState.MainMenu;
                            }
                        }
                    }
                    else
                    {
                        using (VictoryForm victoryForm = new VictoryForm())
                        {
                            if (victoryForm.ShowDialog(this) == DialogResult.Retry)
                            {
                                StartPlaying();
                            }
                            else
                            {
                                gameManager.player.ResetMovement();
                                currentState = GameState.MainMenu;
                            }
                        }
                    }
                    gameTimer.Start();
                }
            }
            Invalidate();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (currentState == GameState.Playing)
                {
                    gameManager.player.ResetMovement();
                    currentState = GameState.MainMenu;
                }
                else if (currentState == GameState.MainMenu && !gameManager.IsGameOver)
                {
                    currentState = GameState.Playing;
                }
                return;
            }

            if (currentState == GameState.GameOver)
            {
                if (e.KeyCode == Keys.R) StartPlaying();
                return;
            }

            if (currentState != GameState.Playing) return;

            Player p = gameManager.player;
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) p.MoveUp = true;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) p.MoveDown = true;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) p.MoveLeft = true;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) p.MoveRight = true;
            if (e.KeyCode == Keys.ShiftKey) p.SpecialAbility();
            if (e.KeyCode == Keys.Space) p.Fire();
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (currentState != GameState.Playing) return;

            Player p = gameManager.player;
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) p.MoveUp = false;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) p.MoveDown = false;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) p.MoveLeft = false;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) p.MoveRight = false;
        }

        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (currentState == GameState.MainMenu)
            {
                HandleMenuClick(e.Location);
                return;
            }

            if (currentState != GameState.Playing) return;

            Enemy? closest = null;
            double minDist = double.MaxValue;
            foreach (Enemy enemy in gameManager.enemies)
            {
                double dist = gameManager.player.Position.DistanceTo(enemy.Position);
                if (dist < minDist) { minDist = dist; closest = enemy; }
            }
            if (closest != null) gameManager.player.Attack(closest);
        }

        private void HandleMenuClick(Point click)
        {
            int menuX = ClientSize.Width / 2 - 100;
            int startY = 150;

            if (new Rectangle(menuX, startY, 200, 50).Contains(click))
            {
                switch (gameManager.CurrentDifficulty)
                {
                    case GameDifficulty.Easy:
                        gameManager.CurrentDifficulty = GameDifficulty.Medium;
                        break;
                    case GameDifficulty.Medium:
                        gameManager.CurrentDifficulty = GameDifficulty.Hard;
                        break;
                    default:
                        gameManager.CurrentDifficulty = GameDifficulty.Easy;
                        break;
                }
                Invalidate();
            }
            else if (new Rectangle(menuX, startY + 70, 200, 50).Contains(click) && hasActiveGame && !gameManager.IsGameOver)
            {
                currentState = GameState.Playing;
            }
            else if (new Rectangle(menuX, startY + 140, 200, 50).Contains(click))
            {
                StartPlaying();
            }
            else if (new Rectangle(menuX, startY + 210, 200, 50).Contains(click))
            {
                Application.Exit();
            }
        }

        private void StartPlaying()
        {
            gameManager.StartGame(ScreenBounds);
            hasActiveGame = true;
            currentState = GameState.Playing;
        }

        private void Form1_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (currentState == GameState.MainMenu)
            {
                DrawBackground(g, usePreview: true);
                g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), 0, 0, ClientSize.Width, ClientSize.Height);

                using Font titleFont = new Font("Arial", 48, FontStyle.Bold);
                SizeF titleSize = g.MeasureString("DUNGEON SIEGE", titleFont);
                g.DrawString("DUNGEON SIEGE", titleFont, Brushes.White, (ClientSize.Width - titleSize.Width) / 2, 60);

                int menuX = ClientSize.Width / 2 - 100;
                int startY = 150;
                DrawButton(g, "Difficulty: " + gameManager.CurrentDifficulty, menuX, startY, true);
                DrawButton(g, "Continue", menuX, startY + 70, hasActiveGame && !gameManager.IsGameOver);
                DrawButton(g, "New Game", menuX, startY + 140, true);
                DrawButton(g, "Exit", menuX, startY + 210, true);
                return;
            }

            DrawBackground(g, usePreview: false);
            gameManager.player.Draw(g);
            foreach (Enemy enemy in gameManager.enemies) enemy.Draw(g);
            DrawWalls(g);
            DrawHud(g);

            if (currentState == GameState.GameOver)
                DrawGameOverOverlay(g);
        }

        private void DrawBackground(Graphics g, bool usePreview)
        {
            Image? bg = usePreview ? TextureManager.PreviewImage : TextureManager.FloorImage;
            if (bg == null && usePreview) bg = TextureManager.FloorImage;

            if (bg != null)
            {
                for (int x = 0; x < ClientSize.Width; x += bg.Width)
                    for (int y = 0; y < ClientSize.Height; y += bg.Height)
                        g.DrawImage(bg, x, y);
            }
            else
            {
                g.Clear(Color.DarkSlateGray);
            }
        }

        private void DrawWalls(Graphics g)
        {
            if (TextureManager.WallImage == null) return;

            Image wall = TextureManager.WallImage;
            for (int x = 0; x < ClientSize.Width; x += wall.Width)
            {
                g.DrawImage(wall, x, 0);
                g.DrawImage(wall, x, ClientSize.Height - wall.Height);
            }
            for (int y = 0; y < ClientSize.Height; y += wall.Height)
            {
                g.DrawImage(wall, 0, y);
                g.DrawImage(wall, ClientSize.Width - wall.Width, y);
            }
        }

        private void DrawHud(Graphics g)
        {
            using Font hudFont = new Font("Arial", 12, FontStyle.Bold);
            Player p = gameManager.player;
            g.DrawString($"Health: {p.Health}", hudFont, Brushes.Green, 20, 20);
            g.DrawString($"Shield: {p.Shields}", hudFont, Brushes.DeepSkyBlue, 150, 20);
            g.DrawString($"Stamina: {p.Stamina}", hudFont, Brushes.Gold, 280, 20);
            g.DrawString($"Kills: {gameManager.KillCount} / {gameManager.KillLimit}", hudFont, Brushes.White, 410, 20);
            g.DrawString($"Difficulty: {gameManager.CurrentDifficulty}", hudFont, Brushes.LightGray, 550, 20);
        }

        private void DrawGameOverOverlay(Graphics g)
        {
            string message = gameManager.player.Health <= 0
                ? "GAME OVER! Press 'R' to Restart"
                : "VICTORY! Press 'R' to Restart";

            using Font statusFont = new Font("Arial", 24, FontStyle.Bold);
            SizeF size = g.MeasureString(message, statusFont);
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)), 0, 0, ClientSize.Width, ClientSize.Height);
            g.DrawString(message, statusFont, Brushes.Red,
                (ClientSize.Width - size.Width) / 2, (ClientSize.Height - size.Height) / 2);
        }

        private void DrawButton(Graphics g, string text, int x, int y, bool enabled)
        {
            Rectangle rect = new Rectangle(x, y, 200, 50);
            g.FillRectangle(enabled ? Brushes.DarkRed : Brushes.Gray, rect);
            g.DrawRectangle(Pens.White, rect);

            using Font font = new Font("Arial", 16, FontStyle.Bold);
            SizeF size = g.MeasureString(text, font);
            g.DrawString(text, font, Brushes.White, x + (200 - size.Width) / 2, y + (50 - size.Height) / 2);
        }
    }
}
