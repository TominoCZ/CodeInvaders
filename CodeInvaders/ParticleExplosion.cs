using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInvaders
{
    public class ParticleExplosion : Particle
    {
        private Pen _pen;
        private float _radius = 64;
        private float _time;

        public ParticleExplosion(float posX, float posY) : base(posX, posY)
        {
            var brush = new SolidBrush(Color.FromArgb(255, 180, 32));
            _pen = new Pen(brush, 4);
        }

        public override void Move(Game game, float delta)
        {
            PosY += VelY * delta * game.Speed;
        }

        public override void Paint(Game game, Graphics g, float delta)
        {
            var tween = (float)Math.Sin(Math.PI / 2 * _time);

            for (int i = 0; i < 12; i++)
            {
                var r = i / 12.0f * Math.PI * 2;
                var cx = PosX + Math.Cos(r) * _radius * tween;
                var cy = PosY - Math.Sin(r) * _radius * tween;

                var size = Math.Max(1, 32 * (1 - tween));

                g.FillEllipse(_pen.Brush, (float)cx - size / 2, (float)cy - size / 2, size, size);
            }

            _time = Math.Min(1, _time + delta * 4);

            if (_time >= 1)
                IsAlive = false;
        }
    }
}
