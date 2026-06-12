using System.Drawing;
using System.Windows.Forms;

namespace Dungeon_Siege
{
    partial class VictoryForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label titleLabel;
        private Label descLabel;
        private Panel buttonPanel;
        private Button btnPlayAgain;
        private Button btnMenu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.titleLabel = new Label();
            this.descLabel = new Label();
            this.buttonPanel = new Panel();
            this.btnPlayAgain = new Button();
            this.btnMenu = new Button();

            // ── Form ──────────────────────────────────────────────
            this.Text = "Victory!";
            this.Size = new Size(460, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(18, 35, 18);

            // ── Title label ───────────────────────────────────────
            this.titleLabel.Text = "VICTORY!";
            this.titleLabel.Font = new Font("Arial", 32, FontStyle.Bold);
            this.titleLabel.ForeColor = Color.Gold;
            this.titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.titleLabel.Dock = DockStyle.Top;
            this.titleLabel.Height = 90;

            // ── Description label ─────────────────────────────────
            this.descLabel.Text = "You have cleared the dungeon, the forces of darkness are defeated!";
            this.descLabel.Font = new Font("Arial", 11, FontStyle.Regular);
            this.descLabel.ForeColor = Color.White;
            this.descLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.descLabel.Dock = DockStyle.Fill;

            // ── Button panel ──────────────────────────────────────
            this.buttonPanel.Dock = DockStyle.Bottom;
            this.buttonPanel.Height = 75;
            this.buttonPanel.BackColor = Color.FromArgb(12, 24, 12);

            // ── Play Again button ─────────────────────────────────
            this.btnPlayAgain.Text = "Play Again";
            this.btnPlayAgain.Font = new Font("Arial", 12, FontStyle.Bold);
            this.btnPlayAgain.Size = new Size(150, 42);
            this.btnPlayAgain.Location = new Point(40, 16);
            this.btnPlayAgain.ForeColor = Color.White;
            this.btnPlayAgain.BackColor = Color.DarkGreen;
            this.btnPlayAgain.FlatStyle = FlatStyle.Flat;
            this.btnPlayAgain.FlatAppearance.BorderColor = Color.LimeGreen;
            this.btnPlayAgain.FlatAppearance.BorderSize = 1;
            this.btnPlayAgain.Click += new System.EventHandler(this.BtnPlayAgain_Click);

            // ── Main Menu button ──────────────────────────────────
            this.btnMenu.Text = "Main Menu";
            this.btnMenu.Font = new Font("Arial", 12, FontStyle.Bold);
            this.btnMenu.Size = new Size(150, 42);
            this.btnMenu.Location = new Point(255, 16);
            this.btnMenu.ForeColor = Color.White;
            this.btnMenu.BackColor = Color.DimGray;
            this.btnMenu.FlatStyle = FlatStyle.Flat;
            this.btnMenu.FlatAppearance.BorderColor = Color.Silver;
            this.btnMenu.FlatAppearance.BorderSize = 1;
            this.btnMenu.Click += new System.EventHandler(this.BtnMenu_Click);

            // ── Wire up ───────────────────────────────────────────
            this.buttonPanel.Controls.Add(this.btnPlayAgain);
            this.buttonPanel.Controls.Add(this.btnMenu);

            this.Controls.Add(this.descLabel);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.buttonPanel);
        }
    }
}
