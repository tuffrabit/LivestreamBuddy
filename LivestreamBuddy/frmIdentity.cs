using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LivestreamBuddy
{
    public partial class IdentityForm : Form
    {
        public IdentityForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("You must provide a username.");
            }
            else if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("You must provide a password.");
            }
            else
            {
                Identity newIdentity = new Identity(txtUsername.Text);
                newIdentity.Add(txtPassword.Text);

                this.Close();
            }
        }
    }
}
