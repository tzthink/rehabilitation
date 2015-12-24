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
    /// WindowSetRehabilitationPlan.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSetRehabilitationPlan : Window
    {
        public List<TwoTextbox> tTList = new List<TwoTextbox>();

        public WindowSetRehabilitationPlan()
        {
            InitializeComponent();
            tTList.Clear();
        }

        private void btnRehabilitationAddPose_Click(object sender, RoutedEventArgs e)
        {
            Class1.counterOfTwoTextbox++;
            int counterOfTwoTextboxM = Class1.counterOfTwoTextbox - 1;

            TwoTextbox twoTextbox = new TwoTextbox();
            //tTList.Add(twoTextbox);

            Thickness thick = new Thickness();
            thick.Left = 45;
            thick.Top = 53 + 33 * Class1.counterOfTwoTextbox;
            thick.Right = 0;
            thick.Bottom = 0;
            twoTextbox.tbNameOfPose.Margin = thick;

            thick.Left = 205;
            twoTextbox.tbTime.Margin = thick;

            tTList.Add(twoTextbox);
            Grid1.Children.Add(tTList[counterOfTwoTextboxM]);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Class1.counterOfTwoTextbox; i++)
            {
                Class1.rT.typeOfPose = tTList[i].tbNameOfPose.Text;
                Class1.rT.timeOfPose = Convert.ToInt32(tTList[i].tbTime.Text);
                Class1.listRT.Add(Class1.rT);
            }
            this.Close();
        }
    }
}
