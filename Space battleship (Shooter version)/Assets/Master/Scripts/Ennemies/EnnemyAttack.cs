using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum attackType
{
    normal,
    spe_shield,
    spe_hull
}

public class EnnemyAttack : MonoBehaviour
{
    //Ennemy attack stats
    public string targetingType;           //Targeting type of the attack : Normal, Static
    public string attackType;

    public float damage;
    public float bulletSpeed;           //vitesse des projectiles tirés par l'ennemi
    public float fireRate;              //cadence de tir de l'ennemi
    public int nbrShots;                //nombre de tir par rafale
    private float angleViseur;           //angle de visé par rapport à la cible

    public float cooldown;              //time to wait between two burst
    public float cooldownTimer;
    public bool cooldownIsActive;       //indicate if the cooldown is running or not

    //Attack states
    public bool isFiring;          
    public bool isReadyToFire;

    //Associated objects
    public Ennemy ennemy;
    public GameObject bulletPrefab;
    public PlayerDetector playerDetector;

    //Coroutines
    public Coroutine firingCoroutine;     //coroutine de tir 

    // Start is called before the first frame update
    void Awake()
    {
        cooldownTimer = cooldown;
        ennemy = GetComponentInParent<Ennemy>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
    }

    //Fonction qui donne l'angle de visé
    public void Targeter()
    {
        if (playerDetector.target != null)
        {
            Vector3 dif = playerDetector.target.transform.position - transform.position;
            angleViseur = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        }
    }

    //Fonction qui gère le cooldown minimum entre deux rafales
    public void CoolDownManager()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if (cooldownTimer < 0)
        {
            cooldownTimer = 0;
            isReadyToFire = true;
        }
    }

    //Fonction qui gère l'intégralité de la routine de tir de la tourelle
    public void Fire()
    {
        isFiring = true;
        firingCoroutine = StartCoroutine(BurstFire());

        cooldownTimer = cooldown; //Cooldown is launch
        
        isReadyToFire = false;
    }

    public IEnumerator BurstFire()
    {
        yield return new WaitForSeconds(0.1f);
        // rate of fire in weapons is in rounds per minute (RPM), therefore we should calculate how much time passes before firing a new round in the same burst.

        if (targetingType == "Normal")
        {
            for (int i = 0; i < nbrShots; i++)
            {
                Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 1);
                GameObject bullet = Instantiate
                    (bulletPrefab,
                    bulletPosition,
                    Quaternion.Euler(new Vector3(0, 0, angleViseur - 90)));

                yield return new WaitForSeconds(fireRate); // wait till the next round
            }
        }
        else if (targetingType == "Static")
        {
            for (int i = 0; i < nbrShots; i++)
            {
                Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 1);
                GameObject bullet = Instantiate
                    (bulletPrefab,
                    bulletPosition,
                    Quaternion.Euler(new Vector3(0, 0, 90)));

                yield return new WaitForSeconds(fireRate); // wait till the next round
            }
        }
        else
        {
            Debug.Log("Attacj type invalid");
        }

        isFiring = false;
        cooldownTimer = cooldown;
    }
}
