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
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnGiveaway = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.txtStreamTitle = new System.Windows.Forms.TextBox();
            this.txtStreamGame = new System.Windows.Forms.TextBox();
            this.lblViewerCount = new System.Windows.Forms.LinkLabel();
            this.grpChannelInfo = new System.Windows.Forms.GroupBox();
            this.cmbCommercialLength = new System.Windows.Forms.ComboBox();
            this.btnRunCommercial = new System.Windows.Forms.Button();
            this.btnStreamUpdate = new System.Windows.Forms.Button();
            this.lblStreamTitle = new System.Windows.Forms.Label();
            this.lblStreamGame = new System.Windows.Forms.Label();
            this.geckoMainOutput = new Gecko.GeckoWebBrowser();
            this.grpSetup.SuspendLayout();
            this.grpChannelInfo.SuspendLayout();
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
            this.txtChannel.Location = new System.Drawing.Point(82, 69);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(200, 20);
            this.txtChannel.TabIndex = 2;
            this.toolTip.SetToolTip(this.txtChannel, "The twitch.tv channel you wish to monitor.");
            this.txtChannel.TextChanged += new System.EventHandler(this.txtChannel_TextChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(82, 43);
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
            this.txtUsername.Location = new System.Drawing.Point(82, 17);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 20);
            this.txtUsername.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtUsername, "Your twitch.tv username.");
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
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
            this.lstViewers.Location = new System.Drawing.Point(383, 281);
            this.lstViewers.Name = "lstViewers";
            this.lstViewers.Size = new System.Drawing.Size(200, 303);
            this.lstViewers.TabIndex = 6;
            // 
            // grpSetup
            // 
            this.grpSetup.Controls.Add(this.btnHelp);
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
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(370, 95);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(125, 23);
            this.btnHelp.TabIndex = 6;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
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
            // btnGiveaway
            // 
            this.btnGiveaway.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGiveaway.Location = new System.Drawing.Point(370, 75);
            this.btnGiveaway.Name = "btnGiveaway";
            this.btnGiveaway.Size = new System.Drawing.Size(125, 23);
            this.btnGiveaway.TabIndex = 5;
            this.btnGiveaway.Text = "Giveaway";
            this.btnGiveaway.UseVisualStyleBackColor = true;
            this.btnGiveaway.Click += new System.EventHandler(this.btnGiveaway_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessage.Location = new System.Drawing.Point(13, 603);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(570, 20);
            this.txtMessage.TabIndex = 0;
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            // 
            // txtStreamTitle
            // 
            this.txtStreamTitle.Location = new System.Drawing.Point(82, 23);
            this.txtStreamTitle.Name = "txtStreamTitle";
            this.txtStreamTitle.Size = new System.Drawing.Size(200, 20);
            this.txtStreamTitle.TabIndex = 4;
            this.toolTip.SetToolTip(this.txtStreamTitle, "Title of the current stream.  You will be able to edit this if you have authentic" +
        "ated with Twitch.");
            // 
            // txtStreamGame
            // 
            this.txtStreamGame.Location = new System.Drawing.Point(82, 49);
            this.txtStreamGame.Name = "txtStreamGame";
            this.txtStreamGame.Size = new System.Drawing.Size(200, 20);
            this.txtStreamGame.TabIndex = 5;
            this.toolTip.SetToolTip(this.txtStreamGame, "Game of the current stream.  You will be able to edit this if you have authentica" +
        "ted with Twitch.");
            // 
            // lblViewerCount
            // 
            this.lblViewerCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewerCount.AutoSize = true;
            this.lblViewerCount.Location = new System.Drawing.Point(383, 262);
            this.lblViewerCount.Name = "lblViewerCount";
            this.lblViewerCount.Size = new System.Drawing.Size(73, 13);
            this.lblViewerCount.TabIndex = 5;
            this.lblViewerCount.TabStop = true;
            this.lblViewerCount.Text = "Viewer Count:";
            this.toolTip.SetToolTip(this.lblViewerCount, "This count is pulled from Twitch.  It is possible for this to be 0 and still have" +
        " a viewer list.  The viewer list is pulled from IRC not from Twitch.  Click to f" +
        "orce an update (may take a moment).");
            this.lblViewerCount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblViewerCount_LinkClicked);
            // 
            // grpChannelInfo
            // 
            this.grpChannelInfo.Controls.Add(this.cmbCommercialLength);
            this.grpChannelInfo.Controls.Add(this.btnRunCommercial);
            this.grpChannelInfo.Controls.Add(this.btnStreamUpdate);
            this.grpChannelInfo.Controls.Add(this.btnGiveaway);
            this.grpChannelInfo.Controls.Add(this.lblStreamTitle);
            this.grpChannelInfo.Controls.Add(this.lblStreamGame);
            this.grpChannelInfo.Controls.Add(this.txtStreamTitle);
            this.grpChannelInfo.Controls.Add(this.txtStreamGame);
            this.grpChannelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpChannelInfo.Location = new System.Drawing.Point(13, 145);
            this.grpChannelInfo.Name = "grpChannelInfo";
            this.grpChannelInfo.Padding = new System.Windows.Forms.Padding(7);
            this.grpChannelInfo.Size = new System.Drawing.Size(570, 111);
            this.grpChannelInfo.TabIndex = 11;
            this.grpChannelInfo.TabStop = false;
            this.grpChannelInfo.Text = "Channel";
            // 
            // cmbCommercialLength
            // 
            this.cmbCommercialLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCommercialLength.FormattingEnabled = true;
            this.cmbCommercialLength.Items.AddRange(new object[] {
            "30 Seconds",
            "60 Seconds",
            "90 Seconds"});
            this.cmbCommercialLength.Location = new System.Drawing.Point(432, 22);
            this.cmbCommercialLength.Name = "cmbCommercialLength";
            this.cmbCommercialLength.Size = new System.Drawing.Size(121, 21);
            this.cmbCommercialLength.TabIndex = 9;
            // 
            // btnRunCommercial
            // 
            this.btnRunCommercial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunCommercial.Location = new System.Drawing.Point(300, 21);
            this.btnRunCommercial.Name = "btnRunCommercial";
            this.btnRunCommercial.Size = new System.Drawing.Size(125, 23);
            this.btnRunCommercial.TabIndex = 8;
            this.btnRunCommercial.Text = "Run commercial for";
            this.btnRunCommercial.UseVisualStyleBackColor = true;
            this.btnRunCommercial.Click += new System.EventHandler(this.btnRunCommercial_Click);
            // 
            // btnStreamUpdate
            // 
            this.btnStreamUpdate.Location = new System.Drawing.Point(13, 75);
            this.btnStreamUpdate.Name = "btnStreamUpdate";
            this.btnStreamUpdate.Size = new System.Drawing.Size(125, 23);
            this.btnStreamUpdate.TabIndex = 7;
            this.btnStreamUpdate.Text = "Update";
            this.btnStreamUpdate.UseVisualStyleBackColor = true;
            this.btnStreamUpdate.Click += new System.EventHandler(this.btnStreamUpdate_Click);
            // 
            // lblStreamTitle
            // 
            this.lblStreamTitle.AutoSize = true;
            this.lblStreamTitle.Location = new System.Drawing.Point(10, 26);
            this.lblStreamTitle.Name = "lblStreamTitle";
            this.lblStreamTitle.Size = new System.Drawing.Size(66, 13);
            this.lblStreamTitle.TabIndex = 6;
            this.lblStreamTitle.Text = "Stream Title:";
            // 
            // lblStreamGame
            // 
            this.lblStreamGame.AutoSize = true;
            this.lblStreamGame.Location = new System.Drawing.Point(10, 52);
            this.lblStreamGame.Name = "lblStreamGame";
            this.lblStreamGame.Size = new System.Drawing.Size(38, 13);
            this.lblStreamGame.TabIndex = 3;
            this.lblStreamGame.Text = "Game:";
            // 
            // geckoMainOutput
            // 
            this.geckoMainOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.geckoMainOutput.Location = new System.Drawing.Point(16, 262);
            this.geckoMainOutput.Name = "geckoMainOutput";
            this.geckoMainOutput.Size = new System.Drawing.Size(361, 322);
            this.geckoMainOutput.TabIndex = 12;
            this.geckoMainOutput.UseHttpActivityObserver = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 636);
            this.Controls.Add(this.geckoMainOutput);
            this.Controls.Add(this.lblViewerCount);
            this.Controls.Add(this.grpChannelInfo);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.grpSetup);
            this.Controls.Add(this.lstViewers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(13);
            this.Text = "Livestream Buddy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.grpSetup.ResumeLayout(false);
            this.grpSetup.PerformLayout();
            this.grpChannelInfo.ResumeLayout(false);
            this.grpChannelInfo.PerformLayout();
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
        private System.Windows.Forms.Button btnGiveaway;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox grpChannelInfo;
        private System.Windows.Forms.Button btnStreamUpdate;
        private System.Windows.Forms.Label lblStreamTitle;
        private System.Windows.Forms.Label lblStreamGame;
        private System.Windows.Forms.TextBox txtStreamTitle;
        private System.Windows.Forms.TextBox txtStreamGame;
        private System.Windows.Forms.LinkLabel lblViewerCount;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.ComboBox cmbCommercialLength;
        private System.Windows.Forms.Button btnRunCommercial;
        private Gecko.GeckoWebBrowser geckoMainOutput;

    }
}

