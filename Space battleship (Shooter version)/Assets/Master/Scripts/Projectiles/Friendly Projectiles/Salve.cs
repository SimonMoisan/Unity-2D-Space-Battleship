using Unity.Collections;
using UnityEngine;

public class Salve : MonoBehaviour
{
    public Projectile[] projectiles;
    public Vector2[] projectilesInitialPosition;
    public Quaternion[] projectilesInitialRotation;
    [ReadOnly] public float globalDamage;
    [ReadOnly] public float globalHealth;
    [ReadOnly] public float globalBulletSpeed;

    [ReadOnly] public int nbrProjectile;
    [ReadOnly] int nbrProjectileDestroyed = 0;


    private void OnValidate()
    {
        projectilesInitialPosition = new Vector2[projectiles.Length];
        projectilesInitialRotation = new Quaternion[projectiles.Length];

        for (int i = 0; i < projectiles.Length; i++)
        {
            projectilesInitialPosition[i] = projectiles[i].transform.position;
            projectilesInitialRotation[i] = projectiles[i].transform.rotation;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        nbrProjectile = projectiles.Length;
        //Destroy(gameObject, 15);
    }

    private void Update()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (projectiles[i] != null && projectiles[i].GetComponent<FriendlyProjectile>() != null)
            {
                projectiles[i].transform.position += projectiles[i].transform.up * Time.deltaTime * (projectiles[i].bulletSpeed / 50);
            }
            if (projectiles[i] != null && projectiles[i].GetComponent<EnnemyProjectile>() != null)
            {
                projectiles[i].transform.position += -1 * projectiles[i].transform.up * Time.deltaTime * (projectiles[i].bulletSpeed / 50);
            }
        }
    }

    //Function called by projectile when its destroyed, destroy salve object if all projectiles are destroyed
    public void ImDestroyed()
    {
        nbrProjectileDestroyed++;
        if (nbrProjectile == nbrProjectileDestroyed)
        {
            //Destroy(gameObject);
            destroy();
        }
    }

    private void destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Invoke("destroy", 15f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
