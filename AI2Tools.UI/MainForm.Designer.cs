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
            this.GamePathBrowseDialog = new System.Windows.Forms.OpenFileDialog();
            this.ResourceNameLabel = new System.Windows.Forms.Label();
            this.ResourceNameBox = new System.Windows.Forms.ComboBox();
            this.GamePathLabel = new System.Windows.Forms.Label();
            this.GamePathBox = new System.Windows.Forms.TextBox();
            this.GamePathBrowseButton = new System.Windows.Forms.Button();
            this.RollButton = new System.Windows.Forms.Button();
            this.UnrollButton = new System.Windows.Forms.Button();
            this.CancellationButton = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // GamePathBrowseDialog
            // 
            this.GamePathBrowseDialog.FileName = "AI_TheSomniumFiles2.exe";
            this.GamePathBrowseDialog.Filter = "Game Executable|AI_TheSomniumFiles2.exe|Executable Files|*.exe|All Files|*.*";
            // 
            // ResourceNameLabel
            // 
            this.ResourceNameLabel.AutoSize = true;
            this.ResourceNameLabel.Location = new System.Drawing.Point(12, 9);
            this.ResourceNameLabel.Name = "ResourceNameLabel";
            this.ResourceNameLabel.Size = new System.Drawing.Size(93, 15);
            this.ResourceNameLabel.TabIndex = 0;
            this.ResourceNameLabel.Text = "&Resource Name:";
            // 
            // ResourceNameBox
            // 
            this.ResourceNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResourceNameBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ResourceNameBox.FormattingEnabled = true;
            this.ResourceNameBox.Location = new System.Drawing.Point(12, 27);
            this.ResourceNameBox.Name = "ResourceNameBox";
            this.ResourceNameBox.Size = new System.Drawing.Size(760, 23);
            this.ResourceNameBox.TabIndex = 1;
            this.ResourceNameBox.SelectionChangeCommitted += new System.EventHandler(this.ResourceNameBox_SelectionChangeCommitted);
            // 
            // GamePathLabel
            // 
            this.GamePathLabel.AutoSize = true;
            this.GamePathLabel.Location = new System.Drawing.Point(12, 53);
            this.GamePathLabel.Name = "GamePathLabel";
            this.GamePathLabel.Size = new System.Drawing.Size(68, 15);
            this.GamePathLabel.TabIndex = 2;
            this.GamePathLabel.Text = "&Game Path:";
            // 
            // GamePathBox
            // 
            this.GamePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GamePathBox.Location = new System.Drawing.Point(12, 71);
            this.GamePathBox.Name = "GamePathBox";
            this.GamePathBox.ReadOnly = true;
            this.GamePathBox.Size = new System.Drawing.Size(679, 23);
            this.GamePathBox.TabIndex = 3;
            // 
            // GamePathBrowseButton
            // 
            this.GamePathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GamePathBrowseButton.Location = new System.Drawing.Point(697, 71);
            this.GamePathBrowseButton.Name = "GamePathBrowseButton";
            this.GamePathBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.GamePathBrowseButton.TabIndex = 4;
            this.GamePathBrowseButton.Text = "&Browse...";
            this.GamePathBrowseButton.UseVisualStyleBackColor = true;
            this.GamePathBrowseButton.Click += new System.EventHandler(this.GamePathBrowseButton_Click);
            // 
            // RollButton
            // 
            this.RollButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RollButton.Location = new System.Drawing.Point(535, 100);
            this.RollButton.Name = "RollButton";
            this.RollButton.Size = new System.Drawing.Size(75, 23);
            this.RollButton.TabIndex = 7;
            this.RollButton.Text = "&Roll";
            this.RollButton.UseVisualStyleBackColor = true;
            this.RollButton.Click += new System.EventHandler(this.RollButton_Click);
            // 
            // UnrollButton
            // 
            this.UnrollButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UnrollButton.Location = new System.Drawing.Point(616, 100);
            this.UnrollButton.Name = "UnrollButton";
            this.UnrollButton.Size = new System.Drawing.Size(75, 23);
            this.UnrollButton.TabIndex = 8;
            this.UnrollButton.Text = "&Unroll";
            this.UnrollButton.UseVisualStyleBackColor = true;
            this.UnrollButton.Click += new System.EventHandler(this.UnrollButton_Click);
            // 
            // CancellationButton
            // 
            this.CancellationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancellationButton.Location = new System.Drawing.Point(697, 100);
            this.CancellationButton.Name = "CancellationButton";
            this.CancellationButton.Size = new System.Drawing.Size(75, 23);
            this.CancellationButton.TabIndex = 9;
            this.CancellationButton.Text = "&Cancel";
            this.CancellationButton.UseVisualStyleBackColor = true;
            // 
            // LogBox
            // 
            this.LogBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogBox.FormattingEnabled = true;
            this.LogBox.IntegralHeight = false;
            this.LogBox.ItemHeight = 15;
            this.LogBox.Location = new System.Drawing.Point(12, 129);
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(760, 380);
            this.LogBox.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 521);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.CancellationButton);
            this.Controls.Add(this.UnrollButton);
            this.Controls.Add(this.RollButton);
            this.Controls.Add(this.GamePathBrowseButton);
            this.Controls.Add(this.GamePathBox);
            this.Controls.Add(this.GamePathLabel);
            this.Controls.Add(this.ResourceNameBox);
            this.Controls.Add(this.ResourceNameLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 380);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AI2Tools";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}