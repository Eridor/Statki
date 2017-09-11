using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SRiR_Project.Model;

namespace SRiR_Project.View
{
    /// <summary>
    /// Interaction logic for JoinWindow.xaml
    /// </summary>
    public partial class JoinWindow : Window
    {
        GlobalConfig conf = GlobalConfig.Instance;
        public JoinWindow()
        {
            InitializeComponent();
        }

        private Thread th;

        private void JoingGame_Click(object sender, RoutedEventArgs e)
        {

            if (conf.JoinSerwer(IpAddresBox.Text, PortBox.Text))
            {
                th = new Thread(new ThreadStart(thr));
              
                th.Start();
            }
        }

        

        public void thr()
        {
            for (int i = 30; i >= 0; i--)
            {
                int ans = conf.CheckDecision();
                if (ans == 1)
                {
                    //Start game
                    this.Dispatcher.Invoke((Action) (() =>
                    {
                        Start.IsEnabled = true;
                        InfoText.Text = "Gotowe! Wejdź do gry!";
                        InfoText.Foreground = new SolidColorBrush(Colors.Red);
                        JoingGame.IsEnabled = false;
                    }));
                    break;
                }
                else if (ans == 0 || i == 0)
                {
                    MessageBox.Show("Zaproszenie odrzucone");
                    
                }
                this.Dispatcher.Invoke((Action) (() =>
                {
                    InfoText.Text = "Wygaśnięcie zaproszenia za: " + i + "s.";
                }));
                Thread.Sleep(1000);

            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            GameWindow w = new GameWindow();
            conf.isHost = false;
            w.Show();
            this.Close();
        }
        
    }
}
