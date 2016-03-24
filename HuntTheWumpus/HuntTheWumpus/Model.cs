using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuntTheWumpus
{
    class Model
    {
        public MiniGame minigame { get; set; }

        public Model(int width, int height)
        {
            minigame = new MiniGame(width, height);
            minigame.InitializeMiniGame(2);
        }
    }
}
