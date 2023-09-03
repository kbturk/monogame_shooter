using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace shooter;

static class Art {
    public static Texture2D Player { get; private set; }
    public static Texture2D Seeker { get; private set; }
    public static Texture2D Wanderer { get; private set; }
    public static Texture2D WandererSquare { get; private set; }
    public static Texture2D WandererDiamond { get; private set; }
    public static Texture2D Bullet { get; private set; }
    public static Texture2D Pointer { get; private set; }
    public static Texture2D BlackHole { get; private set; }
    public static Texture2D LineParticle { get; private set; }
    public static SpriteFont Font { get; private set; }

    public static void Load(ContentManager content) {
        Player = content.Load<Texture2D>("Art/Player");
        Seeker = content.Load<Texture2D>("Art/Seeker");
        Wanderer = content.Load<Texture2D>("Art/Wanderer");
        WandererSquare = content.Load<Texture2D>("Art/WandererSquare");
        WandererDiamond = content.Load<Texture2D>("Art/WandererDiamond");
        Bullet = content.Load<Texture2D>("Art/Bullet");
        Pointer = content.Load<Texture2D>("Art/Pointer");
        BlackHole = content.Load<Texture2D>("Art/Black Hole");
        LineParticle = content.Load<Texture2D>("Art/Laser");
        Font = content.Load<SpriteFont>("NovaSquare");
    }
}
