using System;
using System.Windows.Forms;

namespace Exercise_Analyzer {
    public partial class TimeIntervalDialog : Form {
        //private readonly string TIME_FORMAT = "yyyy'-'MM'-'dd HH':'mm':'ss'.'fff'Z'";
        private readonly string TIME_FORMAT = "u";
        public TimeIntervalDialog() {
            InitializeComponent();
        }

        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
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

        public DateTime StartDate
        {
            get
            {
                try {
                    return DateTime.Parse(textBoxStartDate.Text);
                } catch (Exception) {
                    return DateTime.MinValue;
                }
            }
            set
            {
                textBoxStartDate.Text = value.ToString(TIME_FORMAT);
            }
        }

        public DateTime EndDate
        {
            get
            {
                try {
                    return DateTime.Parse(textBoxEndDate.Text);
                } catch (Exception) {
                    return DateTime.MinValue;
                }
            }
            set
            {
                textBoxEndDate.Text = value.ToString(TIME_FORMAT);
            }
        }

        private void btn_clicked(object sender, EventArgs e) {
            Button btn = (Button)sender;
            if (btn == buttonOk) {
                DialogResult = DialogResult.OK;
            } else if (btn == buttonCancel) {
                DialogResult = DialogResult.Cancel;
                return;
            }
            Close();
        }
    }
}
