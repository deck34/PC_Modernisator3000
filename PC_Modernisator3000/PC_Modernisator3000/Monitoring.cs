using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;

namespace PC_Modernisator3000
{
    public class Monitoring
    {
        private SensorType sensorType;
        private String HardwareName;
        private String name;
        private double value;
        private double maxValue;

        public Monitoring(SensorType sensorType, String HardwareName, String name, double value)
        {
            this.sensorType = sensorType;
            this.HardwareName = HardwareName;
            this.name = name;
            this.value = value;
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
            return this.value;
        }

        public void SetValue(float value)
        {
            this.value = value;
        }

        public double GetMaxValue()
        {
            return this.maxValue;
        }

        public void SetMaxValue(float maxValue)
        {
            this.maxValue = maxValue;
        }
    }
}