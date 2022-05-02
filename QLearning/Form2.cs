using System;
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
    public partial class Form2 : Form
    {
        PictureBox[,] pictureBoxes;
        public Form2()
        {
            InitializeComponent();
        }

        private void createBoxes()
        {
            panel1.Controls.Clear();

            panel1.Width = Commons.size * Commons.boxSize + 2;
            panel1.Height = Commons.size * Commons.boxSize + 2;

            groupBox1.Location = new Point(Commons.size * Commons.boxSize + 20, 12);
            
            this.Width = Commons.size * Commons.boxSize + 210;
            this.Height = Commons.size * Commons.boxSize + 150;

            pictureBoxes = new PictureBox[Commons.size,Commons.size];

            for (int i = 0; i < Commons.size; i++)
            {
                for (int j = 0; j < Commons.size; j++)
                {
                    PictureBox pictureBox = new PictureBox
                    {
                        Name = (i * Commons.size + j).ToString(),
                        Size = new Size(Commons.boxSize, Commons.boxSize),
                        Location = new Point(j * Commons.boxSize, i * Commons.boxSize),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle,
                        
                    };

                    pictureBox.MouseClick += new MouseEventHandler(pictureBox_MouseDown);
                    pictureBoxes[i, j] = pictureBox;

                    panel1.Controls.Add(pictureBox);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            int x = random.Next(0, Commons.size); ;
            int y = random.Next(0, Commons.size); ;

            for (int i=0; i<Commons.size * Commons.size * Commons.fillRatio; i++)
            {
                while(pictureBoxes[x,y].BackColor==Color.OrangeRed)
                {
                    x = random.Next(0, Commons.size);
                    y = random.Next(0, Commons.size);
                }

                pictureBoxes[x, y].BackColor = Color.OrangeRed;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;

            int pbId = int.Parse(pictureBox.Name);
            int x = pbId / Commons.size;
            int y = pbId % Commons.size;

            if (pictureBox.BackColor == Color.White)
            {
                pictureBox.BackColor = Color.OrangeRed;
                pictureBoxes[x, y].BackColor = Color.OrangeRed;
            }
            else
            {
                pictureBox.BackColor = Color.White;
                pictureBoxes[x, y].BackColor = Color.White;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Metin Dosyası |*.txt";

            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                String path = saveFileDialog.FileName;
                saveMaze(path);
            }
        }

        private void saveMaze(String path)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, false))
            {
                for (int i = 0; i < Commons.size; i++)
                {
                    for (int j = 0; j < Commons.size; j++)
                    {
                        String row = i.ToString() + "," + j.ToString();

                        if (pictureBoxes[i, j].BackColor == Color.White)
                        {
                            row += ",B";
                        }
                        else
                        {
                            row += ",K";
                        }

                        file.WriteLine(row);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int size = int.Parse(textBox1.Text);

                if (size >= 4)
                {
                    Commons.size = size;
                    createBoxes();
                }
                else
                {
                    MessageBox.Show("Lütfen Labirent Boyutunu Kontrol Ediniz", "Q Learning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Lütfen Labirent Boyutunu Kontrol Ediniz", "Q Learning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
