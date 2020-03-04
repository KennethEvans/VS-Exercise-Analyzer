using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using About;
using KEUtils;

namespace Exercise_Analyzer {
    public partial class MainForm : Form {
        public static readonly String NL = Environment.NewLine;
        private string inputFile;

        public MainForm() {
            InitializeComponent();
        }

        /// <summary>
        /// Appends the input plus a NL to the textBoxInfo.
        /// </summary>
        /// <param name="line"></param>
        public void writeInfo(string line) {
            textBoxInfo.AppendText(line + NL);
        }

        private void file_OpenInput_click(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select an Exercise File";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                inputFile = dlg.FileName;
                if (inputFile == null) {
                    Utils.warnMsg("Failed to open Input file");
                    return;
                }
                test(inputFile);
            }
        }

        private void file_Exit_click(object sender, EventArgs e) {
            Close();
        }

        private void help_About_click(object sender, EventArgs e) {
            AboutBox dlg = new AboutBox();
            dlg.ShowDialog();
        }

    }
}