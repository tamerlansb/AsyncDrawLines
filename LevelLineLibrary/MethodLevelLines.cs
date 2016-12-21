using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ParrallelPrograming_task1_
{
    [Serializable]
    public struct line{ 
        public float x1, x2, y1, y2;
        public line(point a,point b)
        {
            x1 = a.x;
            y1 = a.y;
            x2 = b.x;
            y2 = b.y;
        }
    }
    [Serializable]
    public struct point
    {
        public float x, y;
        public point(float x,float y)
        {
            this.x = x;
            this.y = y;
        }
        public  static point operator+(point a,point b)
        {
            return new point(a.x + b.x, a.y + b.y);
        }
        public static point operator*(float alpha,point a)
        {
            return new point(alpha * a.x, alpha * a.y);
        }
        public static point operator *(point a, float alpha)
        {
            return new point(alpha * a.x, alpha * a.y);
        }
        public static point operator /(point a, float alpha)
        {
            return new point( a.x / alpha, a.y / alpha);
        }
    }
    [Serializable]
    public class MethodLevelLines
    {
        protected float l1, l2, h1, h2;
        protected int N1, N2;
        public delegate float[,] func(float l1, float l2, int N1, int N2);
        protected MethodLevelLines( float l1, float l2, int N1, int N2)
        {
            this.l1 = l1;
            this.l2 = l2;
            this.N1 = N1;
            this.N2 = N2;
            h1 = l1 / N1;
            h2 = l2 / N2;
        }
        public float[,] data
        {
            get;
            set;
        }
        public float Ck
        {
            set;
            get;
        }
    }
}
