using System;
using System.Management;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

namespace PC_Modernisator3000
{
    

    class Sysinfo
    {
        public static Encoding encode = Encoding.GetEncoding("utf-8");
        public List<GPU> gpu { get; set; }
        public CPU cpu { get; set; }
        public List<RAM> ram { get; set; }
        public List<HDD> hdd { get; set; }
        public List<ROM> rom { get; set; }
        public MB mb { get; set; }
        public POWER power { get; set; }


        public static List<GPU> gpuinfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_VideoController");
            List<GPU> gpu = new List<GPU>();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                GPU gpu0 = new GPU();
                gpu0.name = queryObj["Name"].ToString();
                gpu0.videoram = Math.Round(System.Convert.ToDouble(queryObj["AdapterRAM"]) / 1024 / 1024 / 1024, 2).ToString();
                //gpu.interfase = queryObj["VideoMemoryType"].ToString();
                gpu.Add(gpu0);
            }
            return gpu;
        }

        public static CPU cpuinfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_Processor");
            CPU cpu = new CPU();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                cpu.name = queryObj["Name"].ToString();
                //cpu.socket = queryObj["VoltageCaps"].ToString();
                cpu.cores = queryObj["NumberOfCores"].ToString();
            }
            return cpu;
        }

        public static MB mbinfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_MotherboardDevice ");
            MB mb = new MB();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                mb.name = queryObj["Name"].ToString();
                mb.gpuinterface = queryObj["PrimaryBusType"].ToString();
            }
            return mb;
        }

        public static POWER powerinfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_Battery  ");
            POWER power = new POWER();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                power.power = queryObj["Caption"].ToString();
            }
            return power;
        }

        public static List<RAM> raminfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_PhysicalMemory");
            List<RAM> memory = new List<RAM>();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                RAM memory0 = new RAM();
                memory0.size = Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2).ToString();
                memory0.speed = queryObj["Speed"].ToString();
                memory0.slot = queryObj["BankLabel"].ToString();
                //memory0.type = queryObj["Attributes"].ToString();
                memory.Add(memory0);
            }
            return memory;
        }

        public static List<HDD> hddinfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_DiskDrive");
            List<HDD> hdd = new List<HDD>();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                HDD hdd0 = new HDD();
                hdd0.name = queryObj["Model"].ToString();
                hdd0.size = Math.Round(System.Convert.ToDouble(queryObj["Size"]) / 1024 / 1024 / 1024, 2).ToString();
                hdd0.interfase = queryObj["InterfaceType"].ToString();
                hdd.Add(hdd0);
            }
            return hdd;
        }

        public static List<ROM> rominfo()
        {
            ManagementObjectSearcher searcher11 =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_CDROMDrive");
            List<ROM> rom = new List<ROM>();
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                ROM rom0 = new ROM();
                rom0.name = queryObj["Name"].ToString();
                //rom0.interfase = queryObj["Status"].ToString();
                rom.Add(rom0);
            }
            return rom;
        }

        public List<GPU> findGpu(string file)
        {
            string patternName = "(?<=Свойства видеоадаптера:\\s+Описание устройства\\s+)\\S.+$";
            var NameMatches = Regex.Matches(file, patternName, RegexOptions.Multiline);
            string patternVRAM = "(?<=Объем видеоОЗУ\\s+)\\S.+$";
            var RAMMatches = Regex.Matches(file, patternVRAM, RegexOptions.Multiline);
            string patternInterface = "(?<=Разъёмы расширения\\s+)\\S.+$";
            var InterfaceMatches = Regex.Matches(file, patternInterface, RegexOptions.Multiline);
            var Interfaces = Regex.Replace(InterfaceMatches[0].Value, "(\\d| )", "");
            string patternPower = "";
            var gpus = new List<GPU>();
            List<string> added = new List<string>();
            for(var i = 0; i < NameMatches.Count; ++i)
            {
                if(added.IndexOf(NameMatches[i].Value) == -1)
                {
                    gpus.Add(new GPU(NameMatches[i].Value, Interfaces, RAMMatches[i].Value, ""));
                    added.Add(NameMatches[i].Value);
                }
            }
            gpu = gpus;
            return gpus;
        }

        public CPU findCpu(string file)
        {

            string patternName = "(?<=Имя ЦП CPUID\\s+)\\S.+$";
            var nameMatch = Regex.Matches(file, patternName, RegexOptions.Multiline)[0].Value;
            string patternSocket = "(?<=Число гнёзд для ЦП\\s+)\\S.+$";
            var socketMatch = Regex.Matches(file, patternSocket, RegexOptions.Multiline)[0].Value;
            if(socketMatch.IndexOf("LGA") != -1)
                socketMatch = socketMatch.Remove(0, 2).Insert(3, " "); //Плохо работает с процессорами АМД.
            string patternCores = @"\sCPU #\d\s";
            var coresMatch = Regex.Matches(file, patternCores, RegexOptions.Multiline).Count;
            string patternPower = "(?<=Типичная мощность\\s+)\\S.+$";
            var powerMatch = Regex.Matches(file, patternPower, RegexOptions.Multiline)[0].Value;
            string patternFreq = @"\d+(?=\s+MHz)";
            var freqMatch = Regex.Matches(file, patternFreq, RegexOptions.Multiline)[0].Value;
            cpu = new CPU(nameMatch, socketMatch, coresMatch.ToString(), powerMatch, freqMatch);
            return cpu;
        }

        public MB findMB(string file)
        {
            string patternName = "(?<=\\sСистемная плата\\s+)\\S.+$";
            var nameMatch = Regex.Matches(file, patternName, RegexOptions.Multiline)[0].Value;
            if (nameMatch == "Неизвестно")
                throw new Exception("Невозможно определить плату.");

            string patternSocket = "(?<=Число гнёзд для ЦП\\s+)\\S.+$";
            var socketMatch = Regex.Matches(file, patternSocket, RegexOptions.Multiline)[0].Value;
            if (socketMatch.IndexOf("LGA") != -1)
                socketMatch = socketMatch.Remove(0, 2).Insert(3, " "); //Плохо работает с процессорами АМД.

            string patternInterface = "(?<=Разъёмы расширения\\s+)\\S.+$";
            var gpuInterfaceMatches = Regex.Matches(file, patternInterface, RegexOptions.Multiline);
            var gpuInterfaces = Regex.Replace(gpuInterfaceMatches[0].Value, "(\\d| )", "");

            string patternRamSlots = "(?<=Устройства памяти\\s+)\\d+";
            var ramSlotsMatches = Regex.Matches(file, patternRamSlots, RegexOptions.Multiline);

            string patternMaxRam = "(?<=\\s+Макс. объём памяти\\s+)\\d+.+?$";
            var maxRamMatches = Regex.Matches(file, patternMaxRam, RegexOptions.Multiline);

            string patternRamType = "DDR\\d";
            var ramTypeMatches = Regex.Matches(file, patternRamType, RegexOptions.Multiline);

            if (gpuInterfaceMatches.Count == 0 || ramSlotsMatches.Count == 0 || maxRamMatches.Count == 0 || ramTypeMatches.Count == 0)
                throw new Exception("Невозможно определить параметры платы.");

            string patternStoreInterface = "(?<=Интерфейс\\s+)SATA.+$";
            var storeInterfaceMatches = Regex.Matches(file, patternStoreInterface, RegexOptions.Multiline);
            if (storeInterfaceMatches.Count == 0)
                throw new Exception("Не найдена информация об интерфейсах жестких дисков.");
            mb = new MB(nameMatch, storeInterfaceMatches[0].Value, gpuInterfaces, socketMatch, ramSlotsMatches[0].Value, maxRamMatches[0].Value, ramTypeMatches[0].Value);
            return mb;
        }

        public POWER findPower(string file)
        {
            power = new POWER("", "");
            return power;
        }

        public List<RAM> findRAM(string file)
        {
            //string patternName = @"(?<=DIMM\d: ).+?(?=\s{2,})";//Возможно не стабильный вариант
            string patternName = @"(?<=Имя модуля\s+)\S.+$";
            var nameMatch = Regex.Matches(file, patternName, RegexOptions.Multiline);

            string patternType = @"(?<=Тип памяти\s+)\S.+\s";
            var typeMatch = Regex.Matches(file, patternType, RegexOptions.Multiline);

            string patternSize = @"(?<=Размер модуля\s+)\S.+(?=\s\()";
            var sizeMatch = Regex.Matches(file, patternSize, RegexOptions.Multiline);

            string patternSpeed = @"(?<=Макс\. частота\s+)\S.+$";
            var speedMatch = Regex.Matches(file, patternSpeed, RegexOptions.Multiline);

            var ramList = new List<RAM>();
            for(var i = 0; i < nameMatch.Count; ++i)
            {
                ramList.Add(new RAM(nameMatch[i].Value, typeMatch[i].Value, sizeMatch[i].Value, speedMatch[i].Value, ""));
            }
            ram = ramList;

            return ramList;
        }

        public List<HDD> findHDD(string file)
        {
            string patternAllInfo = @"(?<=$\s+Дисковый накопитель\s+)\S.+$";
            var matches = Regex.Matches(file, patternAllInfo, RegexOptions.Multiline);
            var listHDD = new List<HDD>();
            for(var i = 0; i < matches.Count; ++i)
            {
                var tmp = matches[i].Value;
                var name = Regex.Match(tmp, @".+(?=\s+\()");
                var interfase = Regex.Match(tmp, @"SATA-.+(?=\))");
                var size = Regex.Match(tmp, @"\d+ ГБ");
                listHDD.Add(new HDD(name.Value, name == null? "" :interfase.Value, size == null ? "" : size.Value));
            }
            hdd = listHDD;
            return hdd;
        }

        public List<ROM> findROM(string file)
        {
            string patternName = @"(?<=\[ DVD-дисководы и дисководы компакт-дисков /\s).+(?=\s])";
            var nameMatches = Regex.Matches(file, patternName, RegexOptions.Multiline);
            var romList = new List<ROM>();
            for (var i = 0; i < nameMatches.Count; ++i)
            {
                romList.Add(new ROM(nameMatches[i].Value, "SATA"));
            }
            rom = romList;
            return rom;
        }
    }

    class CPU : Item
    {
        public string name { get; set; }
        public string socket { get; set; }
        public string cores { get; set; }
        public string power { get; set; }
        public string freq { get; set; }
        public CPU()
        {

        }
        public CPU(string name, string socket, string cores, string power, string freq)
        {
            this.name = name.Replace("\r", "");
            this.socket = socket.Replace("\r", "");
            this.cores = cores.Replace("\r", "");
            this.power = power.Replace("\r", "");
            this.freq = freq.Replace("\r", "");
        }
        public override string[] toStrArray()
        {
            string[] str = { "CPU", name + " \nFrequency: " + freq + " \nCore  count: " + cores + " \nSocket: " + socket };
            return str;
        }
        override public void add(string field, string value)
        {
            switch (field)
            {
                case "Имя":
                    name = value;
                    break;
                case "Гнездо процессора":
                    socket = value;
                    break;
                case "Количество ядер":
                    cores = value;
                    break;
                case "Тепловыделение":
                    power = value;
                    break;
                case "Частота":
                    freq = value;
                    break;

            }
        }
        public override string get(string field)
        {
            switch (field)
            {
                case "name":
                    return name;
                case "socket":
                    return socket;
                case "cores":
                    return cores;
                case "power":
                    return power;
                case "freq":
                    return freq;
                default:
                    throw new Exception("No field avaible");

            }
        }
    }

    class MB : Item
    {
        public string name { get; set; }
        public string storeinterface { get; set; }
        public string gpuinterface { get; set; }
        public string cpuinterface { get; set; }
        public string ramslot { get; set; }
        public string maxmem { get; set; }
        public string ramtype { get; set; }
        public MB()
        {

        }
        public MB(string name, string storeinterface, string gpuinterface, string cpuinterface, string ramslot, string maxmem, string ramtype)
        {
            this.name = name.Replace("\r", "");
            this.storeinterface = storeinterface.Replace("\r", "");
            this.cpuinterface = cpuinterface.Replace("\r", "");
            this.ramslot = ramslot.Replace("\r", "");
            this.maxmem = maxmem.Replace("\r", "");
            this.ramtype = ramtype.Replace("\r", "");
            this.gpuinterface = gpuinterface.Replace("\r", "");
        }
        public override string[] toStrArray()
        {
            string[] str = { "Motherboard", name + " \nSocket: " + cpuinterface + " \nRAM slots count: " + ramslot + " \nRAM type: " + ramtype + " \nROM interfase: " + storeinterface };
            return str;
        }
        override public void add(string field, string value)
        {
            switch (field)
            {
                case "Имя":
                    name = value;
                    break;
                case "Разъемов SATA2":
                    storeinterface = "SATA2";
                    break;
                case "Разъемов SATA3":
                    storeinterface = "SATA3";
                    break;
                case "Слотов PCI-E x1":
                    gpuinterface = value;
                    break;
                case "Слотов PCI-E 2.0 x16":
                    gpuinterface = value;
                    break;
                case "Слотов памяти DDR3":
                    ramslot = value;
                    ramtype = "DDR3";
                    break;
                case "Слотов памяти DDR2":
                    ramslot = value;
                    ramtype = "DDR2";
                    break;
                case "Максимальный объем оперативной памяти":
                    maxmem = value;
                    break;
                case "Гнездо процессора":
                    cpuinterface = value;
                    break;

            }
        }
        public override string get(string field)
        {
            switch(field)
            {
                case "name":
                    return name;
                case "storeinterface":
                    return storeinterface;
                case "gpuinterface":
                    return gpuinterface;
                case "ramslot":
                    return ramslot;
                case "maxmem":
                    return maxmem;
                case "ramtype":
                    return ramtype;
                case "cpuinterface":
                    return cpuinterface;
                default:
                    throw new Exception("No field avaible");
            }
        }
    }

    class HDD : Item
    {
        public string name { get; set; }
        public string interfase { get; set; }
        public string size { get; set; }
        public HDD() { }
        public HDD(string name, string interfase, string size)
        {
            this.name = name;
            this.interfase = interfase;
            this.size = size;
        }
        public override string[] toStrArray()
        {
            string[] str = { "HDD / SSD", name + " \nSize: " + size };
            return str;
        }
        override public void add(string field, string value)
        {
            switch(field)
            {
                case "Имя":
                    name = value;
                    break;
                case "Объем накопителя":
                    size = value;
                    break;
                case "Интерфейс":
                    interfase = value;
                    break;
            }
        }
        public override string get(string field)
        {
            switch (field)
            {
                case "name":
                    return name;
                case "size":
                    return size;
                case "interfase":
                    return interfase;
                default:
                    throw new Exception("No field avaible");
            }
        }
    }

    class ROM : Item
    {
        public string name { get; set; }
        public string interfase { get; set; }
        public ROM() { }
        public ROM(string name, string interfase)
        {
            this.name = name;
            this.interfase = interfase;
        }
        public override string[] toStrArray()
        {
            string[] str = { "Recording device", name };
            return str;
        }
        override public void add(string field, string value)
        {
            switch (field)
            {
                case "Имя":
                    name = value;
                    break;
                case "Интерфейс":
                    interfase = value;
                    break;

            }
        }
        public override string get(string field)
        {
            switch (field)
            {
                case "name":
                    return name;
                case "interfase":
                    return interfase;
                default:
                    throw new Exception("No field avaible");

            }
        }
    }

    class RAM : Item
    {
        public string name { get; set; }
        public string type { get; set; }
        public string size { get; set; }
        public string speed { get; set; }
        public string slot { get; set; }
        public RAM()
        {

        }
        public RAM(string name, string type, string size, string speed, string slot)
        {
            this.name = name.Replace("\r", "");
            var tmpType = type.Replace("\r", "");
            tmpType = Regex.Match(type, @"DDR\d").Value;
            this.type = tmpType;
            this.size = size.Replace("\r", "");
            this.speed = speed.Replace("\r", "");
            this.slot = slot.Replace("\r", "");
        }
        public override string[] toStrArray()
        {
            if (name != null)
            {
                string[] str = { "RAM",name + " \nSize: " + size + " \nSpeed: " + speed };
                return str;
            }
            else
            {
                string[] str = { "RAM", "Size: " + size + " \nSpeed: " + speed };
                return str;
            }
        }
        override public void add(string field, string value)
        {
            switch (field)
            {
                case "Имя":
                    name = value;
                    break;
                case "Тип памяти":
                    type = value;
                    break;
                case "Объем":
                    size = value;
                    break;
                case "Скорость":
                    speed = value;
                    break;
            }
        }
        public override string get(string field)
        {
            switch (field)
            {
                case "name":
                    return name;
                case "type":
                    return type;
                case "size":
                    return size;
                case "speed":
                    return speed;
                default:
                    throw new Exception("No field avaible");
            }
        }
    }

    class GPU : Item
    {
        public string name { get; set; }
        public string videoram { get; set; }
        public string interfase { get; set; }
        public string power { get; set; }
        public GPU(string name, string interfase, string videoram, string power)
        {
            this.name = name.Replace("\r", "");
            this.interfase = interfase.Replace("\r", "");
            this.videoram = videoram.Replace("\r", "");
            this.power = power.Replace("\r", "");
        }
        public GPU()
        {

        }
        public override string[] toStrArray()
        {
            string[] str = { "GPU", name + " \nVideoram: " + videoram };
            return str;
        }
        override public void add(string field, string value)
        {
            switch (field)
            {
                case "Имя":
                    name = value;
                    break;
                case "Объем видеопамяти":
                    videoram = value;
                    break;
                case "Интерфейс":
                    interfase = value;
                    break;
                case "Мощность":
                    //power = value;
                    break;
            }
        }
        public override string get(string field)
        {
            switch (field)
            {
                case "name":
                    return name;
                case "videoram":
                    return videoram;
                case "interfase":
                    return interfase;
                case "Мощность":
                    //power = value;
                    return "";
                default:
                    throw new Exception("No field avaible");
            }
        }
    }

    class POWER : Item
    {
        public string power { get; set; }
        public string name { get; set; }
        public POWER()
        {

        }
        public POWER(string power, string name)
        {
            this.power = power;
            this.name = name;
        }
        public override string[] toStrArray()
        {
            string[] str = { "Power supply", name + " \nPower: " + power };
            return str;
        }
        override public void add(string field, string value)
        {
            switch (field)
            {
                case "Имя":
                    name = value;
                    break;
                case "Мощность":
                    power = value;
                    break;
            }
        }
        public override string get(string field)
        {
            switch (field)
            {
                case "name":
                    return name;
                case "power":
                    return power;
                default:
                    throw new Exception("No field avaible");
            }
        }
    }

    abstract class Item
    {
        public string webAddress;
        abstract public string[] toStrArray();
        abstract public string get(string field);
        abstract public void add(string field, string value);
        public void fillItem(HtmlNodeCollection tableOfParams)
        {
            foreach (var node in tableOfParams)
            {
                if (node.GetAttributeValue("class", "") == "")
                {
                    var field = node.FirstChild.FirstChild.InnerText;
                    var value = node.ChildNodes[1].InnerText;
                    add(field, value);
                }
            }

        }
        public static Dictionary<string, string> createItem(HtmlNodeCollection tableOfParams)
        {
            var dict = new Dictionary<string, string>();
            foreach (var node in tableOfParams)
            {
                if (node.GetAttributeValue("class", "") == "")
                {
                    var field = node.FirstChild.FirstChild.InnerText;
                    var value = node.ChildNodes[1].InnerText;
                    dict.Add(field, value);
                }
            }
            return dict;
        }
    }
}
