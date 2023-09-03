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
            var pos = particle.Position;
            int width = (int)GameRoot.ScreenSize.X;
            int height = (int)GameRoot.ScreenSize.Y;

            particle.Position += vel;
            particle.Orientation = vel.ToAngle();

            if (pos.X < 0)
                vel.X = Math.Abs(vel.X);
            else if (pos.X > width)
                vel.X = -Math.Abs(vel.X);

            if (pos.Y < 0)
                vel.Y = Math.Abs(vel.Y);
            else if (pos.Y > height)
                vel.Y = -Math.Abs(vel.Y);
            
            float speed = vel.Length();
            float alpha = Math.Min(1,
                    Math.Min(particle.PercentLife * 2, speed * 1f));
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
