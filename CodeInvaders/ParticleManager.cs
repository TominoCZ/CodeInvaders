using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CodeInvaders
{
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
