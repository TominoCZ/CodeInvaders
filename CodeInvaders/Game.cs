using Code_Invaders.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeInvaders
{
    public class Game : IDisposable
    {
        public Form Window;
        public ParticleManager Particles;
        public EntityManager Entities;

        public Action OnEnd;
        public Action<Graphics, float> OnPaint;
        public Action<Keys> OnKeyDown;
        public Action<Keys> OnKeyUp;

        private HashSet<Keys> _keysDown;

        private Thread _paintThread;

        private int _maxHealth = 100;
        private int _health = 100;
        private float _speed = 600;
        private float _playerX = 0;
        private float _playerY = 0;
        private int _playerSize = 64;

        public bool IsDisposed { get; private set; }
        public bool IsRunning { get; private set; }

        public int PlayerMaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = Math.Max(1, value);
            }
        }
        public int PlayerHealth
        {
            get => _health;
            set
            {
                _health = Math.Max(0, value);
            }
        }
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = Math.Max(0, value);
            }
        }
        public float PlayerX
        {
            get => _playerX;
            set
            {
                if (!IsRunning)
                    return;

                _playerX = Math.Min(Window.ClientSize.Width - 42, Math.Max(42, value));
            }
        }
        public float PlayerY
        {
            get => _playerY;
            set
            {
                if (!IsRunning)
                    return;

                _playerY = Math.Min(Window.ClientSize.Height, Math.Max(0, value));
            }
        }
        public int PlayerSize
        {
            get => _playerSize;
            set
            {
                _playerSize = Math.Max(16, value);
            }
        }

        public Game(Form form)
        {
            Window = form;
            Particles = new ParticleManager(this);
            Entities = new EntityManager(this);

            _keysDown = new HashSet<Keys>();

            init();
        }

        public void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
        }

        public void Reset()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            PlayerX = Window.ClientSize.Width / 2;
            PlayerY = Window.ClientSize.Height - 32;
            PlayerHealth = PlayerMaxHealth;
            IsRunning = false;

            Entities.Clear();
            Particles.Clear();
        }

        private void init()
        {
            var lastRender = DateTime.Now;

            Window.Paint += (_, e) =>
            {
                var now = DateTime.Now;
                var delta = (now - lastRender).TotalSeconds;
                lastRender = now;

                paint(e.Graphics, (float)delta);
            };
            Window.LostFocus += (_, __) =>
            {
                while (_keysDown.Count > 0)
                {
                    keyUp(this, new KeyEventArgs(_keysDown.First()));
                }
            };
            Window.ControlAdded += (_, e) => prepControl(e.Control);
            Window.Closing += (_, __) => Dispose();

            prepControl(Window);

            foreach (Control c in Window.Controls)
            {
                prepControl(c);
            }

            _paintThread = new Thread(() =>
            {
                while (!IsDisposed && !Window.Disposing && !Window.IsDisposed)
                {
                    Window.BeginInvoke((MethodInvoker)Window.Invalidate);

                    Thread.Sleep(16);
                }
            })
            { IsBackground = true };

            Application.Idle += (_, __) =>
            {
                Window.Invalidate();
            };

            _paintThread.Start();

            Reset();
        }

        private void prepControl(Control c)
        {
            c.KeyDown += keyDown;
            c.KeyUp += keyUp;

            c.ControlRemoved += (_, __) =>
            {
                c.KeyDown -= keyDown;
                c.KeyUp -= keyUp;
            };
        }

        private void keyDown(object o, KeyEventArgs e)
        {
            if (Window.ActiveControl is Control c)
            {
                if (!c.Visible || c.Parent == null || !c.Parent.Visible || c is TextBox && IsRunning)
                {
                    Window.ActiveControl = null;
                    Window.Focus();

                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }

            if (_keysDown.Contains(e.KeyCode))
                return;

            _keysDown.Add(e.KeyCode);

            OnKeyDown?.Invoke(e.KeyCode);
        }

        private void keyUp(object _, KeyEventArgs e)
        {
            if (!_keysDown.Remove(e.KeyCode))
                return;

            OnKeyUp?.Invoke(e.KeyCode);
        }

        private void paint(Graphics g, float delta)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Particles.Move(delta);
            Entities.Move(delta);

            Particles.Paint(g, delta);
            Entities.Paint(g, delta);

            var p = Math.Floor(PlayerHealth / (double)PlayerMaxHealth / 0.25 + 0.5) * 0.25 * 100;

            var icon = "player_";

            if (p > 75)
            {
                icon += 100;
            }
            else if (p > 50)
            {
                icon += 75;
            }
            else if (p >= 25)
            {
                icon += 50;
            }
            else if (p > 0)
            {
                icon += 25;
            }
            else
            {
                icon += 0;
            }

            g.DrawImage(TextureManager.Get("shadow"), PlayerX - PlayerSize / 2f, PlayerY - PlayerSize / 2f, PlayerSize, PlayerSize);
            g.DrawImage(TextureManager.Get(icon), PlayerX - PlayerSize / 2f, PlayerY - PlayerSize / 2f, PlayerSize, PlayerSize);

            OnPaint?.Invoke(g, delta);

            if (IsRunning && PlayerHealth == 0)
            {
                IsRunning = false;

                OnEnd?.Invoke();
            }
        }

        public void Dispose()
        {
            IsDisposed = true;

            _paintThread.Abort();
        }
    }
}
