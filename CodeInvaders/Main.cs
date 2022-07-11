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

        private int _score;

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

            UpdateTitle();
        }

        private void entityTimer_Tick(object sender, EventArgs e)
        {
            if (!_game.IsRunning)
                return;
            var size = 64;

            Entity entity = null;

            if (_game.PlayerHealth <= 75 && _random.NextDouble() >= 0.9)
            {
                entity = new Entity($"heal", size / 2 + (float)_random.NextDouble() * (ClientSize.Width - size));
                entity.VelX = (1 - (float)_random.NextDouble() * 2) * 60;
                entity.PosY -= (float)_random.NextDouble() * 128;
                entity.Size = size;
                entity.OnTouch = () =>
                {
                    entity.Health = 0; //TODO

                    _game.PlayerHealth += 25;
                };
            }
            else
            {
                if (_random.NextDouble() >= 0.85)
                {
                    entity = new Entity($"money", size / 2 + (float)_random.NextDouble() * (ClientSize.Width - size));
                    entity.VelX = (1 - (float)_random.NextDouble() * 2) * 60;
                    entity.PosY -= (float)_random.NextDouble() * 128;
                    entity.Size = size;
                    entity.OnTouch = () =>
                    {
                        entity.Health = 0; //TODO

                        _score += 10;

                        UpdateTitle();
                    };
                }
                else
                {
                    entity = new Entity($"enemy{_random.Next(1, 7)}", size / 2 + (float)_random.NextDouble() * (ClientSize.Width - size));
                    entity.VelX = (1 - (float)_random.NextDouble() * 2) * 60;
                    entity.PosY -= (float)_random.NextDouble() * 128;
                    entity.Size = size;
                    entity.OnTouch = () =>
                    {
                        entity.Health = 0; //TODO

                        _game.PlayerHealth -= 50;

                        _game.Particles.Add(new ParticleExplosion(entity.PosX, entity.PosY));
                    };
                }
            }

            if (entity != null)
                _game.Entities.Add(entity);
        }

        private void UpdateTitle()
        {
            Text = $"CodeInvaders (score: {_score})";
        }
    }
}
