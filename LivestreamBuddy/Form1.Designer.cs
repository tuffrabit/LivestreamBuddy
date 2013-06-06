namespace LivestreamBuddy
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblChannel = new System.Windows.Forms.Label();
            this.lstViewers = new System.Windows.Forms.ListBox();
            this.grpSetup = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.btnGiveaway = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.grpSetup.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(10, 46);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 0;
            this.lblPassword.Text = "Password:";
            // 
            // txtChannel
            // 
            this.txtChannel.Location = new System.Drawing.Point(74, 69);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(200, 20);
            this.txtChannel.TabIndex = 2;
            this.toolTip.SetToolTip(this.txtChannel, "The twitch.tv channel you wish to monitor.");
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(74, 43);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 1;
            this.toolTip.SetToolTip(this.txtPassword, "Your twitch.tv password.");
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(10, 20);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(58, 13);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(74, 17);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 20);
            this.txtUsername.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtUsername, "Your twitch.tv username.");
            // 
            // lblChannel
            // 
            this.lblChannel.AutoSize = true;
            this.lblChannel.Location = new System.Drawing.Point(10, 72);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(49, 13);
            this.lblChannel.TabIndex = 4;
            this.lblChannel.Text = "Channel:";
            // 
            // lstViewers
            // 
            this.lstViewers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstViewers.FormattingEnabled = true;
            this.lstViewers.Location = new System.Drawing.Point(383, 151);
            this.lstViewers.Name = "lstViewers";
            this.lstViewers.Size = new System.Drawing.Size(200, 277);
            this.lstViewers.TabIndex = 6;
            // 
            // grpSetup
            // 
            this.grpSetup.Controls.Add(this.btnGiveaway);
            this.grpSetup.Controls.Add(this.btnConnect);
            this.grpSetup.Controls.Add(this.lblUsername);
            this.grpSetup.Controls.Add(this.lblPassword);
            this.grpSetup.Controls.Add(this.txtUsername);
            this.grpSetup.Controls.Add(this.txtChannel);
            this.grpSetup.Controls.Add(this.lblChannel);
            this.grpSetup.Controls.Add(this.txtPassword);
            this.grpSetup.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSetup.Location = new System.Drawing.Point(13, 13);
            this.grpSetup.Name = "grpSetup";
            this.grpSetup.Padding = new System.Windows.Forms.Padding(7);
            this.grpSetup.Size = new System.Drawing.Size(570, 132);
            this.grpSetup.TabIndex = 7;
            this.grpSetup.TabStop = false;
            this.grpSetup.Text = "Setup";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(13, 95);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(125, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessage.Location = new System.Drawing.Point(13, 438);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(570, 20);
            this.txtMessage.TabIndex = 0;
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            // 
            // txtMessages
            // 
            this.txtMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessages.Location = new System.Drawing.Point(13, 151);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ReadOnly = true;
            this.txtMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessages.Size = new System.Drawing.Size(364, 277);
            this.txtMessages.TabIndex = 9;
            // 
            // btnGiveaway
            // 
            this.btnGiveaway.Location = new System.Drawing.Point(370, 95);
            this.btnGiveaway.Name = "btnGiveaway";
            this.btnGiveaway.Size = new System.Drawing.Size(125, 23);
            this.btnGiveaway.TabIndex = 5;
            this.btnGiveaway.Text = "Giveaway";
            this.btnGiveaway.UseVisualStyleBackColor = true;
            this.btnGiveaway.Click += new System.EventHandler(this.btnGiveaway_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 471);
            this.Controls.Add(this.txtMessages);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.grpSetup);
            this.Controls.Add(this.lstViewers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(13);
            this.Text = "Livestream Buddy";
            this.grpSetup.ResumeLayout(false);
            this.grpSetup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.ListBox lstViewers;
        private System.Windows.Forms.GroupBox grpSetup;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TextBox txtMessages;
        private System.Windows.Forms.Button btnGiveaway;
        private System.Windows.Forms.ToolTip toolTip;

    }
}

