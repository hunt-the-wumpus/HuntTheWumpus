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

        private List<int>[] coins;
        public List<int>[] graph;
        public List<bool>[] isActive;

        private int VisitRoom;
        private int[] CountVisit;

        Tuple<int, int> cell(int x, int y)
        {
            return Tuple.Create((x + 6) % 6 + 6 * ((y + 5) % 5), Utily.Next() % 100000);
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
            int startvert = Utily.Next() % 30;
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
                        int rn = Utily.Next() % cnt;
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
                int v = Utily.Next() % 30, i = Utily.Next() % 6;
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
            coins = new List<int>[30];
            for (int i = 0; i < 30; ++i)
            {
                coins[i] = new List<int>();
                isActive[i] = new List<bool>();
                graph[i] = new List<int>();
                for (int j = 0; j < 6; j++)
                {
                    graph[i].Add(g[i][j].Item1);
                    if (j < 3)
                        coins[i].Add(Utily.Next() % 4 + 1);
                    else coins[i].Add(0);
                    if (ok[i, g[i][j].Item1] == 1)
                        isActive[i].Add(true);
                    else
                        isActive[i].Add(false);
                }
            }
            for (int i = 0; i < 30; ++i)
                for (int j = 0; j < 3; ++j)
                    coins[graph[i][j]][j + 3] = coins[i][j];
        }
        bool RoomIsDanger(int i)
        {
            return (i == BatRoom.Item1) || (i == BatRoom.Item2) || (i == PitRoom.Item2) || (i == PitRoom.Item1) || (i == Wumpus);
        }
        public Map()
        {
            graph = new List<int>[30];
            isActive = new List<bool>[30];
            CountVisit = new int[30];
            VisitRoom = 1;
            GenGraph();
            int[] a = new int[30];
            for (int i = 0; i < 30; i++)
            {
                a[i] = i;
                int rnd = Utily.Next() % (i + 1);
                Utily.Swap<int>(ref a[rnd], ref a[i]);
            }
            BatRoom = Tuple.Create(a[0], a[1]);
            PitRoom = Tuple.Create(a[2], a[3]);
            Wumpus = a[4];
            Room = a[5];
            CountVisit[Room] = 1;
            Turn = 0;
            IsWin = false;
            danger = Danger.Empty;
        }
        public int Move(int i)
        {
            int ans = -1;
            if (isActive[Room][i])
            {
                ++CountVisit[graph[Room][i]];
                if (CountVisit[graph[Room][i]] == 1)
                    ++VisitRoom;
                ++Turn;
                ans = coins[Room][i];
                coins[Room][i] = coins[graph[Room][i]][(i + 3) % 6] = 0;                
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
            return ans;
        }
        public void PushArrow(int i)
        {
            if (isActive[Room][i] && Wumpus == graph[Room][i])
                IsWin = true;
        }

        private bool TryWumpusGoAway()
        {
            int step = Utily.Next() % 3 + 2;
            int nowWumpus = Wumpus;
            int lastdir = -1;
            for (int i = 0; i < step; i++)
            {
                List<int> good = new List<int>();
                for (int j = 0; j < 6; ++j)
                    if (isActive[nowWumpus][i] && lastdir != j)
                        good.Add(j);
                if (good.Count == 0)
                    return false;
                int dir = Utily.Next() % good.Count;
                lastdir = (dir + 3) % 6;
                nowWumpus = graph[nowWumpus][dir];
            }
            if (!RoomIsDanger(nowWumpus))
            {
                Wumpus = nowWumpus;
                return true;
            }
            return false;
        }

        public void WumpusGoAway()
        {
            bool flag = false;
            while (!flag)
                flag = TryWumpusGoAway();
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
                    int rnd = Utily.Next() % (j + 1);
                    Utily.Swap<int>(ref a[rnd], ref a[j]);
                    ++j;
                }
            }
            BatRoom = Tuple.Create(a[0], a[1]);
            Room = a[2];
            danger = Danger.Empty;
        }
        public int GetBat()
        {
            int rnd = Utily.Next();
            if (rnd % 2 == 0)
                return BatRoom.Item1;
            return BatRoom.Item2;
        }
        public int GetPit()
        {
            int rnd = Utily.Next();
            if (rnd % 2 == 0)
                return PitRoom.Item1;
            return PitRoom.Item2;
        }
        public Danger GetDanger(int room)
        {
            if (Wumpus == room)
                return Danger.Wumpus;
            if (BatRoom.Item1 == room || BatRoom.Item2 == room)
                return Danger.Bat;
            if (PitRoom.Item1 == room || PitRoom.Item2 == room)
                return Danger.Pit;
            return Danger.Empty;
        }
        public Danger GetDangerAbout()
        {
            Danger ans = Danger.Empty;
            for (int i = 0; i < 6; i++)
            {
                ans = (Danger)Math.Max((int)ans, (int)GetDanger(graph[Room][i]));
            }
            return ans;
        }
        public List<Danger> GetDangerList()
        {
            List<Danger> ans = new List<Danger>();
            for (int i = 0; i < 6; i++)
            {
                ans.Add(GetDanger(graph[Room][i]));
            }
            return ans;
        }

        public void GetAchievement(List<string> achiv)
        {
            if (VisitRoom == 30)
                achiv.Add("dich.png/Исследователь#Побывать в каждой#комнате пещеры");
            if (VisitRoom >= 25)
                achiv.Add("MG1.png/Шаг в бездну#Побывать в 25 комнатах");
            bool flag = true;
            for (int i = 0; i < 30; ++i)
                flag = flag && CountVisit[i] == 1;
            if (flag)
                achiv.Add("Hamilton.png/Потомок Гамильтона#Пройти всю пещеру,#побывав в каждой#комнате по разу");
        }
    }

    interface IMap
    {
        Danger danger { get; }
        int Room { get; }
        int Move(int i);
        int Turn { get; }
        void PushArrow(int n);
        bool IsWin { get; }
        void Respaw();
        void WumpusGoAway();
    }
}
