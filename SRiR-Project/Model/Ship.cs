using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SRiR_Project.Model
{
    class Ship
    {
        public Ship(int typ)
        {
            type = typ;
            coords = new int[typ];

            for (int i = 0; i < typ; i++)
            {
                coords[i] = -1;
            }
            FieldsDone = 0;
            shipState = State.Builded;
        }

        public int type;
        public int[] coords;
        public State shipState;

        public int FieldsDone;

        public enum State{Builded, Alive, Sinked}

        public bool AddCord(int id)
        {
            if(shipState == State.Builded)
                if (coords[FieldsDone] == -1)
                {
                    coords[FieldsDone] = id;
                    FieldsDone++;
                    if (FieldsDone == type)
                    {
                        shipState = State.Alive;
                    }
                    return true;
                }
            return false;
        }

        public bool RemoveCord(int id)
        {
            if (shipState == State.Builded)
            {
                int it = 0;
                bool found = false;
                foreach (int item in coords)
                {
                    if (item == id || found)
                    {
                        found = true;
                        if (it + 1 < coords.Length)
                        {
                            coords[it] = coords[it + 1];
                        }
                        else
                        {
                            coords[it] = -1;
                            FieldsDone--;
                            return true;
                        }
                        
                    }
                    it++;
                }
            }
            return false;
        }

        public bool CheckAssign(int id)
        {
            foreach (int item in coords)
            {
                if (item == id)
                    return true;
            }
            return false;
        }
    }
}
