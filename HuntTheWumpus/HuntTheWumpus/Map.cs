using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTheWumpus
{
    public enum Danger
    {
        Empty,
        Bat,
        Pit,
        Wumpus
    }

    public class Map : IMap
    {
        public Danger danger { get; private set; }
        public int Room { get; private set; }
        public int Turn { get; private set; }
        public bool IsWin { get; private set; }

        private Tuple<int, int> BatRoom;
        private Tuple<int, int> PitRoom;
        public int Wumpus { get; private set; }

        public List<int>[] graph;
        public List<bool>[] isActive;
        private System.Random random;

        Tuple<int, int> cell(int x, int y)
        {
            return Tuple.Create((x + 6) % 6 + 6 * ((y + 5) % 5), random.Next() % 100000);
        }

        void GenGraph()
        {
            List<Tuple<int, int>>[] g = new List<Tuple<int, int>>[30];
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    int now = cell(i, j).Item1;
                    g[now] = new List<Tuple<int, int>>();
                    g[now].Add(cell(i, j - 1));
                    if (i % 2 == 1)
                    {
                        g[now].Add(cell(i - 1, j));
                        g[now].Add(cell(i - 1, j + 1));
                        g[now].Add(cell(i, j + 1));
                        g[now].Add(cell(i + 1, j + 1));
                        g[now].Add(cell(i + 1, j));
                    }
                    else
                    {
                        g[now].Add(cell(i - 1, j - 1));
                        g[now].Add(cell(i - 1, j));
                        g[now].Add(cell(i, j + 1));
                        g[now].Add(cell(i + 1, j));
                        g[now].Add(cell(i + 1, j - 1));
                    }
                }
            }
            int[,] ok = new int[30, 30];
            for (int i = 0; i < 30; ++i)
                for (int j = 0; j < 30; ++j)
                    ok[i, j] = 0;
            int[] ctv = new int[30];
            SortedSet<Tuple<int, int, int>> st = new SortedSet<Tuple<int, int, int>>();
            int startvert = random.Next() % 30;
            st.Add(Tuple.Create(0, startvert, startvert));
            bool[] b = new bool[30];
            for (int i = 0; i < 30; ++i)
            {
                while (st.Count > 0 && (b[st.Min.Item2] || ctv[st.Min.Item3] == 3))
                    st.Remove(st.Min);
                if (st.Count == 0)
                {
                    GenGraph();
                    return;
                }
                int start = st.Min.Item2;
                int finish = st.Min.Item3;
                b[start] = true;
                if (finish != start)
                {
                    ok[start, finish] = ok[finish, start] = 1;
                    ++ctv[finish];
                }
                ++ctv[start];

                int cnt = 0;
                for (int j = 0; j < g[start].Count; ++j)
                    if (!b[g[start][j].Item1])
                        ++cnt;
                int okcnt = 0;
                for (int j = 0; j < g[start].Count; ++j)
                    if (!b[g[start][j].Item1])
                    {
                        int rn = random.Next() % cnt;
                        if (rn < 3 - okcnt)
                        {
                            ++okcnt;
                            st.Add(Tuple.Create(g[start][j].Item1, g[start][j].Item1, start));
                        }
                        --cnt;
                    }
            }
            int cntfail = 0;
            while (cntfail < 10)
            {
                int v = random.Next() % 30, i = random.Next() % 6;
                int fin = g[v][i].Item1;
                if (ctv[v] == 3 || ctv[fin] == 3 || ok[v, fin] == 1)
                    ++cntfail;
                else
                {
                    ok[v, fin] = ok[fin, v] = 1;
                    ++ctv[v];
                    ++ctv[fin];
                }
            }
            for (int i = 0; i < 30; ++i)
            {
                isActive[i] = new List<bool>();
                graph[i] = new List<int>();
                for (int j = 0; j < 6; j++)
                {
                    graph[i].Add(g[i][j].Item1);
                    if (ok[i, g[i][j].Item1] == 1)
                        isActive[i].Add(true);
                    else
                        isActive[i].Add(false);
                }
            }
        }
        bool RoomIsDanger(int i)
        {
            return (i == BatRoom.Item1) || (i == BatRoom.Item2) || (i == PitRoom.Item2) || (i == PitRoom.Item1) || (i == Wumpus);
        }
        public Map()
        {
            random = new Random();
            graph = new List<int>[30];
            isActive = new List<bool>[30];
            GenGraph();
            int[] a = new int[30];
            for (int i = 0; i < 30; i++)
            {
                a[i] = i;
                int rnd = random.Next() % (i + 1);
                Program.Swap<int>(ref a[rnd], ref a[i]);
            }
            BatRoom = Tuple.Create(a[0], a[1]);
            PitRoom = Tuple.Create(a[2], a[3]);
            Wumpus = a[4];
            Room = a[5];
            Turn = 0;
            IsWin = false;
            danger = Danger.Empty;
        }
        public void Move(int i)
        {
            if (isActive[Room][i])
            {
                ++Turn;
                Room = graph[Room][i];
                if (Room == BatRoom.Item1 || Room == BatRoom.Item2)
                    danger = Danger.Bat;
                else if (Room == PitRoom.Item1 || Room == PitRoom.Item2)
                    danger = Danger.Pit;
                else if (Room == Wumpus)
                    danger = Danger.Wumpus;
                else
                    danger = Danger.Empty;
            }
        }
        public void PushArrow(int i)
        {
            if (isActive[Room][i] && Wumpus == graph[Room][i])
                IsWin = true;
        }
        public void WumpusGoAway()
        {
            int mem = Wumpus;
            int rnd = random.Next() % 6;
            while (!isActive[mem][rnd])
                rnd = random.Next() % 6;
            Wumpus = graph[mem][rnd];
            rnd = random.Next() % 6;
            int cnt = 0;
            while ((!isActive[Wumpus][rnd] || graph[Wumpus][rnd] == mem || RoomIsDanger(graph[Wumpus][rnd])) && cnt < 100)
            {
                rnd = random.Next() % 6;
                ++cnt;
            }
            if (cnt < 100)
                Wumpus = graph[Wumpus][rnd];
        }
        public void Respaw()
        {
            int[] a = new int[27];
            int j = 0;
            for (int i = 0; i < 30; i++)
            {
                if (i != PitRoom.Item1 && i != PitRoom.Item2 && i != Wumpus)
                {
                    a[j] = i;
                    int rnd = random.Next() % (j + 1);
                    Program.Swap<int>(ref a[rnd], ref a[j]);
                    ++j;
                }
            }
            BatRoom = Tuple.Create(a[0], a[1]);
            Room = a[2];
        }
        public int GetBat()
        {
            int rnd = random.Next();
            if (rnd % 2 == 0)
                return BatRoom.Item1;
            return BatRoom.Item2;
        }
        public int GetPit()
        {
            int rnd = random.Next();
            if (rnd % 2 == 0)
                return PitRoom.Item1;
            return PitRoom.Item2;
        }
    }

    interface IMap
    {
        Danger danger { get; }
        int Room { get; }
        void Move(int i);
        int Turn { get; }
        void PushArrow(int n);
        bool IsWin { get; }
        void Respaw();
        void WumpusGoAway();
    }
}
