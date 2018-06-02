using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;

namespace PC_Modernisator3000
{
    public class VALUE
    {
        private double value;
        private DateTime dtime;
        public VALUE(double value, DateTime dtime)
        {
            this.value = value;
            this.dtime = dtime;
        }

        public double getVal()
        {
            return this.value;
        }
        public DateTime getTime()
        {
            return this.dtime;
        }
    }
    public class Monitoring
    {
        private SensorType sensorType;
        private String HardwareName;
        private String name;
        private List<VALUE> value = new List<VALUE>();
        private double maxValue;

        public Monitoring(SensorType sensorType, String HardwareName, String name, double value)
        {
            this.sensorType = sensorType;
            this.HardwareName = HardwareName;
            this.name = name;
            this.value.Add(new VALUE(value,DateTime.Now.ToLocalTime()));
            this.maxValue = value;
        }

        public SensorType GetSensorType()
        {
            return this.sensorType;
        }

        public String GetHardwareName()
        {
            return this.HardwareName;
        }
        public String GetName()
        {
            return this.name;
        }

        public double GetValue()
        {
            return this.value.Last().getVal();
        }

        public void SetValue(float value)
        {
            this.value.Add(new VALUE(value, DateTime.Now.ToLocalTime()));
        }

        public double GetMaxValue()
        {
            return this.maxValue;
        }

        public void SetMaxValue(float maxValue)
        {
            this.maxValue = maxValue;
        }

        public List<VALUE> getAllValue()
        {
            return this.value;
        }

        public string toString()
        {
            return HardwareName.ToString() + ";" + sensorType.ToString() + ";" + name.ToString() + ";" + value.ToString();
        }
    }
}