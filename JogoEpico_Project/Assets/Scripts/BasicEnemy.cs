using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] GameObject bullet;

    [SerializeField] Transform shootPoint;

    [SerializeField] Transform dirObj;

    private void Awake()
    {
        Invoke("RepeatingShooting", 1);
    }

    void RepeatingShooting()
    {
        Shoot();

        Invoke("RepeatingShooting", 1);
    }

    void Shoot()
    {
        Bullet newBullet = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Bullet>();
        newBullet.dir = (int) dirObj.localScale.x;
        newBullet.speed = 500;
    }
}
