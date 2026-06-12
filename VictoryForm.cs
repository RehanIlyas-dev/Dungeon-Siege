using System;
using System.Windows.Forms;

namespace Dungeon_Siege
{
    public partial class VictoryForm : Form
    {
        public VictoryForm()
        {
            InitializeComponent();
        }

        private void BtnPlayAgain_Click(object sender, EventArgs e)
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
