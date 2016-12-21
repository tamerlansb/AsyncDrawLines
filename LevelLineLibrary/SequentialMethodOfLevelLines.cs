using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParrallelPrograming_task1_
{
    [Serializable]
    public class SequentialMethodOfLevelLines : MethodLevelLines, ICalculation
    {
        public SequentialMethodOfLevelLines(func f, float l1 = 1, float l2 = 1, int N1 = 100,int N2 = 100) : base(l1,l2,N1,N2)
        {
            data = f(l1, l2, N1, N2);
        }
        public List<line> Calculation()
        {
            List<line> lines = new List<line>();
            for (int i = 0; i < N1 ; ++i) 
                for (int  j = 0; j < N2 ;++j)
                {
                    point p0 = new point(i * h1, j * h2) * (data[i + 1, j] - Ck) / (data[i + 1, j] - data[i, j]) + new point((i + 1) * h1, j * h2) * (Ck - data[i, j]) / (data[i + 1, j] - data[i, j]),
                          p1 = new point(i * h1, j * h2) * (data[i, j + 1] - Ck) / (data[i, j + 1] - data[i, j]) + new point(i * h1, (j + 1) * h2) * (Ck - data[i, j]) / (data[i , j + 1] - data[i, j]),
                          p2 = new point(i * h1, (j + 1) * h2) * (data[i + 1, j + 1] - Ck) / (data[i + 1, j + 1] - data[i, j+ 1]) + new point((i + 1) * h1, (j + 1) * h2) * (Ck - data[i, j + 1]) / (data[i + 1, j + 1] - data[i, j + 1]),
                          p3 = new point((i +1)* h1, j * h2) * (data[i + 1, j + 1] - Ck) / (data[i + 1, j + 1] - data[i + 1, j]) + new point((i + 1)* h1, (j + 1) * h2) * (Ck - data[i + 1, j]) / (data[i + 1, j + 1] - data[i + 1, j]);
                    if (data[i,j] >= Ck && data[i+1, j] >= Ck && data[i, j+1] >= Ck && data[i+1, j+1] >= Ck  ||
                        data[i, j] < Ck && data[i + 1, j] < Ck && data[i, j + 1] < Ck && data[i + 1, j + 1] < Ck)
                    {
                        //линии уровня нет
                    } else if (data[i, j] < Ck && data[i + 1, j] >= Ck && data[i, j + 1] >= Ck && data[i + 1, j + 1] >= Ck ||
                        data[i, j] >= Ck && data[i + 1, j] < Ck && data[i, j + 1] < Ck && data[i + 1, j + 1] < Ck)
                    { 
                        lines.Add(new line(p0, p1));
                    } else if (data[i, j] >= Ck && data[i + 1, j] < Ck && data[i, j + 1] >= Ck && data[i + 1, j + 1] >= Ck ||
                        data[i, j] < Ck && data[i + 1, j] >= Ck && data[i, j + 1] < Ck && data[i + 1, j + 1] < Ck)
                    {
                       lines.Add(new line(p0, p3));
                    } else if (data[i, j] >= Ck && data[i + 1, j] >= Ck && data[i, j + 1] < Ck && data[i + 1, j + 1] >= Ck ||
                        data[i, j] < Ck && data[i + 1, j] < Ck && data[i, j + 1] >= Ck && data[i + 1, j + 1] < Ck)
                    {
                       lines.Add(new line(p1, p2));
                    } else  if (data[i, j] >= Ck && data[i + 1, j] >= Ck && data[i, j + 1] >= Ck && data[i + 1, j + 1] < Ck ||
                        data[i, j] < Ck && data[i + 1, j] < Ck && data[i, j + 1] < Ck && data[i + 1, j + 1] >= Ck)
                    {
                       lines.Add(new line(p2, p3));
                    } else if (data[i, j] >= Ck && data[i + 1, j] >= Ck && data[i, j + 1] < Ck && data[i + 1, j + 1] < Ck ||
                        data[i, j] < Ck && data[i + 1, j] < Ck && data[i, j + 1] >= Ck && data[i + 1, j + 1] >= Ck)
                    {
                         lines.Add(new line(p1,p3));
                    } else if (data[i, j] >= Ck && data[i + 1, j] < Ck && data[i, j + 1] >= Ck && data[i + 1, j + 1] < Ck ||
                               data[i, j] < Ck && data[i + 1, j] >= Ck && data[i, j + 1] < Ck && data[i + 1, j + 1] >= Ck)
                    {
                        lines.Add(new line(p0, p2));
                    } else if (data[i, j] >= Ck && data[i + 1, j] < Ck && data[i, j + 1] < Ck && data[i + 1, j + 1] >= Ck )
                    {
                         float alpha = ((i + 1) * h1 / 2 - i * h1) / h1,
                             beta = ((j + 1) * h2 / 2 - j * h2) / h2,
                        temp = (1 - alpha) * (1 - beta) * data[i, j] + (1 - alpha) * beta * data[i, j + 1] + alpha * (1 - beta) * data[i + 1, j] + alpha * beta * data[i, j];
                        if (temp>=Ck)
                        {
                            lines.Add(new line(p0, p3));
                            lines.Add(new line(p1, p2));
                        } 
                        else
                        {
                            lines.Add(new line(p0, p1));
                            lines.Add(new line(p2, p3));
                        }
                    } else if (data[i, j] < Ck && data[i + 1, j] >= Ck && data[i, j + 1] >= Ck && data[i + 1, j + 1] < Ck)
                    {
                        float alpha = ((i + 1) * h1 / 2 - i * h1) / h1,
                             beta = ((j + 1) * h2 / 2 - j * h2) / h2,
                        temp = (1 - alpha) * (1 - beta) * data[i, j] + (1 - alpha) * beta * data[i, j + 1] + alpha * (1 - beta) * data[i + 1, j] + alpha * beta * data[i, j];
                        if (temp < Ck)
                        {
                            lines.Add(new line(p0, p3));
                            lines.Add(new line(p1, p2));
                        }
                        else
                        {
                            lines.Add(new line(p0, p1));
                            lines.Add(new line(p2, p3));
                        }
                    }
                }
            return lines;
        }
    }
}
