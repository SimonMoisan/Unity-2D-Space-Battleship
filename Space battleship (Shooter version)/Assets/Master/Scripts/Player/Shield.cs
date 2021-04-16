using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Shield : MonoBehaviour
{
    [Serializable]
    public class ShieldTier
    {
        public int tier; // 0 to 10
        public int energyCorePrice;
        public float updatedMaxShield;
    }

    [Header("Shields stats :")]
    //Caracteristics
    public int actualTier;
    public float shieldPoints;
    public float maxShieldPoints;
    public float shieldGenerationRate;
    public float cooldown;                //time to wait between two burst
    public float cooldownTimer;
    public bool shieldGenerationActive;
    public bool overshieldActive;
    [Space]
    public ShieldTier[] shieldTiers;
    [Space]
    [Header("Associated objects :")]
    public Collider2D col;
    public Battleship battleship;
    public Animator animator;
    public PlayerStats playerStats;
    public SpriteRenderer overshieldImage;

    // Start is called before the first frame update
    void OnValidate()
    {
        //Find associated objects
        battleship = FindObjectOfType<Battleship>();
        playerStats = FindObjectOfType<PlayerStats>();
        animator = gameObject.GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        updateShieldStats(0);
    }

    // Update is called once per frame
    void Update()
    {
        CoolDownManager();
        ShieldRegeneration();
    }

    public void updateShieldStats(int tier)
    {
        maxShieldPoints = shieldTiers[tier].updatedMaxShield;
        shieldPoints = maxShieldPoints;
        actualTier = tier;
        playerStats.updateShieldIndicators();
    }

    //Function used to manage damages taken by the shield
    public void takingDamages(EnnemyProjectile ennemyProjectile)
    {
        if (shieldPoints > 0)
        {
            animator.Play("Damaged");

            float finalDamage = ennemyProjectile.damage;

            //Calculate damages taken
            switch(ennemyProjectile.damageType)
            {
                case DamageType.Kinetic:
                    finalDamage *= 0.7f;
                    break;
                case DamageType.Laser:
                    finalDamage *= 0.8f;
                    break;
                case DamageType.Plasma:
                    finalDamage *= 0.9f;
                    break;
                case DamageType.Ion:
                    finalDamage *= 3f;
                    break;
                case DamageType.Explosive:
                    finalDamage *= 1f;
                    break;
            }

            if(overshieldActive && ennemyProjectile.damageType != DamageType.Ion && ennemyProjectile.projectileSize == ProjectileSize.Light) //Reduce damages of small projectiles (execpt ions)
            {
                finalDamage *= 0.5f;
            }

            //Apply damages
            shieldPoints -= finalDamage;
            playerStats.updateShieldIndicators();

            //Activate shield regeneration cooldown
            if(cooldownTimer > 0 && cooldownTimer < cooldown)
            {
                cooldownTimer += 0.5f;
            }
            else
            {
                cooldownTimer = cooldown;
            }
        }
        else if(shieldPoints <= 0)
        {
            shieldPoints = 0;
            cooldownTimer = cooldown;
            StartCoroutine(ShieldDestruction());
            col.enabled = false;
        }
    }

    public void takingDamages(float beamDamage)
    {
        Debug.LogWarning("Beam damages not implemented");
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
            battleship.hasTakenDamagesRecently = false;
            StartCoroutine(ShieldGeneration());
        }
    }

    public void activateOvershield()
    {
        overshieldImage.enabled = true;
        overshieldActive = true;
    }

    public void deactivateOvershield()
    {
        overshieldImage.enabled = false;
        overshieldActive = false;
    }

    public void ShieldRegeneration()
    {
        if(shieldGenerationActive && shieldPoints < maxShieldPoints && !battleship.hasTakenDamagesRecently)
        {
            shieldPoints += Time.deltaTime * shieldGenerationRate;
            playerStats.updateShieldIndicators();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("EnnemyProjectile"))
        {
            takingDamages(collision.GetComponent<EnnemyProjectile>());
        }
    }

    IEnumerator ShieldDestruction()
    {
        animator.Play("Destruction");
        yield return new WaitForSeconds(0.5f);
        shieldGenerationActive = false;
    }

    IEnumerator ShieldGeneration()
    {
        if(shieldPoints <= 0)
        {
            animator.Play("Generation");
        }
        
        yield return new WaitForSeconds(0.5f);
        shieldGenerationActive = true;
        col.enabled = true;
    }
}
