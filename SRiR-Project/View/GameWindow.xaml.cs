using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
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

namespace SRiR_Project
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private GlobalConfig conf = GlobalConfig.Instance;
        private GameViewModel GameModel;
        private GameCore _game = GameCore.Instance;

        
        private bool _terminateGame;
        private Thread thInfotext;
        public GameWindow()
        {
            Closing += OnWindowClosing;
            _terminateGame = false;
            GameModel = new GameViewModel();
            this.DataContext = GameModel;
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            UserInfo.Text = GenerateInfoText();
            thInfotext = new Thread(new ThreadStart(loop));
            thInfotext.Start();
        }

        private void loop()
        {
            bool gameready = false;
            while(!_terminateGame)
            {
                
                Dispatcher.BeginInvoke(new Action(() => {
                    GameInfo.Text = conf.Infotext;
                    MoveCount.Text = _game.moves.ToString();
                    if (!gameready)
                    {
                        AvaibleShips.Text = conf.Shiptext;
                        if (conf.Shiptext == "")
                        {
                            gameready = true;
                            AvaibleShipsLabel.Content = "";
                        }
                    }
                    
                }));
                if (_game.PlayerReady && conf.PendingMessages())
                {
                    string msg = conf.GetMessage();
                    if (msg == "ready")
                    {
                        _game.EnemyReady = true;
                        conf.Infotext = "START GRY";
                        if (conf.isHost)
                        {
                            if (conf.mode == 0)
                            {
                                conf.InfotextAdd("Tryb trzech ruchów");
                            }
                            else if (conf.mode == 1)
                            {
                                conf.InfotextAdd("Tryb jednego ruchu i nagrody za trafienie");
                            }
                            conf.InfotextAdd("Zaczynasz grę - masz " + _game.moves +" strzały");

                        }
                        else
                        {
                            if (conf.mode == 0)
                            {
                                conf.InfotextAdd("Tryb trzech ruchów");
                            }
                            else if (conf.mode == 1)
                            {
                                conf.InfotextAdd("Tryb jednego ruchu i nagrody za trafienie");
                            }
                            conf.InfotextAdd("Przeciwnik atakuje - poczekaj na swoją turę");
                        }
                    }
                    else if (msg.Substring(0, 1) == "a")
                    {
                        int attackedId = Convert.ToInt32(msg.Substring(1));
                        if (GameModel.MyFields[attackedId].Type == Field.FieldType.Unknown)
                        {
                            GameModel.MyFields[attackedId].ContentText = "●";
                            GameModel.MyFields[attackedId].Background = "DarkGray";
                            conf.SendMessage("mis");
                            conf.InfotextAdd("Strzał: " + attackedId + ", pudło");
                        }
                        else if (GameModel.MyFields[attackedId].Type == Field.FieldType.Ship)
                        {
                            GameModel.MyFields[attackedId].Background = "Red";
                            if (GameModel.CheckShip(attackedId))
                            {
                                GameModel.MyFields[attackedId].ContentText = "✖";
                                conf.SendMessage("sink");
                                conf.InfotextAdd("Strzał: " + attackedId + ", trafiony zatopiony");
                                _game.ShipsOnMap--;
                                if (_game.ShipsOnMap == 0)
                                {
                                    conf.SendMessage("YouWin");
                                    conf.InfotextAdd("KONIEC GRY - przegrałeś");
                                    _game.GameFaze = 4;
                                    _game.EndGame = true;
                                }
                            }
                            else
                            {
                                GameModel.MyFields[attackedId].ContentText = "✕";
                                conf.SendMessage("hit");
                                conf.InfotextAdd("Strzał: " + attackedId + ", trafiony");
                            }
                        }

                    }
                    else if (msg == "go")
                    {
                        _game.GameFaze = 2;
                        if (conf.mode == 0)
                        {
                            _game.moves = 3;
                        }
                        else if (conf.mode == 1)
                        {
                            _game.moves = 1;
                        }
                        conf.InfotextAdd("Twój ruch!\n");
                    }
                    else if (msg == "YouWin")
                    {
                        conf.InfotextAdd("KONIEC GRY - wygrałeś!");
                        _game.SendNotHitted(GameModel.MyFields);
                        _game.GameFaze = 4;
                        _game.EndGame = true;
                    }
                    else if (msg.Substring(0, 1) == "m" && _game.GameFaze == 4)
                    {
                        string fieldID = "";
                        foreach (char item in msg)
                        {
                            if (item == 'm' && fieldID != "")
                            {
                                GameModel.EnemyFields[Convert.ToInt32(fieldID)].Background = "Green";
                                fieldID = "";
                            }
                            else if (item == 'm')
                            {
                                //pierwsze m
                            }
                            else
                            {
                                fieldID += item;
                            }

                        }
                    }

                }
                Thread.Sleep(100);
            }
        }

        private string GenerateInfoText()
        {
            string ret = "Witaj " + conf.Username + "!\n";
            ret += "Grasz przeciwko: " + conf.OtherUserName + "\n";

            return ret;
        }

        

        private void MyClick(object sender, RoutedEventArgs e)
        {
            var id = ((Button)sender).Tag;
            MessageBox.Show(id.ToString());
            
        }
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _terminateGame = true;
        }
    }
}
