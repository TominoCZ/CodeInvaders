using System;
using System.Collections.Generic;
using System.Drawing;

namespace CodeInvaders
{
    public class EntityManager
    {
        private List<Entity> _entities;
        private Queue<Entity> _disposing;
        private Game _game;

        public EntityManager(Game game)
        {
            _game = game;

            _entities = new List<Entity>();
            _disposing = new Queue<Entity>();
        }

        public void Move(float delta)
        {
            foreach (Entity e in _entities)
            {
                if (_game.IsRunning)
                    e.Move(_game, delta);

                var sx = Math.Abs(e.PosX - _game.PlayerX);
                var sy = Math.Abs(e.PosY - _game.PlayerY);

                var dist = Math.Sqrt(sx * sx + sy * sy);

                if (dist < (_game.PlayerSize + e.Size) / 2)
                {
                    e.OnTouch?.Invoke();
                }
            }
        }

        public void Paint(Graphics g, float delta)
        {
            foreach (Entity e in _entities)
            {
                e.Paint(_game, g, delta);

                if (!e.IsAlive)
                    _disposing.Enqueue(e);
            }

            while (_disposing.Count > 0)
            {
                var particle = _disposing.Dequeue();
                particle.Dispose();

                _entities.Remove(particle);
            }
        }

        public void Add(Entity p)
        {
            _entities.Add(p);
        }

        public void Clear()
        {
            foreach (var entity in _entities)
            {
                entity.Dispose();
            }
            while (_disposing.Count > 0)
            {
                _disposing.Dequeue().Dispose();
            }

            _entities.Clear();
        }
    }
}
