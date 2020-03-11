using System;
using System.Collections.Generic;
using System.Windows.Forms;
using KEUtils;

namespace Exercise_Analyzer {

    public partial class MultiChoiceCheckDialog : Form {
        public List<Result> Results { get; set; }

        public MultiChoiceCheckDialog(List<string> fileNames) {
            InitializeComponent();

            // Fill in the check boxes
            Results = new List<Result>(fileNames.Count);
            CheckBox checkBox;
            this.flowLayoutPanelChoices.Controls.Clear();
            int i = 0;
            foreach(string fileName in fileNames) { 
                Results.Add(new Result(fileName));
                checkBox = new System.Windows.Forms.CheckBox();
                checkBox.AutoSize = true;
                checkBox.Dock = System.Windows.Forms.DockStyle.Left;
                checkBox.Location = new System.Drawing.Point(3, 3);
                checkBox.Name = "checkBox" + i;
                checkBox.Size = new System.Drawing.Size(293, 36);
                checkBox.TabIndex = 0;
                checkBox.Text = fileName;
                checkBox.UseVisualStyleBackColor = true;
                checkBox.Click += new System.EventHandler(checkBox_clicked);
                this.flowLayoutPanelChoices.Controls.Add(checkBox);
                i++;
            }
        }

        public string Label
        {
            get
            {
                return labelMsg.Text;
            }
            set
            {
                labelMsg.Text = value;
            }
        }

        private void btn_clicked(object sender, EventArgs e) {
            Button btn = (Button)sender;
            if (btn == buttonOK) {
                DialogResult = DialogResult.OK;
            } else if (btn == buttonCancel) {
                DialogResult = DialogResult.Cancel;
            } else if (btn == buttonAbort) {
                DialogResult = DialogResult.Abort;
            } else {
                Utils.errMsg("Unknown button clicked: " + btn.Text);
                return;
            }
            Close();
        }

        private void checkBox_clicked(object sender, EventArgs e) {
            CheckBox cb = (CheckBox)sender;
            // Get the data from the name. (There is probably a better way.)
            string numString = cb.Name.Substring("checkBox".Length);
            int i = Int32.Parse(numString);
            Results[i].Checked = cb.Checked;
        }

    }

    public class Result {
        public bool Checked { get; set; }
        public string FileName { get; set; }

        public Result(string fileName) {
            FileName = fileName;
            Checked = false;
        }
    }
}
