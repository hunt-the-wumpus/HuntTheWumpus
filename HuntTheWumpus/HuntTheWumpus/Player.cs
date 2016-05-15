using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTheWumpus
{
    public class Player
    {
        public int Arrow { get; private set; }
        public int Coins { get; private set; }

        private int CostArrow;
        private int CountBuyArrow;
        private int CostHint;

        public Player()
        {
            CostArrow = 15;
            CostHint = 25;
            Coins = 5;
            Arrow = 3;
            CountBuyArrow = 0;
        }

        public bool CanBuyArrow()
        {
            return Coins >= CostArrow;
        }
        public void BuyArrows()
        {
            Coins -= CostArrow;
        }
        public void GiveArrows()
        {
            CountBuyArrow += 2;
            Arrow += 2;
        }

        public void PushArrow()
        {
            --Arrow;
        }
        public bool CanBuyHint()
        {
            return Coins >= CostHint;
        }
        public void BuyHint()
        {
            Coins -= CostHint;
        }
        public void AddCoins(int i)
        {
            Coins += i;
        }
        public void GetAchievement(List<string> achiv)
        {
            if (CountBuyArrow == 2)
                achiv.Add("Shooter.png/Shooter#Buy 2 arrows");
            if (CountBuyArrow == 4)
                achiv.Add("Hunter.png/Hunter#Buy 4 arrows");
            if (CountBuyArrow == 8)
                achiv.Add("Robin.png/Robin Hood#Buy 8 arrows");
        }
    }
}
