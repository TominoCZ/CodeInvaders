using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
                    e.OnHit?.Invoke();
                    e.IsAlive = false;
                }

                if (!e.IsAlive)
                    _disposing.Enqueue(e);
            }
        }

        public void CheckCollision()
        {
        }

        public void Paint(Graphics g, float delta)
        {
            foreach (Entity e in _entities)
            {
                e.Paint(_game, g, delta);
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

    public class ParticleManager
    {
        private List<Particle> _particles;
        private Queue<Particle> _disposing;
        private Game _game;

        public ParticleManager(Game game)
        {
            _game = game;

            _particles = new List<Particle>();
            _disposing = new Queue<Particle>();
        }

        public void Move(float delta)
        {
            foreach (Particle p in _particles)
            {
                if (_game.IsRunning || p.AlwaysUpdate)
                    p.Move(_game, delta);

                if (!p.IsAlive)
                    _disposing.Enqueue(p);
            }
        }

        public void Paint(Graphics g, float delta)
        {
            foreach (Particle p in _particles)
            {
                p.Paint(_game, g, delta);
            }

            while (_disposing.Count > 0)
            {
                var particle = _disposing.Dequeue();
                particle.Dispose();

                _particles.Remove(particle);
            }
        }

        public void Add(Particle p)
        {
            _particles.Add(p);
        }

        public void Clear()
        {
            foreach (var particle in _particles)
            {
                particle.Dispose();
            }
            while (_disposing.Count > 0)
            {
                _disposing.Dequeue().Dispose();
            }

            _particles.Clear();
        }
    }
}
