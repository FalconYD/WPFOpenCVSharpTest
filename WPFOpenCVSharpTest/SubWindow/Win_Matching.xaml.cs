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
    /// Win_Matching.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Matching : Window
    {
        public Win_Matching(List<Win_NewImage> list)
        {
            InitializeComponent();

            for (int i = 0; i < list.Count; i++)
            {
                cb_Src .Items.Add(list[i].Title);
                cb_Tmpl.Items.Add(list[i].Title);
            }
            cb_Src .SelectedIndex = 0;
            cb_Tmpl.SelectedIndex = 1;
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
