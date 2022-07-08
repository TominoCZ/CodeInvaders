using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInvaders
{
    public abstract class Particle : IDisposable
    {
        public bool IsAlive { get; set; } = true;
        public bool AlwaysUpdate { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }

        public Particle(float posX, float posY, float velX = 0, float velY = 0)
        {
            PosX = posX;
            PosY = posY;
            VelX = velX;
            VelY = velY;
        }

        public virtual void Move(Game game, float delta)
        {
            PosX += VelX * delta;
            PosY += VelY * delta;
        }

        public abstract void Paint(Game game, Graphics g, float delta);

        public virtual void Dispose()
        {
            IsAlive = false;
        }
    }
}
