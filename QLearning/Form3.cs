using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLearning
{
    public partial class Form3 : Form
    {
        String path;

        PictureBox[,] pictureBoxes;
        Cell[] cells;

        int startRoom = -1;
        int endRoom = -1;

        public static int[,] R;
        public static double[,] Q;

        public static double gamma = 0.8;
        int sleepTime = 1;

        bool result = false;

        public static ArrayList costs = new ArrayList();

        public Form3()
        {
            InitializeComponent();
        }

        private void initialize()
        {
            R = new int[Commons.size * Commons.size, Commons.size * Commons.size];
            Q = new double[Commons.size * Commons.size, Commons.size * Commons.size];

            resetRMatrix();
            resetQMatrix();

            result = false;

            sleepTime = int.Parse(comboBox4.Text);
        }

        private void resetRMatrix()
        {
            for (int i = 0; i < Commons.size * Commons.size; i++)
            {
                for (int j = 0; j < Commons.size * Commons.size; j++)
                {
                    R[i, j] = -1;
                }
            }
        }

        private void resetQMatrix()
        {
            for (int i = 0; i < Commons.size * Commons.size; i++)
            {
                for (int j = 0; j < Commons.size * Commons.size; j++)
                {
                    Q[i, j] = 0;
                }
            }
        }

        private void updateRMatrix()
        {
            for (int i = 0; i < Commons.size * Commons.size; i++)
            {
                int[] neighbours = findNeighbours(i);

                for(int j=0; j<neighbours.Length; j++)
                {
                    int neighbour = neighbours[j];

                    if(neighbour!=-1)
                    {
                        int x = neighbour / Commons.size;
                        int y = neighbour % Commons.size;

                        Color boxColor = pictureBoxes[x, y].BackColor;

                        if (boxColor == Color.OrangeRed)
                        {
                            R[i, neighbour] = -1;
                        }
                        else
                        {
                            if(neighbour==endRoom)
                            {
                                R[i, neighbour] = 100;
                            }
                            else
                            {
                                R[i, neighbour] = 0;
                            } 
                        }
                    }
                }
            }
        }

        public int[] findNeighbours(int no)
        {
            int[] neighbours = { -1, -1, -1, -1, -1, -1, -1, -1 };

            int row = no / Commons.size;

            neighbours[0] = (no - 1 < (row * Commons.size)) ? -1 : no - 1;
            neighbours[1] = ((no - Commons.size) % Commons.size <= 0) ? -1 : no - Commons.size - 1;
            neighbours[2] = (no - Commons.size < 0) ? -1 : no - Commons.size;
            neighbours[3] = ((no - Commons.size + 1) % Commons.size <= 0) ? -1 : no - Commons.size + 1;
            neighbours[4] = (no + 1 >= ((row + 1) * Commons.size)) ? -1 : no + 1;
            neighbours[5] = ((no + Commons.size + 1) % Commons.size == 0) ? -1 : no + Commons.size + 1;
            neighbours[6] = (no + Commons.size >= (Commons.size * Commons.size)) ? -1 : no + Commons.size;
            neighbours[7] = ((no + Commons.size) % Commons.size == 0) ? -1 : no + Commons.size - 1;

            neighbours[5] = neighbours[5] >= Commons.size * Commons.size ? -1 : neighbours[5];
            neighbours[6] = neighbours[6] >= Commons.size * Commons.size ? -1 : neighbours[6];
            neighbours[7] = neighbours[7] >= Commons.size * Commons.size ? -1 : neighbours[7];

            return neighbours;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Metin Dosyası |*.txt";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                generateMaze(path);
                initialize();
            }      
        }

        private void generateMaze(String path)
        {
            panel1.Controls.Clear();

            int size = (int)Math.Sqrt(File.ReadLines(path).Count());

            Commons.size = size;

            panel1.Width = size * Commons.boxSize + 2;
            panel1.Height = size * Commons.boxSize + 2;

            groupBox1.Location = new Point(size * Commons.boxSize + 20, 12);

            this.Width = size * Commons.boxSize + 200;
            this.Height = size * Commons.boxSize + 310;

            pictureBoxes = new PictureBox[Commons.size, Commons.size];
            cells = new Cell[Commons.size * Commons.size];

            using (StreamReader sr = File.OpenText(path))
            {
                string s = String.Empty;

                int cnt = 0;

                while ((s = sr.ReadLine()) != null)
                {
                    string[] box = s.Split(',');

                    int x = int.Parse(box[0]);
                    int y = int.Parse(box[1]);
                    String clr = box[2];

                    cells[cnt] = new Cell(cnt, clr);

                    Color color;

                    if (clr == "B")
                    {
                        color = Color.White;
                    }
                    else
                    {
                        color = Color.OrangeRed;
                    }

                    PictureBox pictureBox = new PictureBox
                    {
                        Name = (x * Commons.size + y).ToString(),
                        Size = new Size(Commons.boxSize, Commons.boxSize),
                        Location = new Point(y * Commons.boxSize, x * Commons.boxSize),
                        BackColor = color,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    pictureBox.MouseClick += new MouseEventHandler(pictureBox_MouseDown);
                    pictureBoxes[x, y] = pictureBox;
                    panel1.Controls.Add(pictureBox);

                    cnt++;
                }
            }

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            for (int i=0; i<size*size; i++)
            {
                comboBox1.Items.Add("Oda " + i.ToString());
                comboBox2.Items.Add("Oda " + i.ToString());
            }

            comboBox3.SelectedIndex = 9;
            comboBox4.SelectedIndex = 0;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            MessageBox.Show("Oda " + pictureBox.Name, "Q Learning", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(startRoom!=-1)
            {
                int cx = startRoom / Commons.size;
                int cy = startRoom % Commons.size;

                pictureBoxes[cx, cy].BackColor = Color.White;
            }

            int pbId = comboBox1.SelectedIndex;

            int x = pbId / Commons.size;
            int y = pbId % Commons.size;

            if(pictureBoxes[x, y].BackColor!=Color.OrangeRed)
            {
                pictureBoxes[x, y].BackColor = Color.BlueViolet;
                startRoom = comboBox1.SelectedIndex;
            }
            else
            {
                MessageBox.Show("Engel Olan Alan Başlangıç Noktası Olarak Seçilemez","Hata",MessageBoxButtons.OK, MessageBoxIcon.Error);
                startRoom = -1;
            } 
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (endRoom != -1)
            {
                int cx = endRoom / Commons.size;
                int cy = endRoom % Commons.size;

                pictureBoxes[cx, cy].BackColor = Color.White;
            }

            int pbId = comboBox2.SelectedIndex;

            int x = pbId / Commons.size;
            int y = pbId % Commons.size;

            if(startRoom == comboBox2.SelectedIndex)
            {
                MessageBox.Show("Başlangıç ve Bitiş Noktaları Aynı Olamaz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                endRoom = -1;
                return;
            }

            if (pictureBoxes[x, y].BackColor != Color.OrangeRed)
            {
                pictureBoxes[x, y].BackColor = Color.ForestGreen;
                endRoom = comboBox2.SelectedIndex;
            }
            else
            {
                MessageBox.Show("Engel Olan Alan Bitiş Noktası Olarak Seçilemez", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                endRoom = -1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex==-1)
            {
                MessageBox.Show("Lütfen Başlangıç Noktası Seçiniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen Bitiş Noktası Seçiniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox3.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen İterasyon Seçiniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox4.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen Animasyon Hızı Seçiniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            start();
        }

        private int selectRandomRoom(int[] relations)
        {
            int room = -1;

            while (room == -1)
            {
                Random rnd = new Random();
                int rndIndex = rnd.Next(0, relations.Length);

                if (relations[rndIndex] != -1)
                {
                    room = rndIndex;
                }
            }

            return room;
        }

        private double[] getRow(double[,] matrix, int index)
        {

            double[] row = new double[Commons.size * Commons.size];

            for (int i = 0; i < Commons.size * Commons.size; i++)
            {
                row[i] = matrix[index, i];
            }

            return row;
        }

        private int[] getRow(int[,] matrix, int index)
        {
            int[] row = new int[Commons.size * Commons.size];

            for (int i = 0; i < Commons.size * Commons.size; i++)
            {
                row[i] = matrix[index, i];
            }

            return row;
        }

        private double findMax(double[] values)
        {
            double temp = values[0];

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > temp)
                {
                    temp = values[i];
                }
            }

            return temp;
        }

        private int findRoomFromPoint(double[] points, double point)
        {
            int room = -1;

            for (int i = 0; i < points.Length; i++)
            {
                if (point == points[i])
                {
                    room = i;
                    break;
                }
            }

            return room;
        }

        private List<int> findDestinationRooms(int room)
        {
            List<int> destinationRooms = new List<int>();

            int[] rooms = getRow(R, room);

            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i] != -1)
                {
                    destinationRooms.Add(i);
                }
            }

            return destinationRooms;
        }

        private void drawPath()
        {
            int tempRoom = startRoom;
            int x = tempRoom / Commons.size;
            int y = tempRoom % Commons.size;

            pictureBoxes[x, y].BackColor = Color.Black;

            while (tempRoom != endRoom)
            {
                double maxPoint = findMax(getRow(Q, tempRoom));
                tempRoom = findRoomFromPoint(getRow(Q, tempRoom), maxPoint);
                String elementName = tempRoom.ToString();
                PictureBox selPicBox = (PictureBox)panel1.Controls.Find(elementName, true).FirstOrDefault();
                selPicBox.BackColor = Color.Black;
            }
        }

        private void start()
        {            
            updateRMatrix();

            sleepTime = int.Parse(comboBox4.Text);
            int iteration = int.Parse(comboBox3.Text);
            this.Text = "Labirent Çöz (İterasyon 0)";

            int cost = 0;

            for (int i = 1; i <= iteration; i++)
            {
                this.Text = "Labirent Çöz (İterasyon " + i.ToString() + ")";

                int position = startRoom;

                int ex = endRoom / Commons.size;
                int ey = endRoom % Commons.size;

                pictureBoxes[ex, ey].BackColor = Color.ForestGreen;

                while (position != endRoom)
                {
                    double[] points = getRow(Q, position);
                    double maxPoint = findMax(points);

                    Color oColor;

                    int cx = position / Commons.size;
                    int cy = position % Commons.size;

                    if(i==1)
                    {
                        cost += 5;
                        costs.Add(cost);
                    }
                    
                    if (position==startRoom)
                    {
                        oColor = Color.BlueViolet;
                    }
                    else
                    {
                        if (cells[position].isWhite())
                        {
                            oColor = Color.White;
                        }
                        else
                        {
                            oColor = Color.OrangeRed;
                        }
                    }
                    
                    if (maxPoint > 0)
                    {
                        position = findRoomFromPoint(points, maxPoint);
                    }
                    else
                    {

                        int[] relations = getRow(R, position);
                        int newRoom = selectRandomRoom(relations);

                        List<int> destinationRooms = findDestinationRooms(newRoom);

                        if (destinationRooms.Count != 0)
                        {
                            double[] destinationRoomPoints = new double[destinationRooms.Count];

                            for (int j = 0; j < destinationRooms.Count; j++)
                            {
                                destinationRoomPoints[j] = Q[newRoom, destinationRooms.ElementAt(j)];
                            }

                            Q[position, newRoom] = R[position, newRoom] + (gamma * findMax(destinationRoomPoints));
                            position = newRoom;
                        }
                    }

                    int nx = position / Commons.size;
                    int ny = position % Commons.size;

                    pictureBoxes[cx, cy].BackColor = oColor;
                    pictureBoxes[nx, ny].BackColor = Color.Black;

                    if (sleepTime != 0)
                    {
                        Thread.Sleep(sleepTime);
                        Application.DoEvents();
                    }
                }
            }

            drawPath();
            result = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(!result)
            {
                MessageBox.Show("Lütfen Öncelikle Öğrenme Sürecini Başlatın", "Q Learning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form4 form4 = new Form4();
            Form4.costs.Clear();
            Form4.costs = costs;
            form4.Show();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            sleepTime = int.Parse(comboBox4.Text);
        }
    }
}
