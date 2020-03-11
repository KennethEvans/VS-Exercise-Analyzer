namespace Exercise_Analyzer {
    partial class MultiChoiceCheckDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiChoiceCheckDialog));
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanelChoices = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxSample = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelMsg = new System.Windows.Forms.Label();
            this.tableLayoutPanelTop.SuspendLayout();
            this.flowLayoutPanelChoices.SuspendLayout();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelTop
            // 
            this.tableLayoutPanelTop.AutoSize = true;
            this.tableLayoutPanelTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelTop.ColumnCount = 1;
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.Controls.Add(this.labelMsg, 0, 0);
            this.tableLayoutPanelTop.Controls.Add(this.flowLayoutPanelChoices, 0, 1);
            this.tableLayoutPanelTop.Controls.Add(this.flowLayoutPanelButtons, 1, 2);
            this.tableLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelTop.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            this.tableLayoutPanelTop.RowCount = 3;
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(1458, 473);
            this.tableLayoutPanelTop.TabIndex = 0;
            // 
            // flowLayoutPanelChoices
            // 
            this.flowLayoutPanelChoices.AutoSize = true;
            this.flowLayoutPanelChoices.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelChoices.Controls.Add(this.checkBoxSample);
            this.flowLayoutPanelChoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelChoices.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelChoices.Location = new System.Drawing.Point(3, 85);
            this.flowLayoutPanelChoices.Name = "flowLayoutPanelChoices";
            this.flowLayoutPanelChoices.Size = new System.Drawing.Size(1452, 42);
            this.flowLayoutPanelChoices.TabIndex = 2;
            // 
            // checkBoxSample
            // 
            this.checkBoxSample.AutoSize = true;
            this.checkBoxSample.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkBoxSample.Location = new System.Drawing.Point(3, 3);
            this.checkBoxSample.Name = "checkBoxSample";
            this.checkBoxSample.Size = new System.Drawing.Size(293, 36);
            this.checkBoxSample.TabIndex = 0;
            this.checkBoxSample.Text = "FileName Example";
            this.checkBoxSample.UseVisualStyleBackColor = true;
            this.checkBoxSample.Click += new System.EventHandler(this.checkBox_clicked);
            // 
            // flowLayoutPanelButtons
            // 
            this.flowLayoutPanelButtons.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.flowLayoutPanelButtons.AutoSize = true;
            this.flowLayoutPanelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelButtons.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanelButtons.Controls.Add(this.buttonCancel);
            this.flowLayoutPanelButtons.Controls.Add(this.buttonAbort);
            this.flowLayoutPanelButtons.Controls.Add(this.buttonOK);
            this.flowLayoutPanelButtons.Location = new System.Drawing.Point(541, 425);
            this.flowLayoutPanelButtons.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            this.flowLayoutPanelButtons.Size = new System.Drawing.Size(375, 46);
            this.flowLayoutPanelButtons.TabIndex = 1;
            this.flowLayoutPanelButtons.WrapContents = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCancel.Location = new System.Drawing.Point(3, 2);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(114, 42);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.btn_clicked);
            // 
            // buttonAbort
            // 
            this.buttonAbort.AutoSize = true;
            this.buttonAbort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonAbort.Location = new System.Drawing.Point(123, 2);
            this.buttonAbort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(177, 42);
            this.buttonAbort.TabIndex = 4;
            this.buttonAbort.Text = "Stop Asking";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.btn_clicked);
            // 
            // buttonOK
            // 
            this.buttonOK.AutoSize = true;
            this.buttonOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonOK.Location = new System.Drawing.Point(306, 2);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(66, 42);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.btn_clicked);
            // 
            // labelMsg
            // 
            this.labelMsg.AutoSize = true;
            this.labelMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMsg.Location = new System.Drawing.Point(3, 0);
            this.labelMsg.Name = "labelMsg";
            this.labelMsg.Padding = new System.Windows.Forms.Padding(25);
            this.labelMsg.Size = new System.Drawing.Size(1452, 82);
            this.labelMsg.TabIndex = 1;
            this.labelMsg.Text = "Select which files to keep:";
            // 
            // MultiChoiceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1458, 473);
            this.Controls.Add(this.tableLayoutPanelTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MultiChoiceDialog";
            this.Text = "Select Files to Keep";
            this.tableLayoutPanelTop.ResumeLayout(false);
            this.tableLayoutPanelTop.PerformLayout();
            this.flowLayoutPanelChoices.ResumeLayout(false);
            this.flowLayoutPanelChoices.PerformLayout();
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.flowLayoutPanelButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelChoices;
        private System.Windows.Forms.CheckBox checkBoxSample;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Label labelMsg;
    }
}