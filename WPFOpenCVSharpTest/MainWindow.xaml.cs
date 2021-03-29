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

namespace WPFOpenCVSharpTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Win_NewImage> listImage = new List<Win_NewImage>();
        public int _nSelWin;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void bn_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            if(fdlg.ShowDialog() == true)
            {
                int id = listImage.Count;
                //fdlg.FileName
                listImage.Add(new Win_NewImage(this, id));
                listImage[id].fn_OpenImage(fdlg.FileName);
                listImage[id].Show();
            }
        }

        private void bn_Zoom_In(object sender, RoutedEventArgs e)
        {
            double scale = listImage[_nSelWin].fn_GetScale();
            listImage[_nSelWin].fn_Zoom(scale + 0.1);
        }

        private void bn_Zoom_Out(object sender, RoutedEventArgs e)
        {
            double scale = listImage[_nSelWin].fn_GetScale();
            listImage[_nSelWin].fn_Zoom(scale - 0.1);
        }

        private void bn_Zoom_1x(object sender, RoutedEventArgs e)
        {
            listImage[_nSelWin].fn_Zoom(1.0);
        }

        private void bn_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fdlg = new SaveFileDialog();
            if(fdlg.ShowDialog() == true)
            {
                listImage[_nSelWin].fn_SaveImage(fdlg.FileName);
            }
        }

        private void bn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
