using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLearning
{
    class Cell
    {
        int no;
        String color;

        public Cell(int no, String color)
        {
            this.no = no;
            this.color = color;
        }

        public bool isWhite()
        {
            if(color=="B")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
