using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySalve : MonoBehaviour
{
    public EnnemyProjectile[] ennemyProjectiles;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ennemyProjectiles.Length; i++)
        {
            ennemyProjectiles[i].transform.GetComponent<Rigidbody2D>().AddForce(ennemyProjectiles[i].transform.up * ennemyProjectiles[i].GetBulletSpeed());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsProjectilesAllDestroyed())
        {
            Destroy(gameObject);
        }
    }

    public bool IsProjectilesAllDestroyed()
    {
        int nbrProjectileDestroyed = 0;
        for (int i = 0; i < ennemyProjectiles.Length; i++)
        {
            if (ennemyProjectiles[i].isDestroyed)
            {
                nbrProjectileDestroyed++;
            }
            if (ennemyProjectiles.Length == nbrProjectileDestroyed)
            {
                return true;
            }
        }
        return false;
    }
}
