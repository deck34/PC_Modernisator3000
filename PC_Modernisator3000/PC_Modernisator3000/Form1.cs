﻿using System;
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

namespace PC_Modernisator3000
{
    public partial class Modernizator : Form
    {
        
        Sysinfo sysinfo = new Sysinfo();
        // Sysinfo sysinfoModified = new Sysinfo();
        List<Item> curconf = new List<Item>();
        List<Item> conf = new List<Item>();
        List<Item> templist = new List<Item>();

        string currentSocket = "";
        string currentDDR = "";
        bool MBInstalled = false;

        Dictionary<string, string[]> precedents = new Dictionary<string, string[]>();

        //string[] precedents = { "Проблемы с изображением", "Недостаточная производительность ПК", "Компьютер долго запускается" };
        //string[] devicetypes = {"Процессор","Материнская плата", "Видеокарта", "Оперативная память", "Жесткий диск / SSD", "Блок питания", "Оптический привод", "Корпус", "Охлаждение процессора"};

        private DateTime now;
        protected readonly ListSet<ISensor> active = new ListSet<ISensor>();
        public event SensorEventHandler SensorAdded;
        public event SensorEventHandler SensorRemoved;

        protected virtual void ActivateSensor(ISensor sensor)
        {
            if (active.Add(sensor))
                if (SensorAdded != null)
                    SensorAdded(sensor);
        }

        public Modernizator()
        {
            InitializeComponent();
            cbPrecedent.SelectedIndexChanged += cbPrecedentTypeSelectedIndexChanged;
            //Parser.generateFile(0, null);
            cbDeviceType.SelectedIndexChanged += cbDeviceTypeSelectedIndexChanged;
            tabControl1.SelectedIndexChanged += tabControl1SelectedIndexChanged;
            btnAdd.Click += btnAdd_Click;
            firstfunction();
            initPrecedents();
            updateMonitoring();
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
            }catch(Exception msg)
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
                        list.Add(i.toStrArray()[1] );
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
            foreach(var i in conf)
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

        private void btnUpdateMonitoring_Click(object sender, EventArgs e)
        {
            updateMonitoring();
        }

        private void tabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 2)
            {
                //TO DO функция для обновления данных с сенсоров
            }
        }

        void updateMonitoring()
        {
            //ManagementObjectSearcher searcher11 =
            //    new ManagementObjectSearcher("root\\CIMV2",
            //    "SELECT * FROM Win32_Processor  ");
            List<string[]> monitoring = new List<string[]>();
            //foreach (ManagementObject queryObj in searcher11.Get())
            //{
            //    monitoring.Add(new string[] { "Загрузка процессора", queryObj["LoadPercentage"].ToString() + " %" });

            //}
            //try
            //{


            //    ManagementObjectSearcher searcher12 =
            //        new ManagementObjectSearcher("root\\WMI",
            //        "SELECT * FROM MSAcpi_ThermalZoneTemperature  ");
            //    foreach (ManagementObject queryObj in searcher12.Get())
            //    {
            //        monitoring.Add(new string[] { "Температура процессора", Convert.ToString(Convert.ToDouble(queryObj["CurrentTemperature"]) / 10 - 273.15) + " °C" });

            //    }
            //}
            //catch (Exception e)
            //{
            //    //throw new Exception("Для вывода температуры запустите программу от имени администратора!");
            //}
            //ManagementObjectSearcher searcher13 =
            //    new ManagementObjectSearcher("SELECT * FROM meta_class WHERE __class = 'Win32_LogicalDisk'");
            ///*foreach (ManagementObject queryObj in searcher13.Get())
            //{
            //    monitoring.Add(new string[] { "Место", queryObj["Size"].ToString() + " %" } );
            //}*/

            //ObjectQuery DiskQuery = new System.Management.ObjectQuery("select FreeSpace, Size from Win32_LogicalDisk where DriveType = 3");
            //ManagementObjectSearcher DiskSearcher = new ManagementObjectSearcher(DiskQuery);
            //ManagementObjectCollection DiskCollection = DiskSearcher.Get();
            //foreach (ManagementObject DiskInfo in DiskCollection)
            //{
            //    monitoring.Add(new string[] { "Всего/свободо места на диске", Math.Round(System.Convert.ToDouble(DiskInfo["Size"]) / 1024 / 1024 / 1024, 2).ToString() + " / " + Math.Round(System.Convert.ToDouble(DiskInfo["FreeSpace"]) / 1024 / 1024 / 1024, 2).ToString() + " Gb." });
            //}

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
                   // if (sensor.SensorType == SensorType.Temperature)
                    //{
                        //Console.WriteLine("{0} {1} {2} = {3}", sensor.Name, sensor.Hardware, sensor.SensorType, sensor.Value);
                        monitoring.Add(new string[] { sensor.Name.ToString() + " " + sensor.SensorType.ToString(),  sensor.Value.ToString()});
                    //}

                }
            }

            dgvMonitoring.ColumnCount = 2;
            dgvMonitoring.RowCount = monitoring.Count;

            for (int i = 0; i < monitoring.Count; i++)
            {
                dgvMonitoring[0, i].Value = monitoring[i][0];
                dgvMonitoring[1, i].Value = monitoring[i][1];
            }
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
                    txt.WriteLine(temp[0]+": "+ temp[1]);
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

    }
}