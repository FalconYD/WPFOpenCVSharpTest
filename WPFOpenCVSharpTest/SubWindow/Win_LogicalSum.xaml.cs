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
    /// Win_LogicalSum.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_LogicalSum : Window
    {
        public int nMode = 0;
        public int nSrc1 = 0;
        public int nSrc2 = 0;
        public Win_LogicalSum(List<Win_NewImage> list)
        {
            InitializeComponent();

            for (int i = 0; i < list.Count; i++)
            {
                cb_Src1.Items.Add(list[i].Title);
                cb_Src2.Items.Add(list[i].Title);
            }
            cb_Src1.SelectedIndex = 0;
            cb_Src2.SelectedIndex = 1;
        }

        private void radio_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if(rb != null)
            {
                int.TryParse(rb.CommandParameter as string, out nMode);
            }
        }

        private void bn_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void bn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //DialogResult = false;
        }
    }
}
