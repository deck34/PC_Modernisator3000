using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PC_Modernisator3000
{
    enum ProblemParts { OK, GPU, HDD, RAM, CPU };
    enum Problems {
        OK, HDD_LACK_OF_MEMORY, RAM_LACK_OF_MEMORY, CPU_OVERHEAT, CPU_OVERLOAD,
        CPU_TROTTING, GPU_OVERHEAT, GPU_OVERLOAD, GPU_LACK_OF_MEMORY, GPU_TROTTING
    };
    class Analysis
    {
        Dictionary<string, DataClass> log;
        public Dictionary<ProblemParts, double> problemParts = new Dictionary<ProblemParts, double>();
        public Dictionary<Problems, double> indicatedProblems = new Dictionary<Problems, double>();

        public Analysis(string path)
        {
            // This text is added only once to the file.
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                log = JsonConvert.DeserializeObject<Dictionary<string, DataClass>>(json);
            }
        }

        public void run()
        {
            analyse();
        }
        private void analyse()
        {
            var vals = log.Values;
            foreach (var val in vals)
            {
                var resParts = analyzeParts(val);
                var resProblems = analyzeProblems(val);
                foreach (var part in resParts)
                {
                    if (problemParts.ContainsKey(part))
                    {
                        problemParts[part] += 1;
                    }
                    else
                    {//init key
                        problemParts[part] = 1;
                    }
                }

                foreach (var problem in resProblems)
                {
                    if (indicatedProblems.ContainsKey(problem))
                    {
                        indicatedProblems[problem] += 1;
                    }
                    else
                    {//init key
                        indicatedProblems[problem] = 1;
                    }
                }

            }
            foreach (var key in problemParts.Keys.ToList())
                problemParts[key] = problemParts[key] / problemParts.Keys.Count;

            foreach (var key in indicatedProblems.Keys.ToList())
                indicatedProblems[key] = indicatedProblems[key] / indicatedProblems.Keys.Count;
            return;
        }

        private List<ProblemParts> analyzeParts(DataClass data)
        {
            var listOfProblems = new List<ProblemParts>();
            //Check load hdd
            foreach(var hdd in data.hdd)
            {
                if(hdd.used_space > 90.0)
                {
                    listOfProblems.Add(ProblemParts.HDD);
                }
            }
            //check load ram
            if(data.memory.used_space > 95.0)
            {
                listOfProblems.Add(ProblemParts.RAM);
            }
            //check cpu temp
            if (data.cpu.temp > 70.0)
            {
                listOfProblems.Add(ProblemParts.CPU);
            }
            //check cpu load
            if (data.cpu.load > 90.0)
            {
                listOfProblems.Add(ProblemParts.CPU);
            }
            //check cpu trotling
            if(data.cpu.temp > 70.0 && data.cpu.load < 50.0)
            {
                listOfProblems.Add(ProblemParts.CPU);
            }

            //check gpu temp
            if (data.gpu.temp > 90.0)
            {
                listOfProblems.Add(ProblemParts.GPU);
            }
            //check gpu load
            if (data.gpu.load > 90.0)
            {
                listOfProblems.Add(ProblemParts.GPU);
            }
            //check gpu memload
            if (data.gpu.gpumem_load > 90.0)
            {
                listOfProblems.Add(ProblemParts.GPU);
            }
            //check gpu trotling
            if (data.gpu.temp > 90.0 && data.gpu.load < 50.0)
            {
                listOfProblems.Add(ProblemParts.GPU);
            }

            if (listOfProblems.Count == 0)
                listOfProblems.Add(ProblemParts.OK);

            return listOfProblems;
        }

        private List<Problems> analyzeProblems(DataClass data)
        {
            var listOfProblems = new List<Problems>();
            //Check load hdd
            foreach (var hdd in data.hdd)
            {
                if (hdd.used_space > 90.0)
                {
                    listOfProblems.Add(Problems.HDD_LACK_OF_MEMORY);
                }
            }
            //check load ram
            if (data.memory.used_space > 95.0)
            {
                listOfProblems.Add(Problems.RAM_LACK_OF_MEMORY);
            }
            //check cpu temp
            if (data.cpu.temp > 70.0)
            {
                listOfProblems.Add(Problems.CPU_OVERHEAT);
            }
            //check cpu load
            if (data.cpu.load > 90.0)
            {
                listOfProblems.Add(Problems.CPU_OVERLOAD);
            }
            //check cpu trotling
            if (data.cpu.temp > 70.0 && data.cpu.load < 50.0)
            {
                listOfProblems.Add(Problems.CPU_TROTTING);
            }

            //check gpu temp
            if (data.gpu.temp > 90.0)
            {
                listOfProblems.Add(Problems.GPU_OVERHEAT);
            }
            //check gpu load
            if (data.gpu.load > 90.0)
            {
                listOfProblems.Add(Problems.GPU_OVERLOAD);
            }
            //check gpu memload
            if (data.gpu.gpumem_load > 90.0)
            {
                listOfProblems.Add(Problems.GPU_LACK_OF_MEMORY);
            }
            //check gpu trotling
            if (data.gpu.temp > 90.0 && data.gpu.load < 50.0)
            {
                listOfProblems.Add(Problems.GPU_TROTTING);
            }
            return listOfProblems;
        }
    }
}
