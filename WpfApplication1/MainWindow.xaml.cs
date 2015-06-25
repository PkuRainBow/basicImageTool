/***
 * Refer video 
 * https://www.youtube.com/watch?v=XYj9byw49K4
 * https://www.youtube.com/watch?v=5k_uYtO10tw
 ***/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        double Scale = 1;
        int Angle = 0;
        double Col = 0, Row = 0;
        int userIndex = 0;
        int modelIndex = 15;
        int CountUser = 0;
        int CountModel = 0;
        string userStr = "";
        string modelStr = "";
        System.IO.DirectoryInfo userDir = new System.IO.DirectoryInfo("E:\\tupian\\markPoint\\userHead\\");
        System.IO.DirectoryInfo modelDir = new System.IO.DirectoryInfo("E:\\tupian\\markPoint\\modelPic\\");
        //System.IO.FileInfo[] userFiles = userDir.GetFiles();
        System.IO.FileInfo[] userFiles;
        System.IO.FileInfo[] modelFiles;

        public MainWindow()
        {
            userFiles = userDir.GetFiles();
            modelFiles = modelDir.GetFiles();
            CountUser = userFiles.Length;
            CountModel = modelFiles.Length;
            //first initialize ...
            InitializeComponent();
            //set and change the modelImage & userHead information ...
            modelStr = @"E:\\tupian\\markPoint\\modelPic\\" + modelFiles[modelIndex].Name;
            modelImage.Source = new BitmapImage(new Uri(modelStr, UriKind.Absolute));
            userStr = @"E:\\tupian\\markPoint\\userHead\\" + userFiles[userIndex].Name;
            userHead.Source = new BitmapImage(new Uri(userStr, UriKind.Absolute));
            //init : set the mouse call function ...
            INIT();
        }

        private Point firstPoint = new Point();

        public void INIT()
        {
            userHead.MouseLeftButtonDown += (ss, ee) =>
            {
                firstPoint = ee.GetPosition(this);
                userHead.CaptureMouse();
            };

            userHead.MouseWheel += (ss, ee) => {
                Matrix mat = userHead.RenderTransform.Value;
                Point mouse = ee.GetPosition(userHead);

                if(ee.RightButton == MouseButtonState.Pressed)
                {
                    //-- Rotate 滚轮朝上旋转
                    if (ee.Delta > 0)
                    {
                        mat.RotateAtPrepend(2, mouse.X, mouse.Y);
                        Angle += 2;
                    }
                    //--　滚轮朝下旋转
                    else
                    {
                        mat.RotateAtPrepend(-2, mouse.X, mouse.Y);
                        Angle -= 2;
                    }
                }
                else
                {
                    //-- Zoom
                    if (ee.Delta > 0)
                    {
                        mat.ScaleAtPrepend(1.05, 1.05, mouse.X, mouse.Y);
                        Scale *= 1.05;
                    }
                    else
                    {
                        mat.ScaleAtPrepend(1 / 1.05, 1 / 1.05, mouse.X, mouse.Y);
                        Scale /= 1.05;
                    }
                }

                MatrixTransform mtf = new MatrixTransform(mat);
                userHead.RenderTransform = mtf;
            };

            userHead.MouseMove += (ss, ee) =>
            {
                if (ee.LeftButton == MouseButtonState.Pressed)
                {
                    //create temp point
                    Point temp = ee.GetPosition(this);
                    Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);
                    //store the bias 
                    Col = Canvas.GetLeft(userHead) - res.X;
                    Row = Canvas.GetTop(userHead) - res.Y;
                    //update image location
                    Canvas.SetLeft(userHead, Canvas.GetLeft(userHead) - res.X);
                    Canvas.SetTop(userHead, Canvas.GetTop(userHead) - res.Y);
                    //update first point
                    firstPoint = temp;
                }
            };

            userHead.MouseUp += (ss, ee) => 
            { 
                userHead.ReleaseMouseCapture(); 
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch(e.Key)
            {
                case Key.Enter:
                    {
                        string lines = userFiles[userIndex].Name+"\t"+modelFiles[modelIndex].Name+"\tScale:\t" + Scale.ToString() + "\tAngle:\t" + Angle.ToString() + "\tRow\t" + Row.ToString() + "\tCol\t" + Col.ToString() + "\n";
                        //reset all the variable
                        Scale = 1; Angle = 0; Row = 0; Col = 0;
                        System.IO.File.AppendAllText(@"E:\tupian\markPoint\UserModel.txt", lines);
                        MessageBox.Show("Store !! ");
                        break;
                    }
                //previous user picture
                case Key.Left:
                    {
                        userIndex = (userIndex - 1 + userIndex) % CountUser;
                        userStr = @"E:\\tupian\\markPoint\\userHead\\" + userFiles[userIndex].Name;
                        userHead.Source = new BitmapImage(new Uri(userStr, UriKind.Absolute));
                        break;
                    }
                //next user picture
                case Key.Right:
                    {
                        userIndex = (userIndex + 1) % CountUser;
                        userStr = @"E:\\tupian\\markPoint\\userHead\\" + userFiles[userIndex].Name;
                        userHead.Source = new BitmapImage(new Uri(userStr, UriKind.Absolute));
                        break;
                    }
                //previous model picture
                case Key.Up:
                    {
                        modelIndex = (modelIndex - 1 + CountModel) % CountModel;
                        modelStr = @"E:\\tupian\\markPoint\\modelPic\\" + modelFiles[modelIndex].Name;
                        modelImage.Source = new BitmapImage(new Uri(modelStr, UriKind.Absolute));
                        break;
                    }
                //next model picture
                case Key.Down:
                    {
                        modelIndex = (modelIndex + 1) % CountModel;
                        modelStr = @"E:\\tupian\\markPoint\\modelPic\\" + modelFiles[modelIndex].Name;
                        modelImage.Source = new BitmapImage(new Uri(modelStr, UriKind.Absolute));                     
                        break;
                    }
            }
        }
    }
}