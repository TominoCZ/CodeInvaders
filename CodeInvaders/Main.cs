using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeInvaders
{
    public partial class Main : Form
    {
        private Game _game;
        private Random _random;

        private bool _left;
        private bool _right;

        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _game = new Game(this)
            {
                OnEnd = KonecHry,
                OnPaint = Vykresli,
                OnKeyDown = Stiskl,
                OnKeyUp = Pustil
            };
            _random = new Random();
        }

        private void Vykresli(Graphics g, float delta)
        {
            var rychlost = _game.Speed;

            if (_left)
            {
                _game.PlayerX -= delta * rychlost;
            }
            if (_right)
            {
                _game.PlayerX += delta * rychlost;
            }
        }

        private void Stiskl(Keys key)
        {
            Console.WriteLine("stiskl: " + key.ToString());

            if (key == Keys.A || key == Keys.Left)
            {
                _left = true;
            }
            if (key == Keys.D || key == Keys.Right)
            {
                _right = true;
            }
        }

        private void Pustil(Keys key)
        {
            Console.WriteLine("pustil: " + key.ToString());

            if (key == Keys.A || key == Keys.Left)
            {
                _left = false;
            }
            if (key == Keys.D || key == Keys.Right)
            {
                _right = false;
            }
        }

        private void KonecHry()
        {
            btnStart.Visible = true;
        }

        private void starTimer_Tick(object sender, EventArgs e)
        {
            if (!_game.IsRunning)
                return;

            for (int i = 0; i < 2; i++)
            {
                _game.Particles.Add(new ParticleStar((float)_random.NextDouble() * ClientSize.Width));
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _game.Reset();
            _game.Start();

            btnStart.Visible = false;
        }

        private void entityTimer_Tick(object sender, EventArgs e)
        {
            if (!_game.IsRunning)
                return;

            var size = 64;
            var entity = new Entity($"enemy{_random.Next(1, 6)}", size / 2 + (float)_random.NextDouble() * (ClientSize.Width - size));
            entity.VelX = (1 - (float)_random.NextDouble() * 2) * 60;
            entity.PosY -= (float)_random.NextDouble() * 128;
            entity.Size = size;
            entity.OnHit = () =>
            {
                _game.Health -= 50;
            };
            _game.Entities.Add(entity);
        }
    }
}
