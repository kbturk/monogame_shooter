using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shooter;

abstract class Entity
{
    protected Texture2D image;
    //the tint of the image. This will also allow us
    //to control the transparency
    protected Color color = Color.White;

    public Vector2 Position, Velocity;
    public float Orientation;
    public float Radius = 20; //used for sircular collision detection
    public bool IsExpired; //true if entity was destroyed

    public Vector2 Size
    {
        get {
            return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
        }
    }

    public abstract void Update();

    public virtual void  Draw(SpriteBatch spriteBatch)
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

class PlayerShip: Entity
{
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

    public override void Update()
    {
        //Ship logic goes here
    }
}
