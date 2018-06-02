using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Modernisator3000
{
    public class DataClass
    {
        public List<HDD> hdd;
        public GPU gpu;
        public CPU cpu;
        public Memory memory;

        public DataClass(List<HDD> hdd, GPU gpu, CPU cpu, Memory memory)
        {
            this.hdd = hdd;
            this.gpu = gpu;
            this.cpu = cpu;
            this.memory = memory;
        }

        public class HDD
        {
            public double used_space;

            public HDD(double used_space)
            {
                this.used_space = used_space;
            }

            public void setData(double used_space)
            {
                this.used_space = used_space;
            }
        }

        public class GPU
        {
            public double temp;
            public double load;
            public double gpumem_load;

            public GPU(double temp, double load, double gpumem_load)
            {
                this.temp = temp;
                this.load = load;
                this.gpumem_load = gpumem_load;
            }

            public void setTemp(double temp)
            {
                this.temp = temp;
            }

            public void setLoad(double load)
            {
                this.load = load;
            }

            public void setMemLoad(double gpumem_load)
            {
                this.gpumem_load = gpumem_load;
            }
        }

        public class CPU
        {
            public double temp;
            public double load;

            public CPU(double temp, double load)
            {
                this.temp = temp;
                this.load = load;
            }

            public void setTemp(double temp)
            {
                this.temp = temp;
            }
            public void setLoad(double load)
            {
                this.load = load;
            }
        }

        public class Memory
        {
            public double used_space;

            public Memory(double used_space)
            {
                this.used_space = used_space;
            }

            public void setData(double used_space)
            {
                this.used_space = used_space;
            }
        }
    }
}
