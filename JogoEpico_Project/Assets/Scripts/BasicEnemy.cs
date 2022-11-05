using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPoint;
    [SerializeField] Transform dirObj;
    [SerializeField] Animator animator;

    private void Awake()
    {
        Invoke("RepeatingShooting", 1);
    }

    // Health system
    bool legHurted = false;

    public void HeadShooted(OnTriggerDelegation delegation)
    {
        if (delegation.Other.GetComponent<Bullet>() == null) return;

        Destroy(gameObject);
    }

    public void LegShooted(OnTriggerDelegation delegation)
    {
        if (delegation.Other.GetComponent<Bullet>() == null) return;

        legHurted = true;
        CancelInvoke();
        animator.SetTrigger("LegShooted");
    }

    void RepeatingShooting()
    {
        Shoot();

        // Si no ha sido disparado en la pierna, seguir disparando
        if (!legHurted)
            Invoke("RepeatingShooting", 1);
    }
    void Shoot()
    {
        Bullet newBullet = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Bullet>();
        newBullet.dir = (int)dirObj.localScale.x;
        newBullet.speed = 2000;
        newBullet.gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
    }
}
