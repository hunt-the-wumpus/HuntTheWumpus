using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTheWumpus
{
    class Map
    {
    }

    interface IMap
    {
        int danger { get; set; }
        int room { get; set; }//...
        void Move(int i);
        int turn { get; set; }
        int PushArrow(int n);
        bool IsWin { get; set; }
        void Respaw();
        void WumpusGoAway();
    }
}
