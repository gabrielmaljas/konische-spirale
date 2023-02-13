using System;
using System.Configuration;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VMTKonischeSpirale.Points;
using VMTKonischeSpirale.Enums;
using System.Windows.Shapes;
using System.IO;
using VMTKonischeSpirale.Interfaces;

namespace VMTKonischeSpirale
{
    public partial class MainWindow : Window
    {
        private List<Point3D> point3DList { get; set; }
        private Point2D startPoint { get; set; }
        private double startRadius { get; set; }
        private double maxHeight { get; set; }
        private double minRadius { get; set; }
        private double gradient { get; set; }
        private Gradient gradientEnum { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            point3DList= new List<Point3D>();

            GetValuesFromConfig();

            SetFieldsWithConfig();
        }

        private void ExportCsv(object sender, RoutedEventArgs e)
        {
            var outputString = "";
            var time = DateTime.Now.ToString("yyyy-MM-dd");
            var filePath = System.IO.Path.Combine(Path_Input.Text, CsvInput.Text + time + ".csv");
            foreach(var point in point3DList)
            {
                var csvString = point as ICsvFormattable;
                outputString += csvString.ToCsvString(point);
            }

            File.WriteAllText(filePath, outputString);
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            SaveSettings(sender, e);
            GetValuesFromConfig();
            GetGradient();

            canvas.Children.Clear();
            point3DList.Clear();
            
            var radius = startRadius;
            var height = 0.0;
            var angle = 0.0;
            var angleStep = 1.0;
            var deltaRadius = 1.0;

            while (radius > minRadius && height < maxHeight)
            {
                var x = startPoint.X + radius * Math.Cos(angle);
                var y = startPoint.Y + radius * Math.Sin(angle);
                var z = height;

                point3DList.Add(new Point3D(x, y, z));

                angle += angleStep;
                radius -= deltaRadius;
                height += gradient * Math.Sqrt(radius * radius + deltaRadius * deltaRadius);

            }

            foreach (var point in point3DList)
            {
                DrawPoint(point);
            }
        }

        private void GetGradient()
        {
            switch (gradientEnum)
            {
                case Gradient.Procent:
                    {
                        gradient = (gradient / 100) * Math.PI / 2;
                        break;
                    }
                case Gradient.MillimeterPerMeter:
                    {
                        gradient = Math.Atan(gradient / 1000); ;
                        break;
                    }
                case Gradient.Gon:
                    {
                        gradient = (gradient / 400) * Math.PI;
                        break;
                    }
                case Gradient.Degrees:
                    {
                        gradient = (gradient / 100) * Math.PI;
                        break;
                    }
            }
        }

        private void DrawPoint(Point3D point)
        {
            var ellipse = new Ellipse();
            ellipse.Width = 5;
            ellipse.Height = 5;
            ellipse.Fill = System.Windows.Media.Brushes.Black;

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetBottom(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        private void SetFieldsWithConfig()
        {
            X_Input.Text = startPoint.X.ToString(); 
            Y_Input.Text = startPoint.Y.ToString(); 
            StartRadius_Input.Text = startRadius.ToString(); 
            MaxH_Input.Text = maxHeight.ToString(); 
            MinR_Input.Text = minRadius.ToString(); 
            Steigung_Input.Text = gradient.ToString();
            Steigung_Unit.SelectedIndex = (int)gradientEnum;
        }

        private void GetValuesFromConfig()
        {
           
            startPoint = new Point2D
            {
                X = Convert.ToDouble(ConfigurationManager.AppSettings.Get("X")),
                Y = Convert.ToDouble(ConfigurationManager.AppSettings.Get("Y"))
            };

            startRadius = Convert.ToDouble(ConfigurationManager.AppSettings.Get("StartRadius"));

            maxHeight = Convert.ToDouble(ConfigurationManager.AppSettings.Get("MaxH"));

            minRadius = Convert.ToDouble(ConfigurationManager.AppSettings.Get("MinR"));

            gradient = Convert.ToDouble(ConfigurationManager.AppSettings.Get("S"));

            gradientEnum = (Gradient)Enum.ToObject(typeof(Gradient), Convert.ToInt32(ConfigurationManager.AppSettings.Get("SUnit")));
        }

        public void SaveSettings(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["X"].Value = X_Input.Text;
            config.AppSettings.Settings["Y"].Value = Y_Input.Text;
            config.AppSettings.Settings["StartRadius"].Value = StartRadius_Input.Text;
            config.AppSettings.Settings["MaxH"].Value = MaxH_Input.Text;
            config.AppSettings.Settings["MinR"].Value = MinR_Input.Text;
            config.AppSettings.Settings["S"].Value = Steigung_Input.Text;
            config.AppSettings.Settings["SUnit"].Value = Convert.ToInt32((Gradient)Enum.ToObject(typeof(Gradient), Steigung_Unit.SelectedIndex)).ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        
    }
}
