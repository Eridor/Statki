using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Annotations;

namespace SRiR_Project.Model
{
    class GameViewModel : INotifyPropertyChanged
    {
        public GameViewModel()
        {
            MyFields = new Field[100];
            EnemyFields = new Field[100];
            game = GameCore.Instance;
            LoadData();
            conf.Infotext = "Nowa gra! Na początek rozmieść statki";
            conf.InfotextAdd("Dodaj statek 4-masztowy\n");
        }

        public static GameCore game;
        private GlobalConfig conf = GlobalConfig.Instance;
        
        private static string _InfoText;

        private static Field[] _Myfields;
        public Field[] MyFields
        {
            get { return _Myfields; }
            set
            {
                if (!Equals(value, _Myfields))
                {
                    _Myfields = value;
                    OnPropertyChanged();
                }
            }
        }

        private static Field[] _Enemyfields;
        public Field[] EnemyFields
        {
            get { return _Enemyfields; }
            set
            {
                if (!Equals(value, _Enemyfields))
                {
                    _Enemyfields = value;
                    OnPropertyChanged();
                }
            }
        }

        private static Field _SelectedField;
        public static Field SelectedField
        {
            get { return _SelectedField; }
            set
            {
                _SelectedField = value;
                if (!game.EndGame)
                {
                    switch (game.GameFaze)
                    {
                        case 1:
                            //Rozmieszczanie statków
                            if (value.Map == Field.MapType.My && value.Type == Field.FieldType.Unknown)
                            {
                                bool test = game.placeShip(_SelectedField.id, ref _Myfields);
                                if (test)
                                {

                                    if (game.GameFaze == 2 || game.GameFaze == 3)
                                    {
                                        //Wyślij zapytanie że jesteś gotowy
                                        game.SendReady();
                                    }
                                }
                            }
                            else if (value.Type == Field.FieldType.Builded)
                            {
                                game.removeShip(_SelectedField.id, ref _Myfields);
                            }
                            break;
                        case 2:
                            //Twój ruch
                            if (value.Map == Field.MapType.Enemy && game.EnemyReady && game.PlayerReady)
                            {
                                if (value.Type == Field.FieldType.Unknown)
                                {
                                    string ans = game.SendQuestion(value.id);
                                    if (ans == "hit")
                                    {
                                        if (game.getMode() == 0)
                                            game.moves--;
                                        _Enemyfields[value.id].ContentText = "✕";
                                        _Enemyfields[value.id].Background = "Red";
                                    }
                                    else if (ans == "mis")
                                    {
                                        game.moves--;
                                        _Enemyfields[value.id].ContentText = "●";
                                        _Enemyfields[value.id].Background = "DarkGray";
                                    }
                                    else if (ans == "sink")
                                    {
                                        if(game.getMode() == 0)
                                            game.moves--;
                                        _Enemyfields[value.id].ContentText = "✖";
                                        _Enemyfields[value.id].Background = "DarkRed";
                                        game.MarkSinked(value.id, ref _Enemyfields);
                                    }
                                    
                                    if (game.moves == 0)
                                    {
                                        game.GameFaze = 3;
                                        game.SendYourTurn();
                                    }
                                }
                            }
                            break;
                        case 3:
                            //Ruch przeciwnika

                            break;
                        case 4:
                            //Koniec gry

                            break;
                    }
                }
            }
        }

        private void LoadData()
        {
            const int FieldSize = 30;
            const int SpaceBetweenFields = 3;
            const int LeftSeparator = 30;

            for(int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    MyFields[i + 10*j] = new Field()
                    {
                        id = i + 10*j,
                        Map = Field.MapType.My,
                        Enabled = true,
                        X = i,
                        Y = j,
                        Type = Field.FieldType.Unknown,
                        Left = LeftSeparator + ((FieldSize + SpaceBetweenFields) * i) - 210,
                        Top = 0 -(i*FieldSize + j*8*FieldSize + j*(FieldSize - SpaceBetweenFields)),
                        Size = FieldSize,
                        ContentText = (char)(i + 65) + (j + 1).ToString()

                    };
                }
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    EnemyFields[i + 10 * j] = new Field()
                    {
                        id = i + 10 * j,
                        Map = Field.MapType.Enemy,
                        Enabled = true,
                        X = i,
                        Y = j,
                        Type = Field.FieldType.Unknown,
                        Left = LeftSeparator + ((FieldSize + SpaceBetweenFields) * i) - 210,
                        Top = 0 - (i * FieldSize + j * 8 * FieldSize + j * (FieldSize - SpaceBetweenFields)),
                        Size = FieldSize,
                        ContentText = (char)(i + 65) + (j + 1).ToString()

                    };
                }
        }

        public bool CheckShip(int id)
        {
            return game.CheckShipState(id, ref _Myfields);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
