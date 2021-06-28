using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ServiceKolkoKrzyzyk;

namespace KolkoKrzyzyk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly GraKolkoKrzyzykClient clnt;

        List<Button> blist;

        char symbolGracz, symbolServer;

        public MainWindow()
        {
            InitializeComponent();

            clnt = new GraKolkoKrzyzykClient();
            blist = new List<Button>(){ button_1_1, button_1_2, button_1_3, button_2_1, button_2_2, button_2_3, button_3_1, button_3_2, button_3_3 };
        }

        private void LockOut()
        {
            foreach (Button btn in blist)
            {
                btn.IsEnabled = false;
            }
            radio_tic.IsEnabled = true;
            radio_tac.IsEnabled = true;
        }

        private void Button_start_Click(object sender, RoutedEventArgs e)
        {
            foreach (Button btn in blist)
            {
                btn.Content = " ";
                btn.IsEnabled = true;
            }

            if (radio_tic.IsChecked == true)
            {
                clnt.Start(0);
                symbolGracz = 'O';
                symbolServer = 'X';
            } else
            {
                int nom = clnt.Start(1);
                symbolGracz = 'X';
                symbolServer = 'O';
                blist[nom].IsEnabled = false;
                blist[nom].Content = symbolServer;

            }

            radio_tic.IsEnabled = false;
            radio_tac.IsEnabled = false;

            button_start.Content = "RESTART";
            label_status.Content = "Status: Wykonaj ruch naciskając pole";
        }

        private void Buttons_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int index = blist.IndexOf(btn);
            btn.IsEnabled = false;
            btn.Content = symbolGracz;

            int crow = index / 3;
            int ckol = index % 3;

            Boolean tf = clnt.WykonajRuch(crow, ckol, out int srow, out int scol);

            if(tf && srow >= 0)
            {
                int indexu = srow * 3 + scol;
                blist[indexu].IsEnabled = false;
                blist[indexu].Content = symbolServer;
            } else if(!tf)
            {
                int indexu = srow * 3 + scol;
                blist[indexu].IsEnabled = false;
                blist[indexu].Content = symbolServer;
                label_status.Content = "Status: Zwycięstwo " + symbolServer + "!";
                LockOut();
            } else
            {
                switch(scol)
                {
                    case 0:
                        label_status.Content = "Status: Wystąpił remis!";
                        LockOut();
                        break;
                    case 1:
                        label_status.Content = "Status: Zwyciestwo "+symbolGracz+"!";
                        LockOut();
                        break;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
