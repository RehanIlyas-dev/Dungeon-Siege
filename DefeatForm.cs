using System;
using System.Windows.Forms;

namespace Dungeon_Siege
{
    public partial class DefeatForm : Form
    {
        public DefeatForm()
        {
            InitializeComponent();
        }

        private void BtnRetry_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
