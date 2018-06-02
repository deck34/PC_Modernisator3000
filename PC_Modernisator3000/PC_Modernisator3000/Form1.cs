using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;
using System.Threading;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PC_Modernisator3000
{
    public partial class Modernizator : Form
    {

        Sysinfo sysinfo = new Sysinfo();
        // Sysinfo sysinfoModified = new Sysinfo();
        List<Item> curconf = new List<Item>();
        List<Item> conf = new List<Item>();
        List<Item> templist = new List<Item>();
        List<Monitoring> monitoring_data = new List<Monitoring>();
        string currentSocket = "";
        string currentDDR = "";
        bool MBInstalled = false;
        bool collection_data = false;

        Dictionary<string, string[]> precedents = new Dictionary<string, string[]>();

        //string[] precedents = { "Проблемы с изображением", "Недостаточная производительность ПК", "Компьютер долго запускается" };
        //string[] devicetypes = {"Процессор","Материнская плата", "Видеокарта", "Оперативная память", "Жесткий диск / SSD", "Блок питания", "Оптический привод", "Корпус", "Охлаждение процессора"};

        protected readonly ListSet<ISensor> active = new ListSet<ISensor>();
        public event SensorEventHandler SensorAdded;

        protected virtual void ActivateSensor(ISensor sensor)
        {
            if (active.Add(sensor))
                if (SensorAdded != null)
                    SensorAdded(sensor);
        }

        public Modernizator()
        {
            InitializeComponent();
            //btnExporMonitoring.Visible = false;
            cbPrecedent.SelectedIndexChanged += cbPrecedentTypeSelectedIndexChanged;
            //Parser.generateFile(0, null);
            cbDeviceType.SelectedIndexChanged += cbDeviceTypeSelectedIndexChanged;
            //dgvMonitoring.SelectionChanged += dgvMonitoringSelectionChanged;
            btnAdd.Click += btnAdd_Click;
            firstfunction();
            initPrecedents();
        }

        void firstfunction()
        {
            MessageBox.Show("Перед тем как приступить к работе, вам необходимо выбрать файл с отчетом из программы AIDA64!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (loadAIDAreport() == 1)
            {
                initSysInfo();
                initBefore();
                initAfter();
            }
        }

        void initPrecedents()
        {
            precedents.Add("Everything  (it is possible to change any PC component)", new string[] { "CPU", "Motherboard", "GPU", "RAM", "HDD / SSD", "Power supply", "Recording device" });//, "Корпус", "Охлаждение процессора" });
            precedents.Add("(it is possible to change any PC component)", new string[] { "CPU", "GPU", "RAM", "HDD / SSD" });
            precedents.Add("Long OS boot", new string[] { "HDD / SSD", "RAM" });
            cbPrecedent.Items.AddRange(precedents.Keys.ToArray());
            cbPrecedent.SelectedIndex = 0;
        }

        private void cbPrecedentTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            cbDeviceType.Items.Clear();
            cbDeviceType.Items.AddRange(precedents[cbPrecedent.SelectedItem.ToString()].ToArray());
            cbDeviceType.SelectedIndex = 0;
        }

        //обновить список в cbDevice
        private void cbDeviceTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            cbDevice.Items.Clear();
            templist.Clear();
            Parser t;
            try
            {
                t = new Parser();
            }
            catch (Exception msg)
            {
                MessageBox.Show("Не удалось найти файл парсера. Пожалуйста, нажмите кнопку \"Пропарсить ситилинк\" и дождитесь конца.");
                cbDeviceType.SelectedIndex = -1;
                return;
            }

            switch (cbDeviceType.SelectedItem.ToString())
            {
                case "CPU":
                    List<string> list = new List<string>();
                    List<Item> a = Parser.find(t.cpu.ToList<Item>(), "socket", currentSocket);
                    foreach (CPU i in a)
                    {
                        list.Add(i.toStrArray()[1]);
                    }
                    cbDevice.Items.AddRange(list.ToArray<string>());
                    templist = a;
                    break;
                case "Motherboard":
                    List<string> list0 = new List<string>();
                    var b = t.mb.ToList<Item>();
                    foreach (MB i in b)
                    {
                        list0.Add(i.toStrArray()[1]);
                    }
                    cbDevice.Items.AddRange(list0.ToArray<string>());
                    templist = b;
                    break;
                case "GPU":
                    List<string> list1 = new List<string>();
                    var c = t.gpu.ToList<Item>();
                    foreach (GPU i in c)
                    {
                        list1.Add(i.toStrArray()[1]);
                    }
                    cbDevice.Items.AddRange(list1.ToArray<string>());
                    templist = c;
                    break;
                case "RAM":
                    List<string> list2 = new List<string>();
                    var d = Parser.find(t.ram.ToList<Item>(), "type", currentDDR);
                    foreach (RAM i in d)
                    {
                        list2.Add(i.toStrArray()[1]);
                    }
                    cbDevice.Items.AddRange(list2.ToArray<string>());
                    templist = d;
                    break;
                case "HDD / SSD":
                    List<string> list3 = new List<string>();
                    var j = t.hdd.ToList<Item>();
                    foreach (HDD i in j)
                    {
                        list3.Add(i.toStrArray()[1]);
                    }
                    cbDevice.Items.AddRange(list3.ToArray<string>());
                    templist = j;
                    break;
                case "Power supply":
                    List<string> list4 = new List<string>();
                    var f = t.power.ToList<Item>();
                    foreach (POWER i in f)
                    {
                        list4.Add(i.toStrArray()[1]);
                    }
                    cbDevice.Items.AddRange(list4.ToArray<string>());
                    templist = f;
                    break;
                case "Recording device":
                    List<string> list5 = new List<string>();
                    var g = t.rom.ToList<Item>();
                    foreach (ROM i in g)
                    {
                        list5.Add(i.toStrArray()[1]);
                    }
                    cbDevice.Items.AddRange(list5.ToArray<string>());
                    templist = g;
                    break;
                case "Корпус":
                    break;
                case "Охлаждение процессора":
                    break;

                default:
                    break;
            }
            if (cbDevice.Items.Count > 0)
            {
                cbDevice.Enabled = true;
                cbDevice.SelectedIndex = 0;
            }
            else
            {
                cbDevice.Enabled = false;
            }
        }

        void initAfter()
        {
            MBInstalled = true;
            conf.Clear();
            conf.Add(sysinfo.cpu);
            conf.Add(sysinfo.mb);
            foreach (var i in sysinfo.gpu)
                conf.Add(i);
            foreach (var i in sysinfo.hdd)
                conf.Add(i);
            foreach (var i in sysinfo.ram)
                conf.Add(i);
            foreach (var i in sysinfo.rom)
                conf.Add(i);

            updateMofificationTable();
        }

        void updateMofificationTable()
        {
            dgvAfter.RowCount = 0;
            dgvAfter.ColumnCount = 2;
            dgvAfter.RowCount = conf.Count;
            for (int i = 0; i < conf.Count; i++)
            {
                var temp = conf[i].toStrArray();
                dgvAfter[0, i].Value = temp[0];
                dgvAfter[1, i].Value = temp[1];
            }
            checkConflicts();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            conf.Add(templist[cbDevice.SelectedIndex]);
            if (MBInstalled == true)
            {
                //мб что воткнуть сюда
            }
            if (templist[cbDevice.SelectedIndex].toStrArray()[0] == "Motherboard")
            {
                MBInstalled = true;
                dgvAfter.DefaultCellStyle.BackColor = Color.White;
                currentSocket = templist[cbDevice.SelectedIndex].get("cpuinterface");
                currentDDR = templist[cbDevice.SelectedIndex].get("ramtype");
            }
            updateMofificationTable();
        }

        void checkConflicts()
        {
            int cpu = 0, mb = 0;
            List<string[]> checklist = new List<string[]>();
            int j = 0;
            foreach (var i in conf)
            {
                string[] temp = i.toStrArray();
                checklist.Add(temp);
                if (temp[0] == "CPU")
                {
                    cpu++;
                    if (i.get("socket") != currentSocket && MBInstalled)
                    {
                        //dgvAfter.DefaultCellStyle.BackColor = Color.PaleVioletRed;
                        dgvAfter.Rows[j].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                        //MessageBox.Show("Процессор не совместим с материнской платой! Измените конфигруцию!", "Что-то пошло не так...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (temp[0] == "RAM")
                {
                    if (i.get("type") != currentDDR && MBInstalled)
                    {
                        dgvAfter.Rows[j].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                        //MessageBox.Show("Оперативная память не совместима с материнской платой! Измените конфигруцию!", "Что-то пошло не так...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //dgvAfter.DefaultCellStyle.BackColor = Color.PaleVioletRed;
                    }
                }
                if (temp[0] == "Motherboard")
                {
                    mb++;
                    //currentSocket = i.get("cpuinterface");
                    //currentDDR = i.get("ramtype");
                }
                j++;
            }
            string msg = "";
            if (mb > 1 && cpu > 1)
                msg += "Более 1 МАТ. ПЛАТЫ и ПРОЦЕССОРА!\n Удалите лишнее!.";
            else if (cpu > 1 && mb == 1)
                msg += "Более 1 ПРОЦЕССОРА!\n Удалите лишнее!.";
            else if (mb > 1 && cpu == 1)
                msg += "Более 1 МАТ. ПЛАТЫ!\n Удалите лишнее!.";

            if (msg.Length > 0)
                MessageBox.Show(msg, "Что-то пошло не так...", MessageBoxButtons.OK, MessageBoxIcon.Error);


        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvAfter.RowCount > 0)
            {
                if (conf[dgvAfter.CurrentCell.RowIndex].toStrArray()[0] == "Motherboard")
                {
                    MBInstalled = false;
                    conf.RemoveAt(dgvAfter.CurrentCell.RowIndex);
                    foreach (var i in conf)
                    {
                        string[] temp = i.toStrArray();
                        if (temp[0] == "Motherboard")
                        {
                            MBInstalled = true;
                            currentSocket = i.get("cpuinterface");
                            currentDDR = i.get("ramtype");
                        }
                    }
                }
                else
                    conf.RemoveAt(dgvAfter.CurrentCell.RowIndex);
                updateMofificationTable();
                if (MBInstalled == false)
                {
                    // dgvAfter.DefaultCellStyle.BackColor = Color.PaleVioletRed;
                    // MessageBox.Show("Для работы компьютера необходима МАТ. ПЛАТА\nВыберите ее!", "Что-то пошло не так...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void initBefore()
        {
            curconf.Clear();
            curconf.Add(sysinfo.cpu);
            curconf.Add(sysinfo.mb);
            foreach (var i in sysinfo.gpu)
                curconf.Add(i);
            foreach (var i in sysinfo.hdd)
                curconf.Add(i);
            foreach (var i in sysinfo.ram)
                curconf.Add(i);
            foreach (var i in sysinfo.rom)
                curconf.Add(i);

            dgvBefore.RowCount = 0;
            dgvBefore.ColumnCount = 2;
            dgvBefore.RowCount = curconf.Count;
            for (int i = 0; i < curconf.Count; i++)
            {
                var temp = curconf[i].toStrArray();
                dgvBefore[0, i].Value = temp[0];
                dgvBefore[1, i].Value = temp[1];
            }

            /*dgvBefore.ColumnCount = 2;

            List<string[]> specs = new List<string[]>();
            specs.Add(sysinfo.cpu.toStrArray());
            specs.Add(sysinfo.mb.toStrArray());
            foreach (var i in sysinfo.gpu)
                specs.Add(i.toStrArray());

            foreach (var i in sysinfo.hdd)
                specs.Add(i.toStrArray());

            foreach (var i in sysinfo.ram)
                specs.Add(i.toStrArray());

            foreach (var i in sysinfo.rom)
                specs.Add(i.toStrArray());
            
            dgvBefore.RowCount = specs.Count;
            for (int i = 0; i < specs.Count; i++)
            {
                dgvBefore[0, i].Value = specs[i][0];
                dgvBefore[1, i].Value = specs[i][1];
            }*/

        }

        void initSysInfo()
        {
            dgvCPU.Rows.Clear();
            dgvHDD.Rows.Clear();
            dgvMB.Rows.Clear();
            dgvPower.Rows.Clear();
            dgvRAM.Rows.Clear();
            dgvROM.Rows.Clear();
            dgvVideo.Rows.Clear();

            dgvCPU.ColumnCount = 2;
            dgvCPU.RowCount = 1;
            dgvHDD.ColumnCount = 2;
            dgvHDD.RowCount = 1;
            dgvMB.ColumnCount = 2;
            dgvMB.RowCount = 1;
            dgvPower.ColumnCount = 2;
            dgvPower.RowCount = 1;
            dgvRAM.ColumnCount = 2;
            dgvRAM.RowCount = 1;
            dgvROM.ColumnCount = 2;
            dgvROM.RowCount = 1;
            dgvVideo.ColumnCount = 2;
            dgvVideo.RowCount = 1;
            /*
            sysinfo.gpu = Sysinfo.gpuinfo();
            sysinfo.cpu = Sysinfo.cpuinfo();
            sysinfo.ram = Sysinfo.raminfo();
            sysinfo.hdd = Sysinfo.hddinfo();
            sysinfo.mb = Sysinfo.mbinfo();
            sysinfo.power = Sysinfo.powerinfo();
            sysinfo.rom = Sysinfo.rominfo();
            
            //блок заглушек
            CPU cpu0 = new CPU();
            cpu0.name = "Intel(R) Core(TM) i5-4570 CPU @ 3.20GHz";
            cpu0.socket = "LGA 1150";
            cpu0.cores = "4";
            cpu0.power = "95";
            sysinfo.cpu = cpu0;

            MB mb0 = new MB();
            mb0.name = "Gigabyte Ga-B85M-D3H";
            mb0.storeinterface = "SATA III";
            mb0.gpuinterface = "PCI-E";
            mb0.ramslot = "4";
            mb0.maxmem = "32";
            mb0.ramtype = "DDR3";
            sysinfo.mb = mb0;

            GPU gpu0 = new GPU();
            gpu0.name = "NVIDIA GeForce GTX 760";
            gpu0.videoram = "2";
            gpu0.interfase = "PCI-E";
            List<GPU> lstgpu = new List<GPU>();
            lstgpu.Add(gpu0);
            sysinfo.gpu = lstgpu;

            HDD hdd0 = new HDD();
            hdd0.name = "ADATA SP920SS";
            hdd0.size = "238,47";
            hdd0.interfase = "";
            HDD hdd1 = new HDD();
            hdd1.name = "Hitachi HDP725050GLA360";
            hdd1.size = "465,76";
            hdd1.interfase = "";
            List<HDD> lsthdd = new List<HDD>();
            lsthdd.Add(hdd0);
            lsthdd.Add(hdd1);
            sysinfo.hdd = lsthdd;

            RAM ram0 = new RAM();
            ram0.size = "4";
            ram0.speed = "1333";
            List<RAM> lstram = new List<RAM>();
            lstram.Add(ram0);
            lstram.Add(ram0);
            sysinfo.ram = lstram;

            ROM rom0 = new ROM();
            rom0.name = "Optiarc DVD RW AD-7203S";
            List<ROM> lstrom = new List<ROM>();
            lstrom.Add(rom0);
            sysinfo.rom = lstrom;

            POWER pwr = new POWER();
            pwr.power = "500";
            sysinfo.power = pwr;
            /// */

            dgvCPU.Rows.Add("Name", sysinfo.cpu.name);
            dgvCPU.Rows.Add("Socket", sysinfo.cpu.socket);
            currentSocket = sysinfo.cpu.socket;
            dgvCPU.Rows.Add("Core count", sysinfo.cpu.cores);
            dgvCPU.Rows.Add("Power", sysinfo.cpu.power);

            dgvMB.Rows.Add("Название", sysinfo.mb.name);
            dgvMB.Rows.Add("Интерфейс устр. хранения", sysinfo.mb.storeinterface);
            dgvMB.Rows.Add("Интерфейс видеокарты", sysinfo.mb.gpuinterface);
            dgvMB.Rows.Add("Количество слотов оперативной памяти", sysinfo.mb.ramslot);
            dgvMB.Rows.Add("Максимум оперативной памяти", sysinfo.mb.maxmem);
            dgvMB.Rows.Add("Тип оперативной памяти", sysinfo.mb.ramtype);
            currentDDR = sysinfo.mb.ramtype;

            foreach (var i in sysinfo.gpu)
            {
                dgvVideo.Rows.Add("Название", i.name);
                dgvVideo.Rows.Add("Объем Видеопамяти", i.videoram);
                dgvVideo.Rows.Add("Интерфейс", i.interfase);
                //dgvVideo.Rows.Add("Потребляемая мощность", i.power);
            }

            foreach (var i in sysinfo.hdd)
            {
                dgvHDD.Rows.Add("Название", i.name);
                dgvHDD.Rows.Add("Размер", i.size);
                //dgvHDD.Rows.Add("Интерфейс", i.interfase);
            }

            foreach (var i in sysinfo.ram)
            {
                //dgvRAM.Rows.Add("Название", i.slot);
                dgvRAM.Rows.Add("Размер", i.size);
                dgvRAM.Rows.Add("Скорость", i.speed);
                //dgvRAM.Rows.Add("Тип", i.type);
            }

            foreach (var i in sysinfo.rom)
            {
                dgvROM.Rows.Add("Название", i.name);
                //dgvROM.Rows.Add("Интерфейс", i.interfase);
            }

            dgvPower.Rows.Add("Мощность", "100500 " + "W");//dgvPower.Rows.Add("Мощность", sysinfo.power.power + "W");


        }

        private void btn_parse_Click(object sender, EventArgs e)
        {

            try
            {
                Parser.generateFile((int)numPages.Value);
            }
            catch (Exception msg)
            {
                MessageBox.Show("Произошла ошибка вовремя создания файла парсера: " + msg.Message);
            }
        }

        void initMonitoring()
        {
            var myComputer = new Computer();
            myComputer.CPUEnabled = true;
            myComputer.GPUEnabled = true;
            myComputer.RAMEnabled = true;
            myComputer.HDDEnabled = true;
            myComputer.MainboardEnabled = true;
            //myComputer.ToCode();
            myComputer.Open();

            foreach (var hardwareItem in myComputer.Hardware)
            {
                hardwareItem.Update();
                //hardwareItem.GetReport();

                foreach (var sensor in hardwareItem.Sensors)
                {
                    // if(sensor.SensorType != SensorType.Fan && sensor.SensorType != SensorType.Control && sensor.SensorType != SensorType.Factor)
                    monitoring_data.Add(new Monitoring(sensor.SensorType, hardwareItem.Name, sensor.Name, Convert.ToDouble(sensor.Value)));
                }
            }
            monitoring_data = monitoring_data.OrderBy(a => a.GetHardwareName()).ThenBy(a => a.GetSensorType()).ThenBy(a => a.GetName()).ToList<Monitoring>();
            //monitoring_data.Sort(
            //    delegate (Monitoring p1, Monitoring p2)
            //    {
            //        int c1 = p1.GetHardwareName().CompareTo(p2.GetHardwareName());
            //        int c2 = p1.GetSensorType().CompareTo(p2.GetSensorType());
            //        if (p1.GetHardwareName() == p2.GetHardwareName())
            //        {
            //            if (p1.GetSensorType() == p2.GetSensorType())
            //                return p1.GetName().CompareTo(p2.GetName());
            //        }
            //        return p1.GetHardwareName().CompareTo(p2.GetHardwareName());
            //    }
            //);

            showMonitoring();
            bgUpdateMonitoring.RunWorkerAsync();
        }

        void updateMonitoring()
        {
            var myComputer = new Computer();
            myComputer.CPUEnabled = true;
            myComputer.GPUEnabled = true;
            myComputer.RAMEnabled = true;
            myComputer.HDDEnabled = true;
            myComputer.MainboardEnabled = true;
            myComputer.Open();

            foreach (var hardwareItem in myComputer.Hardware)
            {
                hardwareItem.Update();

                foreach (var sensor in hardwareItem.Sensors)
                {
                    //UPDATE VALUES

                    int index = monitoring_data.FindIndex(x => (x.GetName() == sensor.Name && x.GetHardwareName() == hardwareItem.Name && x.GetSensorType() == sensor.SensorType));
                    if (index >= 0)
                    {
                        double maxValue = Math.Max(Convert.ToDouble(sensor.Value), monitoring_data[index].GetMaxValue());
                        monitoring_data[index].SetValue((float)Convert.ToDouble(sensor.Value));
                        monitoring_data[index].SetMaxValue((float)maxValue);
                    }
                }
            }

            showMonitoring();
        }

        void showMonitoring()
        {
            List<string[]> monitoring = new List<string[]>();
            List<string> lst = new List<string>();
            monitoring.Add(new string[] { "Hardware name", "Sensor type", "Sensor name", "Value", "Max value" });

            foreach (var item in monitoring_data)
            {
                if (!lst.Contains(item.GetHardwareName()))
                {
                    lst.Clear();
                    monitoring.Add(new string[] { item.GetHardwareName(), "", "", "", "" });
                    lst.Add(item.GetHardwareName());
                }
                if (!lst.Contains(item.GetSensorType().ToString()))
                {
                    monitoring.Add(new string[] { "", item.GetSensorType().ToString(), "", "", "" });
                    lst.Add(item.GetSensorType().ToString());
                }

                if (item.GetSensorType() == SensorType.Clock)
                {
                    monitoring.Add(new string[] { "", "", item.GetName(), String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetValue()) + " MHz", String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetMaxValue()) + " MHz" });
                }
                else if (item.GetSensorType() == SensorType.Temperature)
                {
                    monitoring.Add(new string[] { "", "", item.GetName(), item.GetValue().ToString() + " °C", item.GetMaxValue().ToString() + " °C" });
                }
                else if (item.GetSensorType() == SensorType.Load)
                {
                    monitoring.Add(new string[] { "", "", item.GetName(), String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetValue()) + " %", String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetMaxValue()) + " %" });
                }
                else if (item.GetSensorType() == SensorType.Data)
                {
                    monitoring.Add(new string[] { "", "", item.GetName(), String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetValue()) + " GB", String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetMaxValue()) + " GB" });
                }
                else if (item.GetSensorType() == SensorType.SmallData)
                {
                    monitoring.Add(new string[] { "", "", item.GetName(), String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetValue()) + " MB", String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetMaxValue()) + " MB" });
                }
                else if (item.GetSensorType() == SensorType.Power)
                {
                    monitoring.Add(new string[] { "", "", item.GetName(), String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetValue()) + " W", String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.GetMaxValue()) + " W" });
                }
                else if (item.GetSensorType() == SensorType.Fan)
                {
                    monitoring.Add(new string[] { "", "", item.GetName(), item.GetValue().ToString() + " RPM", item.GetMaxValue().ToString() + " RPM" });
                }
                else if (item.GetSensorType() == SensorType.Control)
                {
                    monitoring.Add(new string[] { "", "", item.GetName(), item.GetValue().ToString() + " ", item.GetMaxValue().ToString() + " " });
                }
            }

            dgvMonitoring.ColumnCount = 5;
            dgvMonitoring.RowCount = monitoring.Count;

            for (int i = 0; i < monitoring.Count; i++)
            {
                dgvMonitoring[0, i].Value = monitoring[i][0];
                dgvMonitoring[1, i].Value = monitoring[i][1];
                dgvMonitoring[2, i].Value = monitoring[i][2];
                dgvMonitoring[3, i].Value = monitoring[i][3];
                dgvMonitoring[4, i].Value = monitoring[i][4];
            }      
        }

        void ReadWriteLog()
        {

            var myComputer = new Computer();
            myComputer.CPUEnabled = true;
            myComputer.GPUEnabled = true;
            myComputer.RAMEnabled = true;
            myComputer.HDDEnabled = true;
            myComputer.MainboardEnabled = true;
            myComputer.Open();
            DataClass.HDD hdd = new DataClass.HDD(0);
            DataClass.CPU cpu = new DataClass.CPU(0, 0);
            DataClass.GPU gpu = new DataClass.GPU(0, 0, 0);
            DataClass.Memory mem = new DataClass.Memory(0);

            foreach (var hardwareItem in myComputer.Hardware)
            {
                var a = hardwareItem.GetType();
                hardwareItem.Update();
                foreach (var sensor in hardwareItem.Sensors)
                {
                    if (hardwareItem.HardwareType == HardwareType.HDD && sensor.Name == "Used Space")
                        hdd.setData(Convert.ToDouble(sensor.Value));

                    if (hardwareItem.HardwareType == HardwareType.RAM && sensor.Name == "Memory")
                        mem.setData(Convert.ToDouble(sensor.Value));

                    if (hardwareItem.HardwareType == HardwareType.CPU && sensor.Name == "CPU Package")
                        cpu.setTemp(Convert.ToDouble(sensor.Value));
                    if (hardwareItem.HardwareType == HardwareType.CPU && sensor.Name == "CPU Total")
                        cpu.setLoad(Convert.ToDouble(sensor.Value));

                    if ((hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAti) && sensor.Name == "GPU Core")
                        gpu.setTemp(Convert.ToDouble(sensor.Value));
                    if ((hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAti) && sensor.Name == "GPU Video Engine")
                        gpu.setLoad(Convert.ToDouble(sensor.Value));
                    if ((hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAti) && sensor.Name == "GPU Memory")
                        gpu.setMemLoad(Convert.ToDouble(sensor.Value));
                }
            }
            Dictionary<string, DataClass> log = new Dictionary<string, DataClass>();
            DataClass data = new DataClass(hdd, gpu, cpu, mem);
            string time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:fff");
            log.Add(time, data);

            string serialized = JsonConvert.SerializeObject(log);
            string FileName = "HLog.json";
            StreamWriter json = new StreamWriter(FileName, true, System.Text.Encoding.GetEncoding(1251)); // Win-кодировка
            json.WriteLine(serialized);
            json.Close();

        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if (conf.Count == 0)
                return;
            ////экспорт в txt файл
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "TXT file|*.txt";
            sfd.Title = "Save Report an TXT File";
            sfd.FileName = "Report";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                StreamWriter txt = new StreamWriter(sfd.FileName, false, System.Text.Encoding.GetEncoding(1251)); // Win-кодировка

                txt.WriteLine("\t\tОтчет программы Modernisator3000 о модернизации ПК");
                txt.WriteLine("\tПрецедент модернизации: " + precedents.Keys.ToArray()[cbPrecedent.SelectedIndex].ToString());
                txt.WriteLine();
                txt.WriteLine("\t\tТекущая конфигурация");
                for (int i = 0; i < conf.Count; i++)
                {
                    var temp = curconf[i].toStrArray();
                    txt.WriteLine(temp[0] + ": " + temp[1]);
                }
                txt.WriteLine();
                txt.WriteLine("\t\tНовая конфигурация");
                for (int i = 0; i < conf.Count; i++)
                {
                    var temp = conf[i].toStrArray();
                    txt.WriteLine(temp[0] + ": " + temp[1]);
                    if (conf[i].webAddress != null)
                        txt.WriteLine("Ссылка на товар: " + conf[i].webAddress);
                }
                txt.Close();
            }
        }

        private void testBtn_Click(object sender, EventArgs e)
        {
            loadAIDAreport();
            initSysInfo();
            initBefore();
            initAfter();
        }
        int loadAIDAreport()
        {
            //Узнаем где лежит файл с отчетом
            var dialog = new OpenFileDialog();
            dialog.Filter = "TXT file(*.txt)|*.txt";
            dialog.Title = "Выберите отчет.";
            string report;
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return 0;
            }
            //Открываем файл с отчетом 

            try
            {
                Stream myStream = File.Open(dialog.FileName, FileMode.Open);
                if ((myStream/* = dialog.OpenFile()*/) != null)
                {
                    using (myStream)
                    {
                        // преобразуем строку в байты
                        byte[] array = new byte[myStream.Length];
                        // считываем данные
                        myStream.Read(array, 0, array.Length);
                        // декодируем байты в строку
                        report = Encoding.Default.GetString(array);
                    }
                    myStream.Close();
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: невозможно считать файл с диска. Оригинальная ошибка: " + ex.Message);
                return 0;
            }
            //var sys = new Sysinfo();
            //Проверяем версию?
            //Ищим информацию по...
            //...ГПУ
            sysinfo.findGpu(report);
            sysinfo.findCpu(report);
            sysinfo.findMB(report);
            sysinfo.findRAM(report);
            sysinfo.findHDD(report);
            sysinfo.findROM(report);
            //...ЦПУ
            //...МП
            //...БП
            //...ОЗУ
            //...ЖД
            //...Привод
            return 1;
        }

        private void bgUpdateMonitoring_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);
                    updateMonitoring();
                    ReadWriteLog();
                }
                catch (Exception)
                {

                }
            }
        }

        private void dgvMonitoringSelectionChanged(object sender, EventArgs e)
        {

            //if (dgvMonitoring.SelectedRows.Count > 0)
            //{
            //    var index = dgvMonitoring.SelectedRows[0].Index;
            //    MessageBox.Show("AAAAAA: " + index.ToString());
            //}

        }

        private void btnUpdateMonitoring_Click(object sender, EventArgs e)
        {
            if (monitoring_data.Count > 0)
            {
                Chart chart = new Chart(monitoring_data);
                chart.Show();
            }
        }

        private void btnAnalyseLog_Click(object sender, EventArgs e)
        {

        }

        private void btnPlayStopLogThread_Click(object sender, EventArgs e)
        {

            if (collection_data)
            {
                collection_data = false;
                bgUpdateMonitoring.CancelAsync();
                bgUpdateMonitoring.Dispose();
                btnPlayStopLogThread.Text = "Enable data collection";
            }
            else
            {
                collection_data = true;
                initMonitoring();
                btnPlayStopLogThread.Text = "Disable data collection";
            }
        }
    }
}
