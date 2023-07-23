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
    public static SpriteFont Font { get; private set; }

    public static void Load(ContentManager content) {
        Player = content.Load<Texture2D>("Player");
        Seeker = content.Load<Texture2D>("Seeker");
        Wanderer = content.Load<Texture2D>("Wanderer");
        WandererSquare = content.Load<Texture2D>("WandererSquare");
        WandererDiamond = content.Load<Texture2D>("WandererDiamond");
        Bullet = content.Load<Texture2D>("Bullet");
        Pointer = content.Load<Texture2D>("Pointer");
        Font = content.Load<SpriteFont>("NovaSquare");
    }
}
