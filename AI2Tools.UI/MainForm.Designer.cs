namespace AI2Tools
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            GamePathBrowseDialog = new OpenFileDialog();
            ResourceNameLabel = new Label();
            ResourceNameBox = new ComboBox();
            GamePathLabel = new Label();
            GamePathBox = new TextBox();
            GamePathBrowseButton = new Button();
            RollButton = new Button();
            UnrollButton = new Button();
            CancellationButton = new Button();
            LogBox = new ListBox();
            SteamBox = new PictureBox();
            ResourceVersionLabel = new Label();
            ResourceVersionBox = new Label();
            GameVersionLabel = new Label();
            GameVersionBox = new Label();
            ((System.ComponentModel.ISupportInitialize)SteamBox).BeginInit();
            SuspendLayout();
            // 
            // GamePathBrowseDialog
            // 
            GamePathBrowseDialog.FileName = "AI_TheSomniumFiles2.exe";
            GamePathBrowseDialog.Filter = "Game Executable|AI_TheSomniumFiles2.exe|Executable Files|*.exe|All Files|*.*";
            // 
            // ResourceNameLabel
            // 
            ResourceNameLabel.AutoSize = true;
            ResourceNameLabel.Location = new Point(12, 9);
            ResourceNameLabel.Name = "ResourceNameLabel";
            ResourceNameLabel.Size = new Size(93, 15);
            ResourceNameLabel.TabIndex = 0;
            ResourceNameLabel.Text = "&Resource Name:";
            // 
            // ResourceNameBox
            // 
            ResourceNameBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ResourceNameBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ResourceNameBox.FormattingEnabled = true;
            ResourceNameBox.Location = new Point(12, 27);
            ResourceNameBox.Name = "ResourceNameBox";
            ResourceNameBox.Size = new Size(760, 23);
            ResourceNameBox.TabIndex = 1;
            ResourceNameBox.SelectionChangeCommitted += ResourceNameBox_SelectionChangeCommitted;
            // 
            // GamePathLabel
            // 
            GamePathLabel.AutoSize = true;
            GamePathLabel.Location = new Point(12, 53);
            GamePathLabel.Name = "GamePathLabel";
            GamePathLabel.Size = new Size(68, 15);
            GamePathLabel.TabIndex = 2;
            GamePathLabel.Text = "&Game Path:";
            // 
            // GamePathBox
            // 
            GamePathBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GamePathBox.Location = new Point(12, 71);
            GamePathBox.Name = "GamePathBox";
            GamePathBox.ReadOnly = true;
            GamePathBox.Size = new Size(679, 23);
            GamePathBox.TabIndex = 3;
            // 
            // GamePathBrowseButton
            // 
            GamePathBrowseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            GamePathBrowseButton.Location = new Point(697, 71);
            GamePathBrowseButton.Name = "GamePathBrowseButton";
            GamePathBrowseButton.Size = new Size(75, 23);
            GamePathBrowseButton.TabIndex = 4;
            GamePathBrowseButton.Text = "&Browse...";
            GamePathBrowseButton.UseVisualStyleBackColor = true;
            GamePathBrowseButton.Click += GamePathBrowseButton_Click;
            // 
            // RollButton
            // 
            RollButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            RollButton.Location = new Point(535, 109);
            RollButton.Name = "RollButton";
            RollButton.Size = new Size(75, 23);
            RollButton.TabIndex = 7;
            RollButton.Text = "&Roll";
            RollButton.UseVisualStyleBackColor = true;
            RollButton.Click += RollButton_Click;
            // 
            // UnrollButton
            // 
            UnrollButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            UnrollButton.Location = new Point(616, 109);
            UnrollButton.Name = "UnrollButton";
            UnrollButton.Size = new Size(75, 23);
            UnrollButton.TabIndex = 8;
            UnrollButton.Text = "&Unroll";
            UnrollButton.UseVisualStyleBackColor = true;
            UnrollButton.Click += UnrollButton_Click;
            // 
            // CancellationButton
            // 
            CancellationButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CancellationButton.Location = new Point(697, 109);
            CancellationButton.Name = "CancellationButton";
            CancellationButton.Size = new Size(75, 23);
            CancellationButton.TabIndex = 9;
            CancellationButton.Text = "&Cancel";
            CancellationButton.UseVisualStyleBackColor = true;
            // 
            // LogBox
            // 
            LogBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LogBox.FormattingEnabled = true;
            LogBox.IntegralHeight = false;
            LogBox.ItemHeight = 15;
            LogBox.Location = new Point(12, 138);
            LogBox.Name = "LogBox";
            LogBox.Size = new Size(760, 371);
            LogBox.TabIndex = 10;
            // 
            // SteamBox
            // 
            SteamBox.Image = Properties.Resources.Steam;
            SteamBox.Location = new Point(12, 100);
            SteamBox.Name = "SteamBox";
            SteamBox.Size = new Size(32, 32);
            SteamBox.TabIndex = 11;
            SteamBox.TabStop = false;
            // 
            // ResourceVersionLabel
            // 
            ResourceVersionLabel.AutoSize = true;
            ResourceVersionLabel.Location = new Point(50, 100);
            ResourceVersionLabel.Name = "ResourceVersionLabel";
            ResourceVersionLabel.Size = new Size(99, 15);
            ResourceVersionLabel.TabIndex = 12;
            ResourceVersionLabel.Text = "Resource Version:";
            // 
            // ResourceVersionBox
            // 
            ResourceVersionBox.AutoSize = true;
            ResourceVersionBox.Location = new Point(155, 100);
            ResourceVersionBox.Name = "ResourceVersionBox";
            ResourceVersionBox.Size = new Size(73, 15);
            ResourceVersionBox.TabIndex = 13;
            ResourceVersionBox.Text = "<unknown>";
            // 
            // GameVersionLabel
            // 
            GameVersionLabel.AutoSize = true;
            GameVersionLabel.Location = new Point(67, 117);
            GameVersionLabel.Name = "GameVersionLabel";
            GameVersionLabel.Size = new Size(82, 15);
            GameVersionLabel.TabIndex = 14;
            GameVersionLabel.Text = "Game Version:";
            // 
            // GameVersionBox
            // 
            GameVersionBox.AutoSize = true;
            GameVersionBox.Location = new Point(155, 117);
            GameVersionBox.Name = "GameVersionBox";
            GameVersionBox.Size = new Size(73, 15);
            GameVersionBox.TabIndex = 15;
            GameVersionBox.Text = "<unknown>";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 521);
            Controls.Add(GameVersionBox);
            Controls.Add(GameVersionLabel);
            Controls.Add(ResourceVersionBox);
            Controls.Add(ResourceVersionLabel);
            Controls.Add(SteamBox);
            Controls.Add(LogBox);
            Controls.Add(CancellationButton);
            Controls.Add(UnrollButton);
            Controls.Add(RollButton);
            Controls.Add(GamePathBrowseButton);
            Controls.Add(GamePathBox);
            Controls.Add(GamePathLabel);
            Controls.Add(ResourceNameBox);
            Controls.Add(ResourceNameLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(600, 380);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AI2Tools";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)SteamBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog GamePathBrowseDialog;
        private Label ResourceNameLabel;
        private ComboBox ResourceNameBox;
        private Label GamePathLabel;
        private TextBox GamePathBox;
        private Button GamePathBrowseButton;
        private Button RollButton;
        private Button UnrollButton;
        private Button CancellationButton;
        private ListBox LogBox;
        private PictureBox SteamBox;
        private Label ResourceVersionLabel;
        private Label ResourceVersionBox;
        private Label GameVersionLabel;
        private Label GameVersionBox;
    }
}