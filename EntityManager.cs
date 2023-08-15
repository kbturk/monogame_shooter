using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System;

namespace shooter;

static class EntityManager {
    static List<Entity> entities = new List<Entity>();

    static bool isUpdating;
    static List<Entity> addedEntities = new List<Entity>();
    static List<Enemy> enemies = new List<Enemy>();
    static List<Bullet> bullets = new List<Bullet>();
    static public List<BlackHole> blackHoles = new List<BlackHole>();

    public static int Count { get { return entities.Count; }}

    public static void Add(Entity entity) {
        if (!isUpdating)
            AddEntity(entity);
        else
            addedEntities.Add(entity);
    }

    //breaks out enemies and bullets from other objs
    private static void AddEntity(Entity entity)
    {
        //Console.WriteLine($"Entity Count: Black Holes: {blackHoles.Count} Enemies: {enemies.Count}");
        entities.Add(entity);
        if (entity is Bullet)
            bullets.Add(entity as Bullet);
        else if (entity is Enemy)
            enemies.Add(entity as Enemy);
        else if (entity is BlackHole)
            blackHoles.Add(entity as BlackHole);
    }

    // collision detection
    private static bool IsColliding(Entity a, Entity b)
    {
        float radius = a.Radius + b.Radius;
        return !a.IsExpired && !b.IsExpired &&
            Vector2.DistanceSquared(a.Position, b.Position) <
            radius * radius;
    }


    //resolve all collisions
    //TODO: consider other iter methods, this looks inefficient
    static void HandleCollisions()
    {
        //enemies
        for (int i = 0; i < enemies.Count; i++)
            for (int j = i + 1; j < enemies.Count; j++)
            {
                if (IsColliding(enemies[i], enemies[j]))
                {
                    enemies[i].HandleCollision(enemies[j]);
                    enemies[j].HandleCollision(enemies[i]);
                }
            }

        //bullets and enemies
        for (int i = 0; i < enemies.Count; i++)
            for (int j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(enemies[i], bullets[j]))
                {
                    enemies[i].WasShot();
                    bullets[j].IsExpired = true;
                }
            }

        //players and enemies
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
            {
                PlayerShip.Instance.Kill();
                enemies.ForEach(x => x.WasShot());
                break;
            }
        }

        //black holes
        for (int i = 0; i < blackHoles.Count; i++)
        {
            for (int j = 0; j < enemies.Count; j++)
                if (enemies[j].IsActive && IsColliding(blackHoles[i], enemies[j]))
                    enemies[j].WasShot();

            for (int j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(blackHoles[i], bullets[j]))
                {
                    bullets[j].IsExpired = true;
                    blackHoles[i].WasShot();
                }
            }

            if (IsColliding(PlayerShip.Instance, blackHoles[i]))
            {
                PlayerShip.Instance.Kill();
                break;
            }
        }
    }

    public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
    {
        return entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius);
    }

    public static void Update() {
        isUpdating = true;

        HandleCollisions();

        foreach (var entity in entities)
            entity.Update();

        isUpdating = false;

        foreach (var entity in addedEntities)
            AddEntity(entity);

        addedEntities.Clear();

        //remove expired entities
        bullets.RemoveAll(x => x.IsExpired);
        enemies.RemoveAll(x => x.IsExpired);
        blackHoles.RemoveAll(x => x.IsExpired);
        entities.RemoveAll(x => x.IsExpired);
    }

    public static void Draw(SpriteBatch spriteBatch) {
        foreach (var entity in entities)
            entity.Draw(spriteBatch);
    }
}

