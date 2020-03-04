namespace Exercise_Analyzer {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxInfo = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonProcess1 = new System.Windows.Forms.Button();
            this.buttonProcess2 = new System.Windows.Forms.Button();
            this.buttonCompare = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGPXTCXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tableLayoutPanelTop.SuspendLayout();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelTop
            // 
            this.tableLayoutPanelTop.AutoSize = true;
            this.tableLayoutPanelTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelTop.ColumnCount = 1;
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.Controls.Add(this.textBoxInfo, 0, 3);
            this.tableLayoutPanelTop.Controls.Add(this.flowLayoutPanelButtons, 0, 4);
            this.tableLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelTop.Location = new System.Drawing.Point(0, 52);
            this.tableLayoutPanelTop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            this.tableLayoutPanelTop.RowCount = 5;
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(1258, 884);
            this.tableLayoutPanelTop.TabIndex = 1;
            // 
            // textBoxInfo
            // 
            this.textBoxInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxInfo.Location = new System.Drawing.Point(3, 2);
            this.textBoxInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxInfo.Multiline = true;
            this.textBoxInfo.Name = "textBoxInfo";
            this.textBoxInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxInfo.Size = new System.Drawing.Size(1252, 830);
            this.textBoxInfo.TabIndex = 4;
            // 
            // flowLayoutPanelButtons
            // 
            this.flowLayoutPanelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.flowLayoutPanelButtons.AutoSize = true;
            this.flowLayoutPanelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelButtons.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanelButtons.Controls.Add(this.buttonProcess1);
            this.flowLayoutPanelButtons.Controls.Add(this.buttonProcess2);
            this.flowLayoutPanelButtons.Controls.Add(this.buttonCompare);
            this.flowLayoutPanelButtons.Controls.Add(this.buttonQuit);
            this.flowLayoutPanelButtons.Location = new System.Drawing.Point(381, 836);
            this.flowLayoutPanelButtons.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            this.flowLayoutPanelButtons.Size = new System.Drawing.Size(495, 46);
            this.flowLayoutPanelButtons.TabIndex = 0;
            this.flowLayoutPanelButtons.WrapContents = false;
            // 
            // buttonProcess1
            // 
            this.buttonProcess1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonProcess1.AutoSize = true;
            this.buttonProcess1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonProcess1.Location = new System.Drawing.Point(3, 2);
            this.buttonProcess1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonProcess1.Name = "buttonProcess1";
            this.buttonProcess1.Size = new System.Drawing.Size(131, 42);
            this.buttonProcess1.TabIndex = 0;
            this.buttonProcess1.Text = "Button 1";
            this.buttonProcess1.UseVisualStyleBackColor = true;
            // 
            // buttonProcess2
            // 
            this.buttonProcess2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonProcess2.AutoSize = true;
            this.buttonProcess2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonProcess2.Location = new System.Drawing.Point(140, 2);
            this.buttonProcess2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonProcess2.Name = "buttonProcess2";
            this.buttonProcess2.Size = new System.Drawing.Size(131, 42);
            this.buttonProcess2.TabIndex = 1;
            this.buttonProcess2.Text = "Button 2";
            this.buttonProcess2.UseVisualStyleBackColor = true;
            // 
            // buttonCompare
            // 
            this.buttonCompare.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonCompare.AutoSize = true;
            this.buttonCompare.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCompare.Location = new System.Drawing.Point(277, 2);
            this.buttonCompare.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonCompare.Name = "buttonCompare";
            this.buttonCompare.Size = new System.Drawing.Size(131, 42);
            this.buttonCompare.TabIndex = 2;
            this.buttonCompare.Text = "Button 3";
            this.buttonCompare.UseVisualStyleBackColor = true;
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonQuit.AutoSize = true;
            this.buttonQuit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonQuit.Location = new System.Drawing.Point(414, 2);
            this.buttonQuit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(78, 42);
            this.buttonQuit.TabIndex = 3;
            this.buttonQuit.Text = "Quit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.file_Exit_click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1258, 52);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGPXTCXToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(75, 48);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openGPXTCXToolStripMenuItem
            // 
            this.openGPXTCXToolStripMenuItem.Name = "openGPXTCXToolStripMenuItem";
            this.openGPXTCXToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.openGPXTCXToolStripMenuItem.Text = "Open GPX, TCX...";
            this.openGPXTCXToolStripMenuItem.Click += new System.EventHandler(this.file_OpenInput_click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.file_Exit_click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(92, 48);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(214, 46);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.help_About_click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(393, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 936);
            this.Controls.Add(this.tableLayoutPanelTop);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Exercise Analyzer";
            this.tableLayoutPanelTop.ResumeLayout(false);
            this.tableLayoutPanelTop.PerformLayout();
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.flowLayoutPanelButtons.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private System.Windows.Forms.TextBox textBoxInfo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.Button buttonProcess1;
        private System.Windows.Forms.Button buttonProcess2;
        private System.Windows.Forms.Button buttonCompare;
        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGPXTCXToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

