using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLearning
{
    public partial class Form4 : Form
    {
        public static ArrayList costs = new ArrayList();

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            foreach (int cost in costs)
            {
                chart1.Series["Maliyet"].Points.Add(cost);
            }
        }
    }
}
