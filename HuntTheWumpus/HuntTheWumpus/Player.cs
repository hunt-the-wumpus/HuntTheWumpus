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
        private int CostHint;

        public Player()
        {
            CostArrow = 3;
            CostHint = 5;
            Arrow = 3;
        }

        public bool CanBuyArrow()
        {
            return Coins >= CostArrow && Arrow < 3;
        }
        public void BuyArrow()
        {
            Arrow = 3;
            Coins -= CostArrow;
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
    }
}
