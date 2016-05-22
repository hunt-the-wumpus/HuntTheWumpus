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
        public bool IsShotArrow { get; private set; }

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
            IsShotArrow = false;
        }

        public bool CanBuyArrow()
        {
            return Coins >= CostArrow;
        }
        public void BuyArrows()
        {
            Coins -= CostArrow;
        }
        public int NeedForBuyArrows()
        {
            return Math.Max(0, CostArrow - Coins);
        }
        public void GiveArrows()
        {
            CountBuyArrow += 2;
            Arrow += 2;
        }

        public void PushArrow()
        {
            --Arrow;
            IsShotArrow = true;
        }
        public bool CanBuyHint()
        {
            return Coins >= CostHint;
        }
        public void BuyHint()
        {
            Coins -= CostHint;
        }
        public int NeedForBuyHint()
        {
            return Math.Max(0, CostHint - Coins);
        }
        public void AddCoins(int i)
        {
            Coins += i;
        }
        public void GetAchievement(List<string> achiv)
        {
            if (CountBuyArrow == 2)
                achiv.Add(Messages.ShooterAchiv);
            if (CountBuyArrow == 4)
                achiv.Add(Messages.HunterAchiv);
            if (CountBuyArrow == 8)
                achiv.Add(Messages.RobinAchiv);
        }
    }
}
