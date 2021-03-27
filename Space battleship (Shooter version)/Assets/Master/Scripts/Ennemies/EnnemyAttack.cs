using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { normal, spe_shield, spe_hull }
public enum TargetingType { circle, front }

public class EnnemyAttack : MonoBehaviour
{
    //Ennemy attack stats
    [Header("Attack stats")]
    public TargetingType targetingType;           //Targeting type of the attack : Normal, Static
    public AttackType attackType;
    public float globalDamage;
    public float globalHealth;
    public float globalBulletSpeed;           //vitesse des projectiles tirés par l'ennemi
    public float fireRate;              //cadence de tir de l'ennemi
    public int nbrShots;                //nombre de tir par rafale
    private float angleViseur;           //angle de visé par rapport à la cible
    public float cooldown;              //time to wait between two burst
    public float cooldownTimer;
    public bool cooldownIsActive;       //indicate if the cooldown is running or not
    public bool stopMovement;
    public float precisionFactor;
    [Space]
    [Header("Turret Beam :")]
    public bool isTurretBeam;
    public float beamDuration;            //duration of the beam 
    public EnnemyBeam beam;
    public Transform impactDirection;
    public Transform impact;
    private int layerMask = 1 << 10;
    [Space]
    //Attack states
    [Header("Attack states")]
    public bool isFiring;          
    public bool isReadyToFire;
    [Space]
    //Associated objects
    [Header("Associated objects")]
    public Ennemy ennemy;
    public GameObject bulletPrefab;
    public PlayerDetector playerDetector;
    public ProjectilePool pooler;
    public SoundEffectsGenerator soundEffectsGenerator;
    

    //Coroutines
    public Coroutine firingCoroutine;     //coroutine de tir 

    // Start is called before the first frame update
    void Awake()
    {
        cooldownTimer = cooldown;
        ennemy = GetComponentInParent<Ennemy>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
        soundEffectsGenerator = GetComponentInChildren<SoundEffectsGenerator>();

        //Find pooler
        for (int i = 0; i < GameManagerScript.current.poolers.Length; i++)
        {
            if (GameManagerScript.current.poolers[i].salvePrefab == bulletPrefab.GetComponent<Salve>())
            {
                pooler = GameManagerScript.current.poolers[i];
            }
        }

        if (isTurretBeam && beam != null)
        {
            beam.damage = globalDamage;
        }
    }

    private void Update()
    {
        if (isTurretBeam)
        {
            // Cast a ray straight down.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, impactDirection.localPosition, Mathf.Infinity, layerMask); 
            if(hit.collider != null)
            {
                Vector3 local_point = transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point);
                impact.localPosition = local_point;
                if (hit.collider.GetComponent<Shield>() != null)
                {
                    beam.shieldToDamage = hit.collider.GetComponent<Shield>();
                    beam.battleshipToDamage = null;
                }
                else if (hit.collider.GetComponent<Battleship>() != null)
                {
                    beam.battleshipToDamage = hit.collider.GetComponent<Battleship>();
                    beam.shieldToDamage = null;
                }
                else
                {
                    beam.shieldToDamage = null;
                    beam.battleshipToDamage = null;
                }
            }
            else
            {
                impact.localPosition = impactDirection.localPosition;
            }
        }
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
        if (cooldownTimer <= 0)
        {
            cooldownTimer = 0;
            isReadyToFire = true;
        }
    }

    //Fonction qui gère l'intégralité de la routine de tir de la tourelle
    public void Fire()
    {
        if (isTurretBeam && beam != null) //Beam turret
        {
            beam.beamLine.enabled = true;
            beam.beamLine.SetPosition(1, impact.position);
            beam.isActive = true;
        }
        else //Projectile turret
        {
            firingCoroutine = StartCoroutine(BurstFire());
        }

        isFiring = true;
        cooldownTimer = cooldown; //Cooldown is launch
        isReadyToFire = false;
    }

    public IEnumerator BurstFire()
    {
        if (stopMovement)
        {
            ennemy.isMoving = false;
        }

        yield return new WaitForSeconds(0.1f);
        // rate of fire in weapons is in rounds per minute (RPM), therefore we should calculate how much time passes before firing a new round in the same burst.

        //Looping sound effect
        if (soundEffectsGenerator.isLooping)
        {
            soundEffectsGenerator.playDefaultSoundEffect();
        }

        for (int i = 0; i < nbrShots; i++)
        {
            //Mono sound effect
            if (soundEffectsGenerator.isLooping)
            {
                soundEffectsGenerator.playDefaultSoundEffect();
            }

            //Calculate angle
            float angle = 0;
            if (targetingType == TargetingType.circle)
            {
                angle = angleViseur + 90 + Random.Range(-precisionFactor, precisionFactor);
            }
            else if(targetingType == TargetingType.front)
            {
                angle = -90 + Random.Range(-precisionFactor, precisionFactor);
            }

            //Calculate position
            Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 1);

            //Generate salve object
            Salve salve = new Salve();
            salve = pooler.getSalve();
            salve.transform.position = bulletPosition;
            salve.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            salve.gameObject.SetActive(true);
            for (int j = 0; j < salve.projectiles.Length; j++)
            {
                salve.projectiles[j].transform.localPosition = salve.projectilesInitialPosition[j];
                salve.projectiles[j].transform.localRotation = salve.projectilesInitialRotation[j];
                salve.projectiles[j].gameObject.SetActive(true);
                salve.projectiles[j].col.enabled = true;
            }

            //Initiate object stats
            salve.globalDamage = globalDamage;
            salve.globalHealth = globalHealth;
            salve.globalBulletSpeed = globalBulletSpeed;
            for (int j = 0; j < salve.projectiles.Length; j++)
            {
                salve.projectiles[j].damage = globalDamage;
                salve.projectiles[j].health = globalHealth;
                salve.projectiles[j].bulletSpeed = globalBulletSpeed;
            }

            yield return new WaitForSeconds(fireRate); // wait till the next round
        }

        //Stop looping sound effect
        if (soundEffectsGenerator.isLooping)
        {
            soundEffectsGenerator.stopSoundEffect();
        }

        ennemy.isMoving = true;
        isFiring = false;
        cooldownTimer = cooldown;
    }

    private void dispersionShot()
    {

    }
}
