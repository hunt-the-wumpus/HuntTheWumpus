using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTheWumpus
{
    class Control
    {
        public Model model;
        public View view;
        public Control(int width, int height)
        {
            model = new Model();
            view = new View(width, height);
        }

        public void UpDate()
        {
        }
    }
}
