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

namespace WpfRehabilitation8
{
    /// <summary>
    /// WindowPatientRehabilitation.xaml 的交互逻辑
    /// </summary>
    public partial class WindowPatientRehabilitation : Window
    {
        public WindowPatientRehabilitation()
        {
            InitializeComponent();
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            WindowSetRehabilitationPlan wsd = new WindowSetRehabilitationPlan();
            wsd.Show();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            WindowStartRehabilitationPlan wsrp = new WindowStartRehabilitationPlan();
            wsrp.Show();
        }
    }
}
