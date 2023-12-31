using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace shooter;

static class EnemySpawner
{
    static Random rand = new Random();
    static float inverseSpawnChance = 120;//originally 60
    static float inverseBlackHoleChance = 120;//originally 60

    public static void Update()
    {
        if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
        {
            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateWandererDiamond(GetSpawnPosition()));

            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateWandererSquare(GetSpawnPosition()));

            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));

            if (EntityManager.blackHoles.Count < 2 && rand.Next((int)inverseBlackHoleChance) == 0)
            {
                EntityManager.Add(new BlackHole(GetSpawnPosition()));
            }
        }

        //slowly increase the spawn rate as time progresses
        if (inverseSpawnChance > 20)
            inverseSpawnChance -= 0.005f;
    }

    private static Vector2 GetSpawnPosition()
    {
        Vector2 pos;

        //TODO: set distance based on viewport size

        do
        {
            pos = new Vector2(rand.Next((int)GameRoot.ScreenSize.X), rand.Next((int)GameRoot.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

        return pos;
    }

    public static void Reset()
    {
        inverseSpawnChance = 60;
    }
}
