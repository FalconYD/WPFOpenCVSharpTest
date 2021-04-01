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

namespace WPFOpenCVSharpTest.SubWindow
{
    /// <summary>
    /// Win_Resize.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Rotate : Window
    {
        public Win_Rotate()
        {
            InitializeComponent();
        }

        private void bn_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void bn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
