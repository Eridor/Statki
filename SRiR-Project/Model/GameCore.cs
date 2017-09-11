using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRiR_Project.Model
{
    class GameCore 
    {
        private static GameCore _instance;
        public static GameCore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameCore();
                }
                return _instance;

            }
        }

        public GameCore()
        {
            GameFaze = 1;
            EnemyReady = false;
            PlayerReady = false;
            EndGame = false;
            ShipsOnMap = 0;
            mapShip = new Ship[10]
            {
                new Ship(4), new Ship(3), new Ship(3), new Ship(2), new Ship(2), new Ship(2), new Ship(1), new Ship(1),
                new Ship(1), new Ship(1)
            };
            moves = 0;
        }

        private GlobalConfig conf = GlobalConfig.Instance;

        public int GameFaze;
        public bool EnemyReady, PlayerReady, EndGame;
        public int ShipsOnMap;
        public Ship[] mapShip;

        public bool placeShip(int id, ref Field[] fields)
        {
            if (ShipsOnMap < 10)
            {
                List<int> collisions = FindCollisions(id, fields);
                bool shipintegrity = false;
                foreach (int item in collisions)
                {
                    if (mapShip[ShipsOnMap].CheckAssign(item))
                    {
                        shipintegrity = true;
                    }
                    else
                    {
                        conf.InfotextAdd("Błąd! Kolizja z innym statkiem");
                        return false; //kolizja z innym statkiem
                    }
                }
                if (mapShip[ShipsOnMap].FieldsDone == 0)
                {
                    mapShip[ShipsOnMap].AddCord(id);
                    conf.InfotextAdd("Pole zaznaczone: " + id);
                    fields[id].Background = "Lime";
                    if (mapShip[ShipsOnMap].shipState == Ship.State.Alive)
                    {
                        ShipsOnMap++;
                        string txt = "";
                        for (int i = ShipsOnMap; i < 10; i++)
                        {
                            foreach (int item in mapShip[i].coords)
                            {
                                txt += "*";
                            }
                            txt += "\n";
                        }
                        conf.Shiptext = txt;
                        conf.InfotextAdd("Statek nr " + ShipsOnMap + " gotowy!");
                        fields[id].Background = "Green";

                        if (ShipsOnMap == 10)
                        {
                            
                            if (conf.isHost)
                            {
                                if (conf.mode == 0)
                                {
                                    conf.InfotextAdd("Tryb trzech ruchów");
                                    moves = 3;
                                }
                                else if (conf.mode == 1)
                                {
                                    conf.InfotextAdd("Tryb jednego ruchu i nagrody za trafienie");
                                    moves = 1;
                                }
                                GameFaze = 2;
                            }
                            else
                            {
                                if (conf.mode == 0)
                                {
                                    conf.InfotextAdd("Tryb trzech ruchów\n\n");
                                }
                                else if (conf.mode == 1)
                                {
                                    conf.InfotextAdd("Tryb jednego ruchu i nagrody za trafienie\n\n");
                                }
                                moves = 0;
                                GameFaze = 3;
                            }
                            PlayerReady = true;
                            conf.InfotextAdd("Poczekaj na przeciwnika....");
                        }
                        else
                        {
                            conf.InfotextAdd("Dodaj statek 1-masztowy\n");
                        }
                    }
                    return true;
                }
                else
                {
                    if (shipintegrity)
                    {
                        mapShip[ShipsOnMap].AddCord(id);

                        fields[id].Background = "Lime";
                        conf.InfotextAdd("Pole zaznaczone: " + id);
                        if (mapShip[ShipsOnMap].shipState == Ship.State.Alive)
                        {
                            foreach (int item in mapShip[ShipsOnMap].coords)
                            {
                                fields[item].Background = "Green";
                            }
                            ShipsOnMap++;
                            string txt = "";
                            for (int i = ShipsOnMap; i < 10; i++)
                            {
                                foreach (int item in mapShip[i].coords)
                                {
                                    txt += "*";
                                }
                                txt += "\n";
                            }
                            conf.Shiptext = txt;
                            conf.InfotextAdd("Statek nr " + ShipsOnMap + " gotowy!");
                            conf.InfotextAdd("Dodaj statek " + mapShip[ShipsOnMap].type + "-masztowy\n");
                        }
                        return true;
                    }
                    else
                    {
                        conf.InfotextAdd("Błąd! Pole musi dotykać pozostałej częsci statku");
                        return false; //część statku nie styka się z resztą
                    }
                }
            }
            return false;
        }

        public bool removeShip(int id, ref Field[] fields)
        {
            List<int> coll = FindCollisions(id, fields);

            if (coll.Count == 3)
            {
                int collCount = 0;
                List<int> coll1 = FindCollisions(coll[0], fields);
                List<int> coll2 = FindCollisions(coll[1], fields);
                List<int> coll3 = FindCollisions(coll[2], fields);

                if (coll1.Contains(coll[1]) && coll2.Contains(coll[0]))
                    collCount++;
                if (coll2.Contains(coll[2]) && coll3.Contains(coll[1]))
                    collCount++;
                if (coll3.Contains(coll[0]) && coll1.Contains(coll[2]))
                    collCount++;
                if (collCount >= 2)
                {
                    mapShip[ShipsOnMap].RemoveCord(id);
                    conf.InfotextAdd("Pole odznaczone: " + id);
                    fields[id].Background = "LightBlue";
                }
                else
                {
                    conf.InfotextAdd("Nie można odznaczyc pola (przerwanie integralności statku): " + id);
                }
            }
            else if (coll.Count == 2)
            {
                List<int> coll1 = FindCollisions(coll[0], fields);
                List<int> coll2 = FindCollisions(coll[1], fields);
                if (coll1.Contains(coll[1]) && coll2.Contains(coll[0]))
                {
                    mapShip[ShipsOnMap].RemoveCord(id);
                    conf.InfotextAdd("Pole odznaczone: " + id);
                    fields[id].Background = "LightBlue";
                }
                else
                {
                    conf.InfotextAdd("Nie można odznaczyc pola (przerwanie integralności statku): " + id);
                }
            }
            else
            {
                mapShip[ShipsOnMap].RemoveCord(id);
                conf.InfotextAdd("Pole odznaczone: " + id);
                fields[id].Background = "LightBlue";
            }

            return false;
        }

        public List<int> FindCollisions(int id, Field[] fields)
        {
            List<int> lista = new List<int>();
            int y = id%10;
            int x = (id - y)/10;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (x + i >= 0 && x + i <= 9 && y + j >= 0 && y + j <= 9)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            int idk = ((x+i)*10)+(y+j);
                            if (fields[idk].Type == Field.FieldType.Builded || fields[idk].Type == Field.FieldType.Ship)
                                lista.Add(idk);

                        }
                    }
                }
            }
            return lista;
        }

        public int moves;

        public void SendReady()
        {
            conf.SendMessage("ready");
        }
        public void SendYourTurn()
        {
            conf.SendMessage("go");
            conf.InfotextAdd("Tura przeciwnika\n");
        }
        public bool CheckShipState(int id, ref Field[] fields)
        {
            int shipid = -1;
            for(int item = 0; item < mapShip.Length; item++)
            {
                foreach (int it in mapShip[item].coords)
                {
                    if (it == id)
                    {
                        shipid = item;
                        break;
                    }
                }
            }

            bool sinked = true;
            foreach (int item in mapShip[shipid].coords)
            {
                if (fields[item].Type == Field.FieldType.Hited)
                {

                }
                else
                {
                    sinked = false;
                }
            }

            if (sinked)
            {
                foreach (int item in mapShip[shipid].coords)
                {
                    fields[item].Background = "DarkRed";
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MarkSinked(int id, ref Field[] Enemyfields)
        {
            int y = id % 10;
            int x = (id - y) / 10;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (x + i >= 0 && x + i <= 9 && y + j >= 0 && y + j <= 9)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            int idk = ((x + i) * 10) + (y + j);
                            if (Enemyfields[idk].Type == Field.FieldType.Unknown)
                                Enemyfields[idk].Background = "DarkGray";
                            if (Enemyfields[idk].Type == Field.FieldType.Hited)
                            {
                                Enemyfields[idk].Background = "DarkRed";
                                MarkSinked(idk, ref Enemyfields);
                            }
                        }
                    }
                }
            }
        }

        public string SendQuestion(int id)
        {
            conf.SendMessage("a" + id);
            string ans = conf.GetMessage();
            if (ans == "hit")
                {
                    conf.InfotextAdd("Strzał: " + id + ", trafiony");
                }
            else if (ans == "mis")
                {
                    conf.InfotextAdd("Strzał: " + id + ", pudło");
                }
            else if (ans == "sink")
            {
                conf.InfotextAdd("Strzał: " + id + ", trafiony zatopiony");
            }
            return ans;

        }

        public void SendNotHitted(Field[] myFields)
        {
            string txt = "";
            foreach (Field item in myFields)
            {
                if (item.Type == Field.FieldType.Ship)
                {
                    txt += "m" + item.id.ToString();
                }
            }
            conf.SendMessage(txt);
        }

        public int getMode()
        {
            return conf.mode;
        }
    }
}
