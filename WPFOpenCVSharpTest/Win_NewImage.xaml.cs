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
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace WPFOpenCVSharpTest
{
    /// <summary>
    /// Win_NewImage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_NewImage : System.Windows.Window
    {
        public Win_NewImage(System.Windows.Window win)
        {
            InitializeComponent();
            this.Owner = win;
        }
        
        public void fn_SetImage(Mat mat, string strTitle = "Image_")
        {
            this.Title = strTitle;
            ImageBrush ib = new ImageBrush(WriteableBitmapConverter.ToWriteableBitmap(mat));
            ib.Stretch = Stretch.None;

            canvas.Width = ib.ImageSource.Width;
            canvas.Height = ib.ImageSource.Height;
            canvas.Background = ib;
        }

        public Mat fn_GetImage()
        {
            Mat matRtn = null;
            ImageBrush ib = canvas.Background as ImageBrush;
            if (ib != null)
            {
                WriteableBitmap wb = ib.ImageSource as WriteableBitmap;
                if (wb != null)
                {
                    matRtn = WriteableBitmapConverter.ToMat(wb);
                }
            }
            return matRtn;
        }

        public void fn_Zoom(double zoom)
        {
            myScaleTransform.ScaleX = zoom;
            myScaleTransform.ScaleY = zoom;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ((MainWindow)Owner).fn_ActivatedWindow(this);
        }

        public double fn_GetScale()
        {
            return myScaleTransform.ScaleX;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainWindow)Owner).fn_DestoryWindow(this);
        }
    }
}
