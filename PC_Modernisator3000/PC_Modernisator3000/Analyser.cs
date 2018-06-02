using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace PC_Modernisator3000
{
    public partial class Analyser : Form
    {
        string pathFile;
        GraphPane piePane;
        GraphPane piePaneProblems;
        public Analyser()
        {
            InitializeComponent();
            if(!loadFile())
            {
                Load += (s, e) => Close();
                return;
            }
            var analysitor = new Analysis(pathFile);
            analysitor.run();
            drawParts(analysitor.problemParts);
            drawProblems(analysitor.indicatedProblems);

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

        private void drawParts(Dictionary<ProblemParts, double> data)
        {
            ChartView.GraphPane.CurveList.Clear();
            ChartView.GraphPane.GraphObjList.Clear();
            piePane = ChartView.GraphPane;
            piePane.Title.Text = "Ratio of dangerous state to normal for parts";
            var labels = new List<string>();
            var values = new List<double>();
            foreach (var key in data.Keys)
            {
                values.Add(data[key]);
                labels.Add(key.ToString());
            }
            piePane.AddPieSlices(values.ToArray(), labels.ToArray());
            ChartView.AxisChange();
            ChartView.Invalidate();
        }

        private void drawProblems(Dictionary<Problems, double> data)
        {
            pieChartProblems.GraphPane.CurveList.Clear();
            pieChartProblems.GraphPane.GraphObjList.Clear();
            piePaneProblems = pieChartProblems.GraphPane;
            piePaneProblems.Title.Text = "Ratio of finded problems";
            var labels = new List<string>();
            var values = new List<double>();
            foreach (var key in data.Keys)
            {
                values.Add(data[key]);
                labels.Add(key.ToString());
            }
            piePaneProblems.AddPieSlices(values.ToArray(), labels.ToArray());
            pieChartProblems.AxisChange();
            pieChartProblems.Invalidate();
        }
    }
}
