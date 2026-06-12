using System.Drawing;
using System.Windows.Forms;

namespace Dungeon_Siege
{
    partial class DefeatForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label titleLabel;
        private Label descLabel;
        private Panel buttonPanel;
        private Button btnRetry;
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
            this.btnRetry = new Button();
            this.btnMenu = new Button();

            // ── Form ──────────────────────────────────────────────
            this.Text = "Defeat!";
            this.Size = new Size(460, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(40, 10, 10);

            // ── Title label ───────────────────────────────────────
            this.titleLabel.Text = "DEFEAT!";
            this.titleLabel.Font = new Font("Arial", 32, FontStyle.Bold);
            this.titleLabel.ForeColor = Color.OrangeRed;
            this.titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.titleLabel.Dock = DockStyle.Top;
            this.titleLabel.Height = 90;

            // ── Description label ─────────────────────────────────
            this.descLabel.Text = "You Lose,Try again!";
            this.descLabel.Font = new Font("Arial", 11, FontStyle.Regular);
            this.descLabel.ForeColor = Color.White;
            this.descLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.descLabel.Dock = DockStyle.Fill;

            // ── Button panel ──────────────────────────────────────
            this.buttonPanel.Dock = DockStyle.Bottom;
            this.buttonPanel.Height = 75;
            this.buttonPanel.BackColor = Color.FromArgb(28, 8, 8);

            // ── Retry button ──────────────────────────────────────
            this.btnRetry.Text = "Retry";
            this.btnRetry.Font = new Font("Arial", 12, FontStyle.Bold);
            this.btnRetry.Size = new Size(150, 42);
            this.btnRetry.Location = new Point(40, 16);
            this.btnRetry.ForeColor = Color.White;
            this.btnRetry.BackColor = Color.DarkRed;
            this.btnRetry.FlatStyle = FlatStyle.Flat;
            this.btnRetry.FlatAppearance.BorderColor = Color.OrangeRed;
            this.btnRetry.FlatAppearance.BorderSize = 1;
            this.btnRetry.Click += new System.EventHandler(this.BtnRetry_Click);

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
            this.buttonPanel.Controls.Add(this.btnRetry);
            this.buttonPanel.Controls.Add(this.btnMenu);

            this.Controls.Add(this.descLabel);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.buttonPanel);
        }
    }
}
