using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySalve : MonoBehaviour
{
    public EnnemyProjectile[] ennemyProjectiles;
    public int nbrProjectile;
    public int nbrProjectileDestroyed = 0;

    // Start is called before the first frame update
    void Start()
    {
        nbrProjectile = ennemyProjectiles.Length;
        for (int i = 0; i < ennemyProjectiles.Length; i++)
        {
            ennemyProjectiles[i].transform.GetComponent<Rigidbody2D>().AddForce(ennemyProjectiles[i].transform.up * ennemyProjectiles[i].bulletSpeed);
        }
    }

    //Function called by projectile when its destroyed, destroy salve object if all projectiles are destroyed
    public void ImDestroyed()
    {
        nbrProjectileDestroyed++;
        if (nbrProjectile == nbrProjectileDestroyed)
        {
            Destroy(gameObject);
        }
    }
}
