﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Snake
{
    public partial class Snake : Form
    {
        int cols = 50,
            rows = 25,
            score = 0,
            dx = 0,
            dy = 0,
            front = 0,
            back = 0;

        Piece[] snake = new Piece[1250];
        List<int> available = new List<int>();
        bool[,] visit;

        Random random = new Random();
        Timer timer = new Timer();

        public Snake()
        {
            InitializeComponent();
            Initial();
            LaunchTimer();
        }

        private void LaunchTimer()
        {
            timer.Interval = 50;
            timer.Tick += Moving;
            timer.Start();
        }

        private void Snake_KeyDown(object sender, KeyEventArgs e)
        {
            dx = dy = 0;

            switch (e.KeyCode)
            {
                case Keys.Right:
                    dx = 20;
                    break;
                case Keys.Left:
                    dx = -20;
                    break;
                case Keys.Up:
                    dy = -20;
                    break;
                case Keys.Down:
                    dy = 20;
                    break;
            }
        }

        private void Moving(object sender, EventArgs e)
        {
            int x = snake[front].Location.X,
                y = snake[front].Location.Y;

            if (dx == 0 && dy == 0) return;

            if (game_over(x + dx, y + dy))
            {
                timer.Stop();
                MessageBox.Show("Game Over");
                return;
            }
            if (collisionFood(x + dx, y + dy))
            {
                score += 1;
                labelScore.Text = "Score: " + score.ToString();
                if (hits((y + dy) / 20, (x + dx) / 20)) return;

                Piece head = new Piece(x + dx, y + dy);
                front = (front - 1 + 1250) % 1250;
                snake[front] = head;
                visit[head.Location.Y / 20, head.Location.X / 20] = true;
                Controls.Add(head);
                randomFood();
            }
            else
            {
                if (hits((y + dy) / 20, (x + dx) / 20)) return;
                visit[snake[back].Location.Y / 20, snake[back].Location.X / 20] = false;
                front = (front - 1 + 1250) % 1250;
                snake[front] = snake[back];
                snake[front].Location = new Point(x + dx, y + dy);
                back = (back - 1 + 1250) % 1250;
                visit[(y + dy) / 20, (x + dx) / 20] = true;
            }
        }

        private void randomFood()
        {
            available.Clear();
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (!visit[i, j]) available.Add(i * cols + j);
            int idx = random.Next(available.Count) % available.Count;
            labelFood.Left = (available[idx] * 20) % Width;
            labelFood.Top = (available[idx] * 20) / Width * 20;                
                
        }

        private bool hits(int x, int y)
        {
            if (visit[x, y])
            {
                timer.Stop();
                MessageBox.Show("Oh, no! Snake hits its body!");
                MessageBox.Show("Game over");
                return true;
            }
            return false;
        }

        private bool collisionFood(int x, int y)
        {
            return x == labelFood.Location.X && y == labelFood.Location.Y;
        }

        private bool game_over(int x, int y)
        {
            return x < 0 || y < 0 || x > 980 || y > 480;
        }

        private void Initial()
        {
            visit = new bool[rows, cols];
            Piece head 
                = new Piece((random.Next() % cols) * 20, (random.Next() % rows) * 20);
            labelFood.Location 
                = new Point((random.Next() % cols) * 20, (random.Next() % rows) * 20);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    visit[i, j] = false;
                    available.Add(i * cols + j);
                }
            visit[head.Location.Y / 20, head.Location.X / 20] = true;
            available.Remove(head.Location.Y / 20 * cols + head.Location.X / 20);
            Controls.Add(head);
            snake[front] = head;
        }
    }
}
