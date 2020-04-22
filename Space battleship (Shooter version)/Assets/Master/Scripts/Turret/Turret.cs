﻿using System.Collections;
using Unity.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    //Configuration parameters
    [Header("Turret parameters :")]
    public int idTurret;               //id de la tourelle
    public float damage;
    public float bulletHealth;          
    public float fireRate;                
    public int nbrBullet;               //number of salve per burst
    public float cooldown;                //time to wait between two burst
    [ReadOnly] public float cooldownTimer;
    public float bulletSpeed;
    public float precisionFactor;         //factor of bullet dispersion
    
    [HideInInspector] public float angleMouse;              //angle de visé de la tourelle par rapport à la sourie
    [HideInInspector] public float angleViseur;             //angle de visé de la tourelle par rapport à son viseur
    [Space]
    [Header("Turret Beam :")]
    public bool isTurretBeam;
    public float beamDuration;            //duration of the beam  
    [Space]
    [Header("Turret with ammunitions :")]   //cooldown is replaced by ammo reloading
    public int actualAmmo;
    public int maxAmmo;            
    [Space]
    [Header("Turret stats :")]
    [ReadOnly] public bool isFiring = false;         //indique si la tourelle est en train de tirer ou non
    [ReadOnly] public bool lockMode = false;         //indique si la tourelle est en mode vérouillage ou 
    [Space]
    //Screen parameters
    [Header("Screen parameters :")]
    [ReadOnly] public float widthUnits = 42.6667f;
    [ReadOnly] public float minX = 0f;
    [ReadOnly] public float maxX = 42.6667f;
    [ReadOnly] public float minY = 0f;
    [ReadOnly] public float maxY = 24f;
    [Space]
    [Header("Associated objects :")]
    public TurretDescritpion descritpion;
    public Beam beam;                     //Only here if its a beam turret

    //Coroutines
    public Coroutine manualFiringCoroutine;                //coroutine de tir manuel de la tourelle 

    //Associated gameobjects
    public GameObject Bullet;                              //type de balle tiré par la tourelle
    public Viseur viseur;

    //Animator
    public Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        //Initiate turrets stats from its description
        if(descritpion != null)
        {
            damage = descritpion.actualDamage;
            bulletHealth = descritpion.actualProjectileHealth;
            nbrBullet = descritpion.actualNbrOfSalve;
            fireRate = descritpion.actualFirerate;
            cooldown = descritpion.actualCooldown;
            bulletSpeed = descritpion.actualSpeed;

            if (descritpion.modifierPrimaryType == ModifierPrimaryType.Beam) //Beam turret
            {
                isTurretBeam = true;
            }

            if(descritpion.actualAmmo > 0) //Turret with ammunitions
            {
                maxAmmo = descritpion.actualAmmo;
                actualAmmo = maxAmmo;
            }
        }

        //Find associated objects
        animator = gameObject.GetComponent<Animator>();
        if(isTurretBeam)
        {
            beam = GetComponentInChildren<Beam>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
    }

    //Fonction qui gère l'intégralité de la routine de tir de la tourelle
    public void Fire()
    {
        CoolDownManager();
        if ((maxAmmo > 0 && actualAmmo > 0) || (maxAmmo == 0 && cooldownTimer == 0))
        {
            //Standard turret
            int key = idTurret + 1;
            if (descritpion.turretSize == TurretSize.Standard && Input.GetKeyDown(key.ToString()) && !isFiring) //on lance le burst de tir
            {
                animator.SetBool("Firing", true);
                animator.SetBool("ReadyToFire", true);
                isFiring = true;
                SetViseurLocation();

                if (isTurretBeam && beam != null) //Beam turret
                {
                    beam.beamLine.enabled = true;
                    beam.beamLine.SetPosition(1, viseur.transform.position);
                    beam.isActive = true;
                    lockMode = true;
                }
                else //Projectile turret
                {
                    manualFiringCoroutine = StartCoroutine(BurstFire());
                }
            }
            //Frontal turret
            if(descritpion.turretSize == TurretSize.Frontal && Input.GetKeyDown("space"))
            {
                animator.SetBool("Firing", true);
                animator.SetBool("ReadyToFire", true);
                isFiring = true;

                if (isTurretBeam && beam != null) //Beam turret
                {
                    beam.beamLine.enabled = true;
                    beam.beamLine.SetPosition(1, viseur.transform.position);
                    beam.isActive = true;
                    lockMode = true;
                }
                else //Projectile turret
                {
                    manualFiringCoroutine = StartCoroutine(FrontalFire());
                }
            }
        }

        //le vérouillage est activé
        else if (isFiring && lockMode)
        {
            FollowViseur();
        }
    }

    //Fonction qui gère le cooldown minimum entre deux rafales
    public void CoolDownManager()
    {
        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if(cooldownTimer < 0)
        {
            if(maxAmmo > 0) //if its a tower with ammunition
            {
                if(actualAmmo < maxAmmo)
                {
                    actualAmmo += nbrBullet;
                    if(actualAmmo != maxAmmo)
                    {
                        cooldownTimer = cooldown;
                    }
                }
                else
                {
                    cooldownTimer = 0;
                }
            }
            else //if its a normal turret
            {
                cooldownTimer = 0;
            }

            animator.SetBool("Firing", false);
            animator.SetBool("ReadyToFire", true);
        }
    }

    IEnumerator FrontalFire()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < nbrBullet; i++)
        {
            Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 0);

            GameObject bullet;
            if (descritpion.turretSize == TurretSize.Frontal)
            {
                bullet = Instantiate(Bullet, bulletPosition, Quaternion.Euler(new Vector3(0, 0, -90)));
            }
            else
            {
                Debug.Log("Wrong size");
                bullet = Instantiate(Bullet, bulletPosition, Quaternion.Euler(new Vector3(0, 0, -90)));
            }

            //Instantiate salve stats
            bullet.GetComponent<Salve>().globalDamage = damage;
            bullet.GetComponent<Salve>().globalHealth = bulletHealth;
            bullet.GetComponent<Salve>().globalbulletSpeed = bulletSpeed;
            yield return new WaitForSeconds(fireRate); // wait till the next round
        }
        animator.SetBool("Firing", false);
        animator.SetBool("ReadyToFire", false);
        isFiring = false;
        cooldownTimer = cooldown;
    }

    IEnumerator BurstFire()
    {
        yield return new WaitForSeconds(0.1f);
        // rate of fire in weapons is in rounds per minute (RPM), therefore we should calculate how much time passes before firing a new round in the same burst.
        for (int i = 0; i < nbrBullet; i++)
        {
            //Set bullet position and turret rotation according to its angle
            Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 0);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleViseur - 90));

            //Create salve gameobject
            GameObject bullet;
            if (descritpion.turretSize == TurretSize.Standard)
            {
                bullet = Instantiate(Bullet, bulletPosition, Quaternion.Euler(new Vector3(0, 0, (angleViseur - 90) + Random.Range(-precisionFactor, precisionFactor))));
            }
            else
            {
                Debug.Log("Wrong size");
                bullet = Instantiate(Bullet, bulletPosition, Quaternion.Euler(new Vector3(0, 0, -90)));
            }

            //Instantiate salve stats
            bullet.GetComponent<Salve>().globalDamage = damage;
            bullet.GetComponent<Salve>().globalHealth = bulletHealth;
            bullet.GetComponent<Salve>().globalbulletSpeed = bulletSpeed;
            yield return new WaitForSeconds(fireRate); // wait till the next round
        }

        if(maxAmmo > 0) //Turret with ammunitions
        {
            actualAmmo -= nbrBullet;
            animator.SetBool("Firing", false);
            animator.SetBool("ReadyToFire", false);

            if(cooldownTimer == 0)
            {
                cooldownTimer = cooldown;
            }
        }
        else //Normal turret
        {
            animator.SetBool("Firing", false);
            animator.SetBool("ReadyToFire", false);
            cooldownTimer = cooldown;
        }
        
        isFiring = false;
    }

    //Fonction qui permet à la tourelle active de suivre la sourie
    public void FollowMouse()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - pos;
        angleMouse = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angleMouse-90, Vector3.forward);
    }

    //Fonction qui permet à la tourelle de rester lock sur le viseur (même si le vaisseau bouge)
    public void FollowViseur()
    {
        Vector3 dif = viseur.transform.position - transform.position;
        angleViseur = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleViseur - 90);
    }

    public float GetCooldownFactor()
    {
        return cooldownTimer / cooldown;
    }

    public void SetViseurLocation()
    {
        float mousePosInUnitX = Input.mousePosition.x / Screen.width * widthUnits; // on récupère la position de la souris sur l'écran en units
        float mousePosInUnitY = Input.mousePosition.y / Screen.width * widthUnits;
        Vector2 viseurPos = new Vector2(transform.position.x, transform.position.y);
        viseurPos.x = Mathf.Clamp(mousePosInUnitX, minX, maxX);
        viseurPos.y = Mathf.Clamp(mousePosInUnitY, minY, maxY);
        viseur.transform.position = viseurPos;

        FollowViseur();
    }
}
