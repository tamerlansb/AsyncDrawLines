using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParrallelPrograming_task1_;
using System.Windows;

namespace AsyncDrawLines
{
    public class Model
    {
        private Dictionary<float,List<line>> lines;
        private  float[,] grid;
        private int N1, N2, l1, l2;
        public Model()
        {
            DownloadGrid();
            lines = new Dictionary<float, List<line>>();
        }
        public List<line> CalcLevelLine(float SetValueLevel)
        {
            if (lines.ContainsKey(SetValueLevel))
            {
                return lines[SetValueLevel];
            }
            ParallelMethodLevelLines obj = new ParallelMethodLevelLines((float l1, float l2, int N1, int N2) => { return null; }, l1, l2,
                N1, N2);
            obj.data = grid;
            obj.Ck = SetValueLevel;
            lines.Add(SetValueLevel, obj.Calculation());
            return lines[SetValueLevel];
        }
        public bool DownloadGrid(string path = "C:\\Users\\Admin\\Documents\\Visual Studio 2015\\Projects\\ClimateData\\Data\\d1-1-temperature.txt")
        {
            bool flag = true;
            System.IO.StreamReader reader = new System.IO.StreamReader(path);
            try
            {
                string str = reader.ReadLine();
                string[] strs = str.Split(',');
                int latmin = int.Parse(strs[0]), latmax = int.Parse(strs[1]), latpoints = int.Parse(strs[2]);
                str = reader.ReadLine(); strs = str.Split(',');
                int lonmin = int.Parse(strs[0]), lonmax = int.Parse(strs[1]), lonpoints = int.Parse(strs[2]);
                grid = new float[lonpoints, latpoints];
                N1 = lonpoints - 1;
                N2 = latpoints - 1;
                l1 = lonmax - lonmin;
                l2 = latmax - latmin;
                for (int i = 0; i < lonpoints; ++i)
                {
                    str = reader.ReadLine();
                    strs = str.Split(',');
                    for (int j = 0; j < latpoints; ++j)
                    {
                        string[] tmp = strs[j].Split('.');
                        strs[j] = tmp[0] + "," + tmp[1];
                        grid[i, j] = float.Parse(strs[j]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение: " + ex.Message);
                flag = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return flag;
        }
    }
}
