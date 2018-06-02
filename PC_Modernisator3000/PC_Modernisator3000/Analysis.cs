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
    enum Problem { OK, GPU, HDD, RAM, CPU };
    class Analysis
    {
        Dictionary<string, DataClass> log;
        public Analysis(string path)
        {
            // This text is added only once to the file.
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                log = JsonConvert.DeserializeObject<Dictionary<string, DataClass>>(json);
            }
        }

        public Dictionary<Problem, double> run()
        {
            var vals = log.Values;
            var problems = new Dictionary<Problem, double>();
            foreach(var val in vals)
            {
                var res = isProblematic(val);
                foreach(var problem in res)
                {
                    if (problems.ContainsKey(problem))
                    {
                        problems[problem] += 1;
                    }
                    else
                    {//init key
                        problems[problem] = 1;
                    }
                }
                
            }
            foreach (var key in problems.Keys.ToList())
                problems[key] = problems[key] / vals.Count;
            return problems;
        }

        private List<Problem> isProblematic(DataClass data)
        {
            var listOfProblems = new List<Problem>();
            //Check load hdd
            foreach(var hdd in data.hdd)
            {
                if(hdd.used_space > 90.0)
                {
                    listOfProblems.Add(Problem.HDD);
                }
            }
            //check load ram
            if(data.memory.used_space > 95.0)
            {
                listOfProblems.Add(Problem.RAM);
            }
            //check cpu temp
            if (data.cpu.temp > 70.0)
            {
                listOfProblems.Add(Problem.CPU);
            }
            //check cpu load
            if (data.cpu.load > 90.0)
            {
                listOfProblems.Add(Problem.CPU);
            }
            //check cpu trotling
            if(data.cpu.temp > 70.0 && data.cpu.load < 50.0)
            {
                listOfProblems.Add(Problem.CPU);
            }

            //check gpu temp
            if (data.gpu.temp > 90.0)
            {
                listOfProblems.Add(Problem.GPU);
            }
            //check gpu load
            if (data.gpu.load > 90.0)
            {
                listOfProblems.Add(Problem.GPU);
            }
            //check gpu memload
            if (data.gpu.gpumem_load > 90.0)
            {
                listOfProblems.Add(Problem.GPU);
            }
            //check gpu trotling
            if (data.gpu.temp > 90.0 && data.gpu.load < 50.0)
            {
                listOfProblems.Add(Problem.GPU);
            }
            return listOfProblems;
        }
    }
}
