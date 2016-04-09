using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTheWumpus
{
    enum Danger
    {
        Empty,
        Bat,
        Pit,
        Wumpus
    }

    class Map : IMap
    {
        public Danger danger { get; private set; }
        public int room { get; private set; }
        public int turn { get; private set; }
        public bool IsWin { get; private set; }

        private List<int>[] graph;

        public Map()
        {
            graph = new List<int>[30];

        }

        public void Move(int i)
        {

        }

        public void PushArrow(int n)
        {
            
        }
        public void WumpusGoAway()
        {

        }
        public void Respaw()
        {

        }
    }

    interface IMap
    {
        Danger danger { get;}
        int room { get; }
        void Move(int i);
        int turn { get; }
        void PushArrow(int n);
        bool IsWin { get; }
        void Respaw();
        void WumpusGoAway();
    }
}
