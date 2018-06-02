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
        Color[] colors = {Color.Blue,
                Color.Green,
                Color.Red,
                Color.Cyan,
                Color.Olive,
                Color.YellowGreen,
                Color.Black,
                Color.BlueViolet,
                Color.Gold,
                Color.Pink,
                Color.Aqua,
                Color.Brown,
                Color.DarkBlue,
                Color.DarkGreen,
                Color.DeepSkyBlue,
                Color.Green,
                Color.Indigo,
                Color.Lime,
                Color.MistyRose,
                Color.Navy};
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
            int i = 0;
            foreach (var key in data.Keys)
            {
                PieItem pieSlice = piePane.AddPieSlice(data[key], colors[i], 0F, key.ToString());
                //pieSlice.LabelType = PieLabelType.None;
                //values.Add(data[key]);
                //labels.Add(key.ToString());
                i++;
            }
            //piePane.AddPieSlices(values.ToArray(), labels.ToArray());
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
