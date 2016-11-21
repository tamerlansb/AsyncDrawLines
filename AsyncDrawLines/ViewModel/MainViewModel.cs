using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ParrallelPrograming_task1_;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace AsyncDrawLines.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Model obj = new Model();
        private DrawingGroup drawingGroup = new DrawingGroup();
        private DrawingImage renderedImage;
        private double w, h;
        public MainViewModel(double w, double h)
        {
            this.w = w;
            this.h = h;
            DrawLines(w, h, System.Windows.Media.Brushes.Violet, 0);
            DrawLines(w, h, System.Windows.Media.Brushes.Red, 10);
            DrawLines(w, h, System.Windows.Media.Brushes.Yellow, 25);
        }

        #region Propeties
        public DrawingImage imageProp
        {
            get
            {
                if (renderedImage == null)
                {
                    renderedImage = new DrawingImage(drawingGroup);
                }
                return renderedImage;
            }
            set
            {
                renderedImage = value;
                NotifityPropetyChanged("imageProp");
            }
        }
        public double W
        {
            get { return w; }
            set
            {
                w = value;
                NotifityPropetyChanged("W");
            }
        }
        public double H
        {
            get { return h; }
            set
            {
                h = value;
                NotifityPropetyChanged("H");
            }
        }
        #endregion

        public void resize(object sender, SizeChangedEventArgs e)
        {

            W = e.NewSize.Width; H = e.NewSize.Height;
            drawingGroup = new DrawingGroup();
            DrawLines(w, h, System.Windows.Media.Brushes.Black, -10);
            DrawLines( w, h, System.Windows.Media.Brushes.Violet, 0);
            DrawLines( w, h, System.Windows.Media.Brushes.Red, 10);
            DrawLines( w, h, System.Windows.Media.Brushes.Yellow, 25);
            H = 17* e.NewSize.Height/18;
            imageProp = new DrawingImage(drawingGroup);
        }
        private void DrawLines( double w, double h, System.Windows.Media.Brush Col, int level)
        {
            Model obj = new Model();
            List<line> lines = obj.CalcLevelLine(level);

            PathGeometry pg = new PathGeometry();
            GeometryGroup geometryGroup = new GeometryGroup();
            for (int i = 0; i < lines.Count; ++i)
            {
                LineGeometry line = new LineGeometry();
                line.StartPoint = new Point(w -lines[i].x1 * w, lines[i].y1 * (17*h/18)+ h/18);
                line.EndPoint = new Point(w - lines[i].x2 * w, lines[i].y2 * (17*h/18)+ h/18);
                geometryGroup.Children.Add(line);
            }
            geometryGroup.Children.Add(pg);
            GeometryDrawing geometryDrawing = new GeometryDrawing();
            geometryDrawing.Geometry = geometryGroup;
            geometryDrawing.Pen = new Pen(Col, 1);
            drawingGroup.Children.Add(geometryDrawing);
        }

        private void DrawLines(double w, double h, System.Windows.Media.Brush Col, int level, DrawingGroup drawingGroup)
        {
            List<line> lines = obj.CalcLevelLine(level);

            PathGeometry pg = new PathGeometry();
            GeometryGroup geometryGroup = new GeometryGroup();
            for (int i = 0; i < lines.Count; ++i)
            {
                LineGeometry line = new LineGeometry();
                line.StartPoint = new Point(w - lines[i].x1 * w, lines[i].y1 * (17 * h / 18) + h / 18);
                line.EndPoint = new Point(w - lines[i].x2 * w, lines[i].y2 * (17 * h / 18) + h / 18);
                geometryGroup.Children.Add(line);
            }
            geometryGroup.Children.Add(pg);
            GeometryDrawing geometryDrawing = new GeometryDrawing();
            geometryDrawing.Geometry = geometryGroup;
            geometryDrawing.Pen = new Pen(Col, 1);
            drawingGroup.Children.Add(geometryDrawing);
        }
        /// <summary>Размер изображения, ожидающего перерисовку или -1, если в очереди запросов нет</summary>
        private double pendingSizeW = -1, pendingSizeH = -1;

        /// <summary>Работает ли в данный момент поток рисования фрактала</summary>
        private bool isRendering = false;

        private async void BeginRendering(double width, double height)
        {
            isRendering = true;
            var rt = Task<DrawingImage>.Factory.StartNew(() =>
            {
                W = width; H = height;
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawLines(w, h, System.Windows.Media.Brushes.White, -20, drawingGroup);
                DrawLines(w, h, System.Windows.Media.Brushes.Blue, -10,drawingGroup);
                DrawLines(w, h, System.Windows.Media.Brushes.Violet, 0, drawingGroup);
                DrawLines(w, h, System.Windows.Media.Brushes.Red, 10,drawingGroup);
                DrawLines(w, h, System.Windows.Media.Brushes.Yellow, 25, drawingGroup);
                H = 17 * height / 18;
                DrawingImage dr = new DrawingImage(drawingGroup);
                dr.Freeze();
                return dr;
            });

            imageProp = await rt;
            isRendering = false;
            if (pendingSizeW != -1 && pendingSizeH !=-1)
            {
                BeginRendering(pendingSizeW,pendingSizeH);
                pendingSizeH = -1;
                pendingSizeW = -1;
            }
        }
        private void EnqueueRenderRequest(double w,double h)
        {
            if (isRendering)
            {
                pendingSizeW = w;
                pendingSizeH = h;
            }
            else
                BeginRendering(w,h);
        }

        public void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double w = e.NewSize.Width;
            double h = e.NewSize.Height;
            EnqueueRenderRequest(w,h);

            
            Image tmp = new Image();
            tmp.Height = h;
            tmp.Width = w;
            tmp.Source = renderedImage;
            //tmp.Save("C:\\Users\\Admin\\Documents\\Visual Studio 2015\\Projects\\ClimateData\\ClimateData\\1.jpg");
            NotifityPropetyChanged("imageProp");
            
        }
    }
}
