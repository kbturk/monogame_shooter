using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Quaternion = System.Numerics.Quaternion;
using System.Collections.Generic;

namespace shooter;

public abstract class Entity
{
    protected Texture2D image;
    //the tint of the image. This will also allow us
    //to control the transparency
    protected Color color = Color.White;

    public Vector2 Position, Velocity, Acceleration;
    public float Orientation;
    public float Radius = 20; //used for sircular collision detection
    public bool IsExpired = false; //true if entity was destroyed

    public Vector2 Size
    {
        get {
            return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
        }
    }

    public abstract void Update();

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
                image,
                Position,
                null,
                color,
                Orientation,
                Size / 2f,
                1f,
                SpriteEffects.None,
                0
                );
    }
}

public class PlayerShip: Entity
{
    const int cooldownFrames = 6;
    int cooldownRemaining = 0;
    int framesUntilRespawn = 0;
    public bool IsDead { get { return framesUntilRespawn > 0; }}
    static Random rand = new Random();


    private static PlayerShip instance;

    public static PlayerShip Instance
    {
        get {
            if (instance == null)
                instance = new PlayerShip();

            return instance;
        }
    }

    private PlayerShip()
    {
        image = Art.Player;
        Position = GameRoot.ScreenSize / 2;
        Radius = 10;
    }

    public void Kill()
    {
        PlayerStatus.RemoveLife();
        framesUntilRespawn = PlayerStatus.IsGameOver ? 300 : 120;

        EnemySpawner.Reset();
    }

    //TODO: build in acceleration/deceleration for fun
    public override void Update()
    {
        //check for death
        if (IsDead)
        {
            framesUntilRespawn--;
            return;
        }

        const float speed = 8;
        Velocity = speed * Input.GetMovementDirection();
        Position += Velocity;
        Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();//(float)Math.Atan2(Velocity.Y, Velocity.X);

        //bullet logic
        var aim = Input.GetAimDirection();
        if (aim.LengthSquared() > 0 && cooldownRemaining <=0)
        {
            cooldownRemaining = cooldownFrames;
            float aimAngle = aim.ToAngle();
            Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

            float randomSpread = rand.NextFloat(-0.04f, 0.0f) + rand.NextFloat(-0.04f, 0.04f);
            Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);

            Vector2 offset = Vector2.Transform(new Vector2(25, -8), aimQuat);
            EntityManager.Add(new Bullet(Position + offset, vel));

            offset = Vector2.Transform(new Vector2(25, 8), aimQuat);
            EntityManager.Add(new Bullet(Position + offset, vel));
        }

        if (cooldownRemaining > 0)
            cooldownRemaining--;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsDead)
            base.Draw(spriteBatch);
    }
}

public class Bullet : Entity {

    public Bullet(Vector2 position, Vector2 velocity)
    {
        image = Art.Bullet;
        Position = position;
        Velocity = velocity;
        Orientation = Velocity.ToAngle();
        Radius = 8;
    }

    public override void Update()
    {
        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();

        Position += Velocity;

        if (!GameRoot.Viewport.Bounds.Contains(Position.ToPoint()))
            IsExpired = true;
    }
}

public class Enemy: Entity
{
    private int timeUntilStart = 60;
    public bool IsActive { get { return timeUntilStart <=0; } }
    public int PointValue { get; private set; }
    private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
    private Random rand = new Random();

    public Enemy(Texture2D image, Vector2 position)
    {
        this.image = image;
        Position = position;
        Radius = image.Width/ 2f;
        color = Color.Transparent;
    }
    
    // collision resolution w other enemy
    public void HandleCollision(Enemy other)
    {
        var d = Position - other.Position;
        Velocity += 10 * d / (d.LengthSquared() + 1);
    }

    // Enemy Types
    public static Enemy CreateSeeker(Vector2 position)
    {
        var enemy = new Enemy(Art.Seeker, position);
        enemy.AddBehavior(enemy.FollowPlayer());
        enemy.PointValue = 2;

        return enemy;
    }

    public static Enemy CreateWanderer(Vector2 position)
    {
        var enemy = new Enemy(Art.Wanderer, position);
        enemy.AddBehavior(enemy.MoveRandomly());
        enemy.PointValue = 1;
        
        return enemy;
    }

    //Behaviours
    private IEnumerable<int> FollowPlayer(float acceleration = 1f)
    {
        while (true)
        {
            Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
            if (Velocity != Vector2.Zero)
                Orientation = Velocity.ToAngle();

            yield return 0;
        }
    }

    private IEnumerable<int> MoveInSquare()
    {
        const int framesPerSide = 30;
        float speed = 1;
        while (true)
        {
            //move right by speed each frame
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = Vector2.UnitX * speed;
                yield return 0;
            }

            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = Vector2.UnitY * speed;
                yield return 0;
            }

            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity =  - Vector2.UnitX * speed;
                yield return 0;
            }

            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity =  - Vector2.UnitY * speed;
                yield return 0;
            }
        }
    }

    private IEnumerable<int> MoveInDiamond()
    {
        const int framesPerSide = 30;
        float speed = 1f;
        while (true)
        {
            //move right & down by speed each frame
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = new Vector2(1f, 1f) * speed;
                yield return 0;
            }

            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = new Vector2(-1f, 1f) * speed;
                yield return 0;
            }

            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity =  new Vector2(-1f, -1f) * speed;
                yield return 0;
            }

            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity =  new Vector2(1f, -1f) * speed;
                yield return 0;
            }
        }
    }

    IEnumerable<int> MoveRandomly()
    {
        float direction = rand.NextFloat(0, MathHelper.TwoPi);

        while (true)
        {
            direction += rand.NextFloat(-0.1f, 0.1f);
            direction = MathHelper.WrapAngle(direction);

            for (int i = 0; i < 6; i++)
            {
                Velocity += MathUtil.FromPolar(direction, 0.4f);
                Orientation -= 0.05f;

                var bounds = GameRoot.Viewport.Bounds;
                bounds.Inflate(-image.Width, -image.Height);

                // if the enemy is outside the bounds
                // move it away from the edge
                if (!bounds.Contains(Position.ToPoint()))
                    direction =
                        (GameRoot.ScreenSize / 2 - Position)
                        .ToAngle() + rand.NextFloat(
                                -MathHelper.PiOver2,
                                MathHelper.PiOver2);

                yield return 0;
            }
        }
    }

    private void AddBehavior(IEnumerable<int> behaviour)
    {
        behaviours.Add(behaviour.GetEnumerator());
    }

    private void ApplyBehaviours()
    {
        for (int i = 0; i < behaviours.Count; i++)
        {
            if (!behaviours[i].MoveNext())
                behaviours.RemoveAt(i--);
        }
    }

    public override void Update()
    {
        if (timeUntilStart <= 0)
        {
            ApplyBehaviours();
        }
        else
        {
            timeUntilStart--;
            color = Color.White * (1 - timeUntilStart / 60f);
        }

        Position += Velocity;
        Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size /2);

        Velocity *= 0.8f;
    }

    public void WasShot()
    {
        IsExpired = true;
        PlayerStatus.AddPoints(PointValue);
        PlayerStatus.IncreaseMultiplier();
    }
}
