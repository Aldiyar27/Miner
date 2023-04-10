using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Miner
{
    public partial class GameMiner : Form
    {
        public static int mapSize = 7;
        public const int cellSize = 70;
        private static int currentPictureToSet = 0;
        public static int[,] map = new int[mapSize, mapSize];
        public static Button[,] buttons = new Button[mapSize, mapSize];
        private static bool isFirstStep;
        private static Point firstCoord;
        public static Form form;
        public static Panel panel;
        public static bool DarkMode = false;

        public GameMiner()
        {
            InitializeComponent();

            Game(panel1);
        }

        private static void ConfigureMapSize(Panel current)
        {
            current.Width = mapSize * cellSize;
            current.Height = mapSize * cellSize;
        }

        private static void Map()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        public static void Game(Panel current)
        {
            panel = current;
            currentPictureToSet = 0;
            isFirstStep = true;
            ConfigureMapSize(current);
            Map();
            Buttons(current);
        }

        private static void Buttons(Panel current)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.Image = GetImg(0);
                    button.MouseUp += new MouseEventHandler(OnButtonPressedMouse);
                    current.Controls.Add(button);
                    buttons[i, j] = button;
                }
            }
        }

        private static void OnButtonPressedMouse(object sender, MouseEventArgs e)
        {
            Button pressedButton = sender as Button;
            switch (e.Button.ToString())
            {
                case "Right":
                    OnRightButtonPressed(pressedButton);
                    break;
                case "Left":
                    OnLeftButtonPressed(pressedButton);
                    break;
            }
        }

        private static void OnRightButtonPressed(Button pressedButton)
        {
            currentPictureToSet++;
            currentPictureToSet %= 2;
            int num = 0;

            switch (currentPictureToSet)
            {
                case 0:
                    num = 0;
                    break;
                case 1:
                    num = -2;
                    break;
            }
            pressedButton.Image = GetImg(num);
        }

        private static Image GetImg(int num)
        {
            return Image.FromFile($"C:/Users/АшкеевАл/Downloads/Miner-master/Miner/Sprites/{num}.png");

        }

        private static void OnLeftButtonPressed(Button pressedButton)
        {
            pressedButton.Enabled = false;
            int iButton = pressedButton.Location.Y / cellSize;
            int jButton = pressedButton.Location.X / cellSize;
            if (isFirstStep)
            {
                firstCoord = new Point(jButton, iButton);
                SpawnBomb();
                CountBomb();
                isFirstStep = false;
            }
            OpenCells(iButton, jButton);

            if (map[iButton, jButton] == -1)
            {
                AllBombs(iButton, jButton);
                MessageBox.Show("Поражение!");
                panel.Controls.Clear();
                Game(panel);
            }
        }

        private static void AllBombs(int iBomb, int jBomb)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (i == iBomb && j == jBomb)
                        continue;
                    if (map[i, j] == -1)
                    {
                        buttons[i, j].Image = GetImg(-1);
                    }
                }
            }
        }

        private static void SpawnBomb()
        {
            Random r = new Random();
            int number = mapSize + 1;

            for (int i = 0; i < number; i++)
            {
                int posI = r.Next(0, mapSize - 1);
                int posJ = r.Next(0, mapSize - 1);

                while (map[posI, posJ] == -1 || (Math.Abs(posI - firstCoord.Y) <= 1 && Math.Abs(posJ - firstCoord.X) <= 1))
                {
                    posI = r.Next(0, mapSize - 1);
                    posJ = r.Next(0, mapSize - 1);
                }
                map[posI, posJ] = -1;
            }
        }

        private static void CountBomb()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == -1)
                    {
                        for (int k = i - 1; k < i + 2; k++)
                        {
                            for (int l = j - 1; l < j + 2; l++)
                            {
                                if (!IsInBorder(k, l) || map[k, l] == -1)
                                    continue;
                                map[k, l] = map[k, l] + 1;
                            }
                        }
                    }
                }
            }
        }

        private static void OpenCell(int i, int j)
        {
            buttons[i, j].Enabled = false;

            switch (map[i, j])
            {
                case 1:
                    buttons[i, j].Image = GetImg(1);
                    break;                
                case 2:
                    buttons[i, j].Image = GetImg(2);
                    break;                
                case 3:                   
                    buttons[i, j].Image = GetImg(3);
                    break;                
                case 4:
                    buttons[i, j].Image = GetImg(4);
                    break;                
                case 5:                   
                    buttons[i, j].Image = GetImg(5);
                    break;                
                case 6:                   
                    buttons[i, j].Image = GetImg(6);
                    break;                
                case 7:                   
                    buttons[i, j].Image = GetImg(7);
                    break;                
                case 8:                   
                    buttons[i, j].Image = GetImg(8);
                    break;
                case -1:
                    buttons[i, j].Image = GetImg(-1);
                    break;
                case 0:
                    buttons[i, j].Image = GetImg(0);
                    break;
            }
        }

        private static void OpenCells(int i, int j)
        {
            OpenCell(i, j);

            if (map[i, j] > 0)
                return;

            for (int k = i - 1; k < i + 2; k++)
            {
                for (int l = j - 1; l < j + 2; l++)
                {
                    if (!IsInBorder(k, l))
                        continue;
                    if (!buttons[k, l].Enabled)
                        continue;
                    if (map[k, l] == 0)
                        OpenCells(k, l);
                    else if (map[k, l] > 0)
                        OpenCell(k, l);
                }
            }
        }

        private static bool IsInBorder(int i, int j)
        {
            if (i < 0 || j < 0 || j > mapSize - 1 || i > mapSize - 1)
            {
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num = Int32.Parse(label2.Text);
            if (num >= 6)
            {
                mapSize--;
                label2.Text = mapSize.ToString();
                panel.Controls.Clear();
                Game(panel1);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            int num = Int32.Parse(label2.Text);
            if (num <= 6)
            {
                mapSize++;
                label2.Text = mapSize.ToString();
                panel.Controls.Clear();
                Game(panel1);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(DarkMode == false)
            {
                this.BackColor = Color.DimGray;
                label1.ForeColor = Color.White;
                label2.ForeColor = Color.White;
                label3.ForeColor = Color.White;
                DarkMode = true;
            }
            else
            {
                this.BackColor = Color.White;
                label1.ForeColor = Color.Black;
                label2.ForeColor = Color.Black;
                label3.ForeColor = Color.Black;
                DarkMode = false;
            }
        }
    }
}
