﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Console = System.Console;

namespace shooter;

public class GameRoot : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static GameRoot Instance { get; private set; }
    public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
    public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
    public static GameTime GameTime { get; private set; }

    public GameRoot()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        Instance = this;
    }

    private void DrawRightAlignedString(string text, float y)
    {
        var textWidth = Art.Font.MeasureString(text).X;
        _spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth *= 2;
        _graphics.PreferredBackBufferHeight *= 2;
        _graphics.ApplyChanges();

        base.Initialize();
        EntityManager.Add(PlayerShip.Instance);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Art.Load(Content);
        Console.WriteLine($"Viewport Size: {ScreenSize}");
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        GameTime = gameTime;

        // TODO: Add your update logic here
        Input.Update();
        EntityManager.Update();
        EnemySpawner.Update();
        PlayerStatus.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
        EntityManager.Draw(_spriteBatch);

        _spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
        DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
        DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);

        //draw gameover
        if (PlayerStatus.IsGameOver)
        {
            string text = $"Game Over\nYour Score: {PlayerStatus.Score}\nHigh Score: {PlayerStatus.HighScore}";

            Vector2 textSize = Art.Font.MeasureString(text);
            _spriteBatch.DrawString(Art.Font, text, ScreenSize / 2- textSize / 2, Color.White);
        }

        //draw the mouse cursor
        _spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
