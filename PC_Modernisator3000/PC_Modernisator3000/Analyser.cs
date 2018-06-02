using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PC_Modernisator3000
{
    public partial class Analyser : Form
    {
        string pathFile;

        public Analyser()
        {
            InitializeComponent();
            if(!loadFile())
            {
                Load += (s, e) => Close();
                return;
            }
            var analysitor = new Analysis(pathFile);
            var problemDetails = analysitor.run();
        }

        private bool loadFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "json files (*.json)|*.json";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pathFile = dialog.FileName;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
