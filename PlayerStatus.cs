using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace shooter;

static class PlayerStatus
{
    private const float multiplierExpiryTime = 0.8f;
    private const int maxMultiplier = 20;
    private const string highScoreFilename = "highscore.txt";

    public static int Lives { get; private set; }
    public static int Score { get; private set; }
    public static int HighScore { get; private set; }
    public static int Multiplier { get; private set; }
    public static bool IsGameOver { get { return Lives == 0; } }

    private static float multiplierTimeLeft;
    private static int scoreForExtraLife;

    static PlayerStatus()
    {
        HighScore = LoadHighScore();
        Reset();
    }

    public static void Reset()
    {
        if (Score > HighScore)
            SaveHighScore(HighScore = Score);

        Score = 0;
        Multiplier = 1;
        Lives = 4;
        scoreForExtraLife = 2000;
        multiplierTimeLeft = 0;
    }

    public static void Update()
    {
        if (Multiplier > 1)
        {
            if ((multiplierTimeLeft -= (float)GameRoot.GameTime.ElapsedGameTime.TotalSeconds) <= 0)
            {
                multiplierTimeLeft = multiplierExpiryTime;
                ResetMultiplier();
            }
        }
    }

    public static void AddPoints(int basePoints)
    {
        if (PlayerShip.Instance.IsDead)
            return;

        Score += basePoints * Multiplier;
        while (Score >= scoreForExtraLife)
        {
            scoreForExtraLife += 2000;
            Lives++;
        }
    }

    public static void IncreaseMultiplier()
    {
        if (PlayerShip.Instance.IsDead)
            return;

        multiplierTimeLeft = multiplierExpiryTime;
        if (Multiplier < maxMultiplier)
            Multiplier++;
    }

    public static void ResetMultiplier()
    {
        Multiplier = 1;
    }

    public static void RemoveLife()
    {
        if (--Lives <= 0)
            Reset();
    }

    private static int LoadHighScore()
    {
        // return the saved high score if possible
        // TODO: input name or save date

        int score;
        return File.Exists(highScoreFilename) &&
            int.TryParse(File.ReadAllText(highScoreFilename),
                    out score) ? score: 0;
    }

    private static void SaveHighScore(int score)
    {
        File.WriteAllText(highScoreFilename, score.ToString());
    }
}
