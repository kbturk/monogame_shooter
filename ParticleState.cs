using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace shooter {

    public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

    public struct ParticleState
    {
        public Vector2 Velocity;
        public ParticleType Type;
        public float LengthMultiplier;


        public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle)
        {
            var vel = particle.State.Velocity;

            particle.Position += vel;
            particle.Orientation = vel.ToAngle();

            float speed = vel.Length();
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;

            particle.Color.A = (byte)(255 * alpha);

            //denormalize floats cause significant performance issues
            if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.0000000000001f)
                vel = Vector2.Zero;

            vel *= 0.97f; //particle velocity decay
            particle.State.Velocity = vel;
        }
    }
}
