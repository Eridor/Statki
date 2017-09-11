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
using SRiR_Project.Model;
using SRiR_Project.View;

namespace SRiR_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GlobalConfig conf = GlobalConfig.Instance;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateServer_Click(object sender, RoutedEventArgs e)
        {
            conf.Username = NicknameBox.Text;
            conf.AppType = GlobalConfig.Type.Server;
            LobbyWindow wind = new LobbyWindow();
            wind.Show();
            this.Close();
        }

        private void JoinServer_Click(object sender, RoutedEventArgs e)
        {
            conf.Username = NicknameBox.Text;
            conf.AppType = GlobalConfig.Type.Client;
            
            JoinWindow wind = new JoinWindow();
            wind.Show();
            this.Close();
        }
    }
}
