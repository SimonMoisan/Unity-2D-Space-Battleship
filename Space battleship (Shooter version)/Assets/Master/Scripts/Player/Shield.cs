using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [Header("Shields stats :")]
    //Caracteristics
    public float shieldPoints;
    public float maxShieldPoints;
    public float shieldGenerationRate;
    public float cooldown;                //time to wait between two burst
    public float cooldownTimer;
    public bool shieldGenerationActive;

    [Header("Associated objects :")]
    //Associated objects
    public Collider2D col;
    public Battleship battleship;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //Find associated objects
        battleship = FindObjectOfType<Battleship>();
        animator = gameObject.GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        shieldPoints = maxShieldPoints;
    }

    // Update is called once per frame
    void Update()
    {
        CoolDownManager();
        ShieldRegeneration();
    }

    //Function used to manage damages taken by the shield
    public void TakingDamages(float damageInput)
    {
        if (shieldPoints > 0)
        {
            shieldPoints -= damageInput;

            //Activate shield regeneration cooldown
            if(cooldownTimer > 0)
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

    public void ShieldRegeneration()
    {
        if(shieldGenerationActive && shieldPoints < maxShieldPoints && !battleship.hasTakenDamagesRecently)
        {
            shieldPoints += Time.deltaTime * shieldGenerationRate;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("EnnemyProjectile"))
        {
            TakingDamages(collision.GetComponent<EnnemyProjectile>().damage);
        }
    }

    IEnumerator ShieldDestruction()
    {
        animator.SetBool("Destroyed", true);
        yield return new WaitForSeconds(0.5f);
        shieldGenerationActive = false;
    }

    IEnumerator ShieldGeneration()
    {
        animator.SetBool("Destroyed", false);
        yield return new WaitForSeconds(0.5f);
        shieldGenerationActive = true;
        col.enabled = true;
    }
}
