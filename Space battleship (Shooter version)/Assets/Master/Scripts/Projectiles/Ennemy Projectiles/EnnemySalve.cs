using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySalve : MonoBehaviour
{
    public EnnemyProjectile[] ennemyProjectiles;
    public int nbrProjectile;
    public int nbrProjectileDestroyed = 0;

    // Start is called before the first frame update
    void OnValidate()
    {
        nbrProjectile = ennemyProjectiles.Length;
    }

    private void Update()
    {
        for (int i = 0; i < ennemyProjectiles.Length; i++)
        {
            if(ennemyProjectiles[i] != null)
            {
                ennemyProjectiles[i].transform.position += -1 * ennemyProjectiles[i].transform.up * Time.deltaTime * (ennemyProjectiles[i].bulletSpeed / 50);
            }
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
