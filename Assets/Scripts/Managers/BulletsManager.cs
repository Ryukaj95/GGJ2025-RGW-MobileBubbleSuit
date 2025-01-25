using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BulletsManager : Singleton<BulletsManager>
{
    // Il sistema di proiettili deve permettere:

    [SerializeField] public Transform bulletContainer;

    public static List<Bullet> enemyBulletsOnScreen = new List<Bullet>();

    // Creare i proiettili nel BulletManager
    // BulletManager.Instance.CreateEnemyBullet() -> Crea proiettile, popola con i parametri passati (prefabProiettile, posizione, direction, speed)
    // BulletManager.Instance.CreateFriendlyBullet()
    public Bullet CreateEnemyBullet(Vector2 spawnPoint, Vector2 direction, Bullet bullet)
    {
        Bullet newBullet = Instantiate(bullet, spawnPoint, Quaternion.identity);
        newBullet.SetDirection(direction);
        newBullet.SetIsFriendlyToPlayer(false);
        enemyBulletsOnScreen.Add(newBullet);
        newBullet.transform.parent = bulletContainer;
        return newBullet;
    }

    public Bullet ShootBullet(Vector2 spawnPoint, Vector2 direction, Bullet bullet, bool isPlayerBullet)
    {
        if (isPlayerBullet) return CreateFriendlyBullet(spawnPoint, direction, bullet);
        else return CreateEnemyBullet(spawnPoint, direction, bullet);
    }

    public Bullet CreateFriendlyBullet(Vector2 spawnPoint, Vector2 direction, Bullet bullet)
    {
        Bullet newBullet = Instantiate(bullet, spawnPoint, Quaternion.identity);
        newBullet.SetDirection(direction);
        newBullet.SetIsFriendlyToPlayer(true);
        newBullet.transform.parent = bulletContainer;
        return newBullet;
    }

    public void RemoveBulletFromList(string bulletId)
    {
        enemyBulletsOnScreen.RemoveAt(enemyBulletsOnScreen.FindIndex(b => b.bulletId == bulletId));
    }
}
