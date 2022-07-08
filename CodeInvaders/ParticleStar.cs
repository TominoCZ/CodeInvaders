using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInvaders
{
    public class ParticleStar : Particle
    {
        private static Random _random = new Random();

        private Pen _pen;
        private float _width;
        private float _length;

        public ParticleStar(float posX) : base(posX, 0)
        {
            var r = (float)_random.NextDouble();
            var mult = r * -0.25f - 0.1f;
            var val = (int)(255 * Math.Pow(1 + mult, 4));

            VelY = 1 + mult;
            VelX = (0.25f - r * 0.5f) * 4 * 2;

            _width = (float)Math.Pow(1 + mult, 4) * 2;
            _length = _width * 150;

            var brush = new SolidBrush(Color.FromArgb(val, val, val));
            _pen = new Pen(brush, _width);
        }

        public override void Move(Game game, float delta)
        {
            PosY += VelY * delta * game.Speed;
        }

        public override void Paint(Game game, Graphics g, float delta)
        {
            g.DrawLine(_pen, PosX - VelX, PosY - _length, PosX, PosY);

            if (PosY - _length > game.Window.ClientSize.Height)
                IsAlive = false;
        }
    }
}
