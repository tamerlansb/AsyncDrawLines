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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;

namespace AsyncDrawLines.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region fields
        private Model obj = new Model();
        private ImageSource renderedImage;
        private double w, h;
        private DataContext db;
        private DelegateCommand _save, _download;
        private DelegateCommand _clear, _addCommand;
        #endregion

        #region ctor
        public MainViewModel(double w, double h)
        {
            this.w = w;
            this.h = h;
            db = new DataContext();
            DataToSave = new Data();
            DataToDownload = new Data();
            obj.CalcLevelLine(-20);
            obj.CalcLevelLine(-10);
            obj.CalcLevelLine(0);
            obj.CalcLevelLine(10);
            obj.CalcLevelLine(25);
        }
        #endregion

        #region commands
   
        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null) _addCommand = new DelegateCommand(()=>
                {
                    obj.CalcLevelLine(LevelAdd);
                    Update();
                });
                return _addCommand;
            }
        }
        public ICommand ClearCommand
        {
            get
            {
                if (_clear == null) _clear = new DelegateCommand(() => {
                    imageProp = null;
                    obj = new Model();
                });
                return _clear;
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                if (_save == null) _save = new DelegateCommand(Save);
                return _save;
            }
        }
        public ICommand DownloadCommand
        {
            get
            {
                if (_download == null) _download = new DelegateCommand(Download);
                return _download;
            }
        }
        #endregion

        #region Propeties
        public Data DataToSave { get; set; }

        public Data DataToDownload { get; set; }
        public int LevelAdd { get; set; }
        public ImageSource imageProp
        {
            get
            {
                if (renderedImage == null)
                {
                    renderedImage = new DrawingImage();
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

        private void Update()
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            InitializeDrawing(drawingGroup, w, h);
            foreach (var l in obj.Lines)
            {
                DrawLines(w, h, (int)l.Key, drawingGroup);
            }
            imageProp = new DrawingImage(drawingGroup);
        }
        private void Download()
        {
            try
            {
                var data = (from elem in db.DataS
                       where elem.keyword == DataToDownload.keyword ||elem.date == DataToDownload.date 
                       select elem).FirstOrDefault();
            
                MemoryStream ms = new MemoryStream(data.blob);
                BinaryFormatter reader = new BinaryFormatter();
                var model = reader.Deserialize(ms) as Model;
                if (model != null)
                {
                    MessageBox.Show("Great");
                    obj = model;
                    Update();
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Save()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter writer = new BinaryFormatter();
            writer.Serialize(ms, obj);
            db.DataS.Add(new Data()
            {
                ID = 1,
                blob = ms.ToArray(),
                keyword = DataToSave.keyword,
                date = DataToSave.date
            });
            db.SaveChanges();
        }

        private void DrawLines(double w, double h, int level, DrawingGroup drawingGroup)
        {
            List<line> lines = obj.CalcLevelLine(level);
            System.Windows.Media.Brush Col;
            if (level > 25)
                Col = Brushes.Yellow;
            else if (level > 15)
                Col = Brushes.Orange;
            else if (level > 10)
                Col = Brushes.Red;
            else if (level > 0)
                Col = Brushes.Blue;
            else if (level > -5)
                Col = Brushes.Aqua;
            else
                Col = Brushes.White;
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

        private void InitializeDrawing(DrawingGroup drawingGroup, double w, double h)
        {
            GeometryGroup geometryGroup = new GeometryGroup();
            PathGeometry pg = new PathGeometry();
            LineGeometry line = new LineGeometry();
            line.StartPoint = new Point(0, 0);
            line.EndPoint = new Point(w, h);
            geometryGroup.Children.Add(line);
            geometryGroup.Children.Add(pg);
            GeometryDrawing geometryDrawing = new GeometryDrawing();
            geometryDrawing.Geometry = geometryGroup;
            geometryDrawing.Pen = new Pen(new SolidColorBrush(), 1);
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
                InitializeDrawing(drawingGroup, w, h);
                foreach (var level in obj.Lines )
                    DrawLines(w, h, (int)level.Key , drawingGroup);
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
            double w = e.NewSize.Width - 100;
            double h = e.NewSize.Height;
            EnqueueRenderRequest(w,h);
        }
    }
}
