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
    /// Interaction logic for LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        private Thread CheckPlayers;
        public LobbyWindow()
        {
            InitializeComponent();
            CheckPlayers = new Thread(new ThreadStart(CheckTh));
            StartSearch_Click(null, null);
        }

        GlobalConfig conf = GlobalConfig.Instance;

        private void CheckTh()
        {
            while (true)
            {
                this.Dispatcher.Invoke((Action) (() =>
                {
                    if (conf.OtherUserName != "")
                    {
                        SearchResults.Text = "Znaleziono gracza: " + conf.OtherUserName;
                        StartGame.IsEnabled = true;
                        //ErasePlayer.IsEnabled = true;
                    }
                    else
                    {
                        SearchResults.Text = "Brak nowych graczy";
                        StartGame.IsEnabled = false;
                        ErasePlayer.IsEnabled = false;
                    }
                }));
                Thread.Sleep(1000);
            }
        }
        private void StartSearch_Click(object sender, RoutedEventArgs e)
        {
            conf.InitServer(Convert.ToInt32(PortBox.Text));
            StartSearch.IsEnabled = false;
            StopSearch.IsEnabled = true;
            CheckPlayers.Start();

        }

        private void StopSearch_Click(object sender, RoutedEventArgs e)
        {
            conf.StopServer();
            StartSearch.IsEnabled = true;
            StopSearch.IsEnabled = false;
            CheckPlayers.Abort();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            CheckPlayers.Abort();
            conf.AcceptLobby();
            conf.isHost = true;
            GameWindow w = new GameWindow();
            w.Show();
            this.Close();
        }

        private void ErasePlayer_Click(object sender, RoutedEventArgs e)
        {
            StartGame.IsEnabled = false;
            ErasePlayer.IsEnabled = false;
            conf.DenyLobby();

        }

        private void Mode3_Checked(object sender, RoutedEventArgs e)
        {
            if (Mode1n != null)
                Mode1n.IsChecked = false;
            conf.mode = 0;
        }

        private void Mode1n_Checked(object sender, RoutedEventArgs e)
        {
            if (Mode3 != null)
                Mode3.IsChecked = false;
            conf.mode = 1;
        }
    }
}
