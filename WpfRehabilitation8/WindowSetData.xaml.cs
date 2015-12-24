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
using Microsoft.Kinect;
using System.IO;

namespace WpfRehabilitation8
{
    /// <summary>
    /// WindowSetData.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSetData : Window
    {
        private int counterOfAngle = 0;
        private int counterOfPose = 0;

        //FiveTextbox[] ft = new FiveTextbox[8];

        List<FiveTextbox> fTList = new List<FiveTextbox>();

        public WindowSetData()
        {
            InitializeComponent();
        }

        private void btnAddAngle_Click(object sender, RoutedEventArgs e)
        {
            counterOfAngle++;
            int counterOfAngleM = counterOfAngle - 1;
            FiveTextbox ft = new FiveTextbox();
            fTList.Add(ft);

            //ft.Name = "ft" + counter.ToString();
            double topValue = 93 + 33 * counterOfAngle;
            Thickness tValue = new Thickness();

            tValue.Top = topValue;
            tValue.Bottom = 0;
            tValue.Right = 0;
            //
            tValue.Left = 10;
            fTList[counterOfAngleM].tb1.Margin = tValue;
            //
            tValue.Left = 110;
            fTList[counterOfAngleM].tb2.Margin = tValue;
            //
            tValue.Left = 210;
            fTList[counterOfAngleM].tb3.Margin = tValue;
            //
            tValue.Left = 310;
            fTList[counterOfAngleM].tb4.Margin = tValue;
            //
            tValue.Left = 410;
            fTList[counterOfAngleM].tb5.Margin = tValue;

            MyGrid.Children.Add(fTList[counterOfAngleM]);
        }

        private void btnRemoveAngle_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.Children.Remove(fTList[--counterOfAngle]);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string textTotal = counterOfAngle.ToString() + "#" + txtNameOfPose.Text + "%";
            for (int i = 0; i < counterOfAngle; i++)
            {
                string text1 = fTList[i].tb1.Text;
                string text2 = fTList[i].tb2.Text;
                string text3 = fTList[i].tb3.Text;
                string text4 = fTList[i].tb4.Text;
                string text5 = fTList[i].tb5.Text;
                string text = text1 + "$" + text2 + "~" + text3 + "|" + text4 + "@" + text5 + "/";
                textTotal = textTotal + text;
            }
            textTotal = textTotal + ")";
            File.AppendAllText("e:\\pose.txt", textTotal, Encoding.ASCII);
            counterOfAngle = 0;
            counterOfPose++;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtNameOfPose.Text = "";
            for (int i = 0; i < counterOfAngle; i++)
            {
                fTList[i].tb1.Text = "";
                fTList[i].tb2.Text = "";
                fTList[i].tb3.Text = "";
                fTList[i].tb4.Text = "";
                fTList[i].tb5.Text = "";
            }
        }
    }
}
