/*Copyright (C) 2013 Robert A. Boucher Jr. (TuFFrabit)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LivestreamBuddy
{
    public partial class frmGiveaway : Form
    {
        public frmGiveaway()
        {
            InitializeComponent();

            this.winner = string.Empty;
        }

        public frmGiveaway(List<string> viewers, ref List<string> excludeList) : this()
        {
            this.Viewers = viewers;
            this.ExcludeList = excludeList;

            foreach (string exclude in this.ExcludeList)
            {
                txtExclude.Text += exclude + Environment.NewLine;
            }
        }

        private string winner;

        public List<string> Viewers { get; set; }

        public List<string> ExcludeList { get; set; }

        private void populateExcludeList()
        {
            ExcludeList.Clear();

            string[] excludeList = txtExclude.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string exclude in excludeList)
            {
                ExcludeList.Add(exclude);
            }
        }

        # region Events

        private void btnPick_Click(object sender, EventArgs e)
        {
            if (Viewers.Count > 0)
            {
                populateExcludeList();

                foreach (string exclude in ExcludeList)
                {
                    Viewers.Remove(exclude);
                }

                Random random = new Random();
                int randomNumber = random.Next(0, Viewers.Count);

                winner = Viewers[randomNumber];

                lblStatus.Text = "AND ";
                Thread.Sleep(750);
                Application.DoEvents();
                lblStatus.Text += "THE ";
                Thread.Sleep(750);
                Application.DoEvents();
                lblStatus.Text += "WINNER ";
                Thread.Sleep(750);
                Application.DoEvents();
                lblStatus.Text += "IS ";
                Thread.Sleep(750);
                Application.DoEvents();
                lblStatus.Text += ".";
                Thread.Sleep(750);
                Application.DoEvents();
                lblStatus.Text += ".";
                Thread.Sleep(750);
                Application.DoEvents();
                lblStatus.Text += ".";
                Thread.Sleep(750);
                Application.DoEvents();
                lblWinner.Text = winner;
            }
        }

        private void frmGiveaway_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrEmpty(winner))
            {
                txtExclude.Text += winner + Environment.NewLine;
            }

            populateExcludeList();
        }

        # endregion
    }
}
