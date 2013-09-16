namespace LivestreamBuddy
{
    partial class frmGiveaway
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGiveaway));
            this.lblExclude = new System.Windows.Forms.Label();
            this.txtExclude = new System.Windows.Forms.TextBox();
            this.btnPick = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblWinner = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblExclude
            // 
            this.lblExclude.AutoSize = true;
            this.lblExclude.Location = new System.Drawing.Point(16, 13);
            this.lblExclude.Name = "lblExclude";
            this.lblExclude.Size = new System.Drawing.Size(48, 13);
            this.lblExclude.TabIndex = 0;
            this.lblExclude.Text = "Exclude:";
            // 
            // txtExclude
            // 
            this.txtExclude.Location = new System.Drawing.Point(16, 29);
            this.txtExclude.Multiline = true;
            this.txtExclude.Name = "txtExclude";
            this.txtExclude.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtExclude.Size = new System.Drawing.Size(486, 140);
            this.txtExclude.TabIndex = 1;
            this.toolTip.SetToolTip(this.txtExclude, "Want to exclude certain viewers from winning your giveaway?  Place each name on a" +
        " separate line.");
            // 
            // btnPick
            // 
            this.btnPick.Location = new System.Drawing.Point(16, 176);
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(486, 69);
            this.btnPick.TabIndex = 2;
            this.btnPick.Text = "Pick mah WINNER!";
            this.btnPick.UseVisualStyleBackColor = true;
            this.btnPick.Click += new System.EventHandler(this.btnPick_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(16, 268);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 36);
            this.lblStatus.TabIndex = 3;
            // 
            // lblWinner
            // 
            this.lblWinner.AutoSize = true;
            this.lblWinner.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWinner.ForeColor = System.Drawing.Color.Lime;
            this.lblWinner.Location = new System.Drawing.Point(16, 323);
            this.lblWinner.Name = "lblWinner";
            this.lblWinner.Size = new System.Drawing.Size(0, 31);
            this.lblWinner.TabIndex = 4;
            // 
            // frmGiveaway
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 368);
            this.Controls.Add(this.lblWinner);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnPick);
            this.Controls.Add(this.txtExclude);
            this.Controls.Add(this.lblExclude);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmGiveaway";
            this.Padding = new System.Windows.Forms.Padding(13);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Giveaway";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGiveaway_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExclude;
        private System.Windows.Forms.TextBox txtExclude;
        private System.Windows.Forms.Button btnPick;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblWinner;
        private System.Windows.Forms.ToolTip toolTip;
    }
}