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
    public partial class Chart : Form
    {
        List<Monitoring> monitoring_data;
        GraphPane myPane;
        Dictionary<string, Dictionary<string, List<string>>> devices = new Dictionary<string, Dictionary<string, List<string>>>();

        public Chart(List<Monitoring> data)
        {
            InitializeComponent();
            monitoring_data = data;
            ChartLoad();
            cbMonDev.SelectedIndexChanged += FillSensors;
            cbMonSen.SelectedIndexChanged += cbMonDevSelectedIndex;
            cbMonSenType.SelectedIndexChanged += cbMonDevSenSelectedIndex;
        }

        void ChartLoad()
        {
            foreach (var device in monitoring_data)
            {
                if (!devices.ContainsKey(device.GetHardwareName()))
                {
                    Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                    List<string> list = new List<string>();
                    list.Add(device.GetName());
                    dict.Add(device.GetSensorType().ToString(), list);
                    devices.Add(device.GetHardwareName(), dict);
                }
                else if (!devices[device.GetHardwareName()].ContainsKey(device.GetSensorType().ToString()))
                {
                    Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                    List<string> list = new List<string>();
                    list.Add(device.GetName());
                    devices[device.GetHardwareName()].Add(device.GetSensorType().ToString(), list);
                }
                else
                {
                    devices[device.GetHardwareName()][device.GetSensorType().ToString()].Add(device.GetName());
                }
            }
            cbMonDev.Items.Clear();
            foreach (var device in devices.Keys)
                cbMonDev.Items.Add(device);
            cbMonDev.SelectedIndex = 0;
        }

        void FillSensors(object sender, EventArgs e)
        {
            cbMonSenType.Items.Clear();
            foreach (var sensor in devices[cbMonDev.SelectedItem.ToString()].Keys)
                cbMonSenType.Items.Add(sensor.ToString());
            cbMonSenType.SelectedIndex = 0;
        }

        void cbMonDevSenSelectedIndex(object sender, EventArgs e)
        {
            cbMonSen.Items.Clear();
            foreach (var sensor in devices[cbMonDev.SelectedItem.ToString()][cbMonSenType.SelectedItem.ToString()])
                cbMonSen.Items.Add(sensor);
            cbMonSen.SelectedIndex = 0;
        }

        void cbMonDevSelectedIndex(object sender, EventArgs e)
        {
            int index = -1;
            foreach(var data in monitoring_data)
            {
                if ((data.GetHardwareName() == cbMonDev.SelectedItem.ToString()) && (data.GetName() == cbMonSen.SelectedItem.ToString()) && (data.GetSensorType().ToString() == cbMonSenType.SelectedItem.ToString()))
                {
                    index = monitoring_data.IndexOf(data);
                    break;
                }
            }
            if (index != -1)
                drawChart(index);
        }

        void drawChart(int index)
        {
            ChartView.GraphPane.CurveList.Clear();
            ChartView.GraphPane.GraphObjList.Clear();
            myPane = ChartView.GraphPane;
            myPane.Title.Text = monitoring_data[index].GetHardwareName() + " " + monitoring_data[index].GetName() + " " + monitoring_data[index].GetSensorType();
            myPane.XAxis.Title.Text = "time";
            myPane.YAxis.Title.Text = "value";

            var values = monitoring_data[index].getAllValue();

            // Создадим список точек
            PointPairList list = new PointPairList();
            PointPairList listMax = new PointPairList();
            // Заполняем список точек
            foreach (var i in values)
            {
                list.Add(values.IndexOf(i), i.getVal());
                listMax.Add(values.IndexOf(i), monitoring_data[index].GetMaxValue());
            }

            LineItem myCurve = myPane.AddCurve("Value", list, Color.Green, SymbolType.None);
            myPane.AddCurve("Max Value", listMax, Color.Red, SymbolType.None);

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию
            ChartView.AxisChange();

            // Обновляем график
            ChartView.Invalidate();
            //ZoomGrid(1.5f);
        }
    }
}
