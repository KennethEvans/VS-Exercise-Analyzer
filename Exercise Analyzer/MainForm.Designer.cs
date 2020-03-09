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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxInfo = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonProcess1 = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processSingleFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consolidateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoVerboseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formattedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unformattedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
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
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(1674, 884);
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
            this.textBoxInfo.Size = new System.Drawing.Size(1668, 830);
            this.textBoxInfo.TabIndex = 4;
            // 
            // flowLayoutPanelButtons
            // 
            this.flowLayoutPanelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.flowLayoutPanelButtons.AutoSize = true;
            this.flowLayoutPanelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelButtons.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanelButtons.Controls.Add(this.buttonProcess1);
            this.flowLayoutPanelButtons.Controls.Add(this.buttonQuit);
            this.flowLayoutPanelButtons.Location = new System.Drawing.Point(745, 836);
            this.flowLayoutPanelButtons.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            this.flowLayoutPanelButtons.Size = new System.Drawing.Size(183, 46);
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
            this.buttonProcess1.Size = new System.Drawing.Size(93, 42);
            this.buttonProcess1.TabIndex = 0;
            this.buttonProcess1.Text = "Clear";
            this.buttonProcess1.UseVisualStyleBackColor = true;
            this.buttonProcess1.Click += new System.EventHandler(this.button_clear_click);
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonQuit.AutoSize = true;
            this.buttonQuit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonQuit.Location = new System.Drawing.Point(102, 2);
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
            this.dataToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1674, 52);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.processFilesToolStripMenuItem,
            this.processSingleFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveCSVToolStripMenuItem,
            this.toolStripSeparator4,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(75, 48);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // processFilesToolStripMenuItem
            // 
            this.processFilesToolStripMenuItem.Name = "processFilesToolStripMenuItem";
            this.processFilesToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.processFilesToolStripMenuItem.Text = "Process Files...";
            this.processFilesToolStripMenuItem.Click += new System.EventHandler(this.file_ProcessFiles_click);
            // 
            // processSingleFileToolStripMenuItem
            // 
            this.processSingleFileToolStripMenuItem.Name = "processSingleFileToolStripMenuItem";
            this.processSingleFileToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.processSingleFileToolStripMenuItem.Text = "Process Single File...";
            this.processSingleFileToolStripMenuItem.Click += new System.EventHandler(this.file_ProcessSingleFile_click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(393, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.file_Exit_click);
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.consolidateToolStripMenuItem,
            this.sortToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.toolStripSeparator2,
            this.infoToolStripMenuItem,
            this.infoVerboseToolStripMenuItem,
            this.toolStripSeparator3,
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(91, 48);
            this.dataToolStripMenuItem.Text = "Data";
            // 
            // consolidateToolStripMenuItem
            // 
            this.consolidateToolStripMenuItem.Name = "consolidateToolStripMenuItem";
            this.consolidateToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.consolidateToolStripMenuItem.Text = "Consolidate";
            this.consolidateToolStripMenuItem.Click += new System.EventHandler(this.data_Consolidate_click);
            // 
            // sortToolStripMenuItem
            // 
            this.sortToolStripMenuItem.Name = "sortToolStripMenuItem";
            this.sortToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.sortToolStripMenuItem.Text = "Sort";
            this.sortToolStripMenuItem.Click += new System.EventHandler(this.data_Sort_click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.data_Clear_click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(393, 6);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.data_Info_click);
            // 
            // infoVerboseToolStripMenuItem
            // 
            this.infoVerboseToolStripMenuItem.Name = "infoVerboseToolStripMenuItem";
            this.infoVerboseToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.infoVerboseToolStripMenuItem.Text = "Info Verbose";
            this.infoVerboseToolStripMenuItem.Click += new System.EventHandler(this.data_InfoVerbose_click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(393, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.formattedToolStripMenuItem,
            this.unformattedToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.exportToolStripMenuItem.Text = "Export...";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.data_Export_click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.importToolStripMenuItem.Text = "Import...";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.data_Import_click);
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
            // formattedToolStripMenuItem
            // 
            this.formattedToolStripMenuItem.Name = "formattedToolStripMenuItem";
            this.formattedToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.formattedToolStripMenuItem.Text = "Indented";
            this.formattedToolStripMenuItem.Click += new System.EventHandler(this.data_Export_click);
            // 
            // unformattedToolStripMenuItem
            // 
            this.unformattedToolStripMenuItem.Name = "unformattedToolStripMenuItem";
            this.unformattedToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.unformattedToolStripMenuItem.Text = "Not Indented";
            this.unformattedToolStripMenuItem.Click += new System.EventHandler(this.data_Export_click);
            // 
            // saveCSVToolStripMenuItem
            // 
            this.saveCSVToolStripMenuItem.Name = "saveCSVToolStripMenuItem";
            this.saveCSVToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.saveCSVToolStripMenuItem.Text = "Save CSV...";
            this.saveCSVToolStripMenuItem.Click += new System.EventHandler(this.file_SaveCsv_click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(393, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1674, 936);
            this.Controls.Add(this.tableLayoutPanelTop);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem processSingleFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem processFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consolidateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem infoVerboseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formattedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unformattedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}

