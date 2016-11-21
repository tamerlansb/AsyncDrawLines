using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncDrawLines;
using ParrallelPrograming_task1_;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Model obj = new Model();
            List<line> lines = obj.CalcLevelLine(10);
            Console.WriteLine("{0}", lines.Count);
            foreach (var c in obj.grid)
                Console.WriteLine(c);
            Console.ReadLine();
        }
    }
}
