using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInvaders
{
    public class Entity : IDisposable
    {
        public bool IsAlive { get; set; } = true;

        public int Health { get; set; } = 100;
        public int Size { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float VelX { get; set; }

        public string Icon { get; set; }

        public Action OnTouch { get; set; }

        public Entity(string icon, float posX, float velX = 0)
        {
            PosX = posX;
            PosY = -32;

            VelX = velX;

            Size = 64;

            Icon = icon;
        }

        public virtual void Move(Game game, float delta)
        {
            PosX += VelX * delta;
            PosY += game.Speed * delta;
        }

        public virtual void Paint(Game game, Graphics g, float delta)
        {
            g.DrawImage(TextureManager.Get("shadow"), PosX - Size / 2f, PosY - Size / 2f, Size, Size);
            g.DrawImage(TextureManager.Get(Icon), PosX - Size / 2f, PosY - Size / 2f, Size, Size);

            if (Health <= 0 || PosY - Size / 2f > game.Window.ClientSize.Height)
                IsAlive = false;
        }

        public virtual void Dispose()
        {
            IsAlive = false;
        }
    }
}
