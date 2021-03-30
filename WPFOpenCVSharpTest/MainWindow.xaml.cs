using Microsoft.Win32;
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
using OpenCvSharp;

namespace WPFOpenCVSharpTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        List<Win_NewImage> listImage = new List<Win_NewImage>();
        DateTime timeStart = new DateTime();
        public int _nSelWin = -1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void bn_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Filter = "Image (*.bmp, *.png, *.jpg, *.tiff)|*.bmp; *.png; *.jpg; *.tiff";
            if (fdlg.ShowDialog() == true)
            {
                int id = listImage.Count;
                string strFile = fdlg.FileName;
                string strTitle = strFile.Substring(strFile.LastIndexOf("\\") + 1);
                Mat temp = Cv2.ImRead(strFile);
                listImage.Add(new Win_NewImage(this));
                listImage[id].fn_SetImage(temp, strTitle);
                listImage[id].Show();
                fn_WriteLog($"Image Open : {fdlg.FileName}");
            }
        }

        public void fn_ActivatedWindow(Win_NewImage win)
        {
            for (int i = 0; i < listImage.Count; i++)
            {
                if (listImage[i] == win)
                {
                    _nSelWin = i;
                    break;
                }
            }
        }
        public void fn_DestoryWindow(Win_NewImage win)
        {
            for (int i = 0; i < listImage.Count; i++)
            {
                if (listImage[i] == win)
                {
                    listImage.RemoveAt(i);
                    break;
                }
            }
        }

        private void fn_WriteLog(string strLog)
        {
            listLog.Items.Add($"[{DateTime.Now:HH:mm:ss:fff}] {strLog}");
            listLog.ScrollIntoView(listLog.Items[listLog.Items.Count - 1]);
        }
       

        private void bn_Zoom_In(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                double scale = listImage[_nSelWin].fn_GetScale();
                listImage[_nSelWin].fn_Zoom(scale + 0.1);
            }
        }

        private void bn_Zoom_Out(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                double scale = listImage[_nSelWin].fn_GetScale();
                listImage[_nSelWin].fn_Zoom(scale - 0.1);
            }
        }

        private void bn_Zoom_1x(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                listImage[_nSelWin].fn_Zoom(1.0);
            }
        }

        private void bn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SaveFileDialog fdlg = new SaveFileDialog();
                fdlg.Filter = "Image (*.bmp, *.png, *.jpg, *.tiff)|*.bmp; *.png; *.jpg; *.tiff";
                fdlg.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
                if (fdlg.ShowDialog() == true)
                {
                    Cv2.ImWrite(fdlg.FileName, listImage[_nSelWin].fn_GetImage());
                    fn_WriteLog($"Image Save : {fdlg.FileName}");
                }
            }
        }

        private void bn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bn_GrayScale_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = new Mat();
                timeStart = DateTime.Now;
                Cv2.CvtColor(matSrc, matDst, ColorConversionCodes.BGR2GRAY);
                listImage[_nSelWin].fn_SetImage(matDst);
                fn_WriteLog($"Color(BGR) to Gray ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
            }
        }

        private void bn_Inverse_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = Mat.Zeros(matSrc.Size(), MatType.CV_8UC1);
                timeStart = DateTime.Now;
                unsafe
                {
                    byte* ptr = matSrc.DataPointer;
                    byte* ptrDst = matDst.DataPointer;
                    for(int i = 0; i < matSrc.Width * matSrc.Height; i++)
                    {
                        ptrDst[i] = (byte)(255 - ptr[i]);
                    }
                }
                listImage[_nSelWin].fn_SetImage(matDst);
                fn_WriteLog($"Inverse ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
            }
        }

        private void bn_LogicalSum_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bn_Resize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bn_Rotate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bn_Gaussian_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bn_Canny_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
