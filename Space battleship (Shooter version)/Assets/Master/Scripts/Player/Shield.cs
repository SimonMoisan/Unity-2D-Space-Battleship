using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    //Caracteristics
    [Range(0, 1000)]
    [SerializeField] public float shieldPoints;
    [SerializeField] public float maxShieldPoints;
    [SerializeField] public float shieldGenerationRate;
    [SerializeField] public float cooldown;                //time to wait between two burst
    [SerializeField] public float cooldownTimer;
    [SerializeField] public bool shieldGenerationActive;

    //Associated gameobjects
    [SerializeField] public Collider2D collider;
    [SerializeField] public Battleship battleship;

    //Animator
    [SerializeField] public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        maxShieldPoints = 1000;
        shieldPoints = 1000;
        shieldGenerationRate = 50;
        cooldown = 5.0f;
        shieldGenerationActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        CoolDownManager();
        ShieldRegeneration();
    }

    //Fonction qui gère les dégats subis par le vaisseau
    public void TakingDamages(int damageInput)
    {
        if (shieldPoints > 0)
        {
            shieldPoints -= damageInput;
        }
        else if(shieldPoints <= 0)
        {
            shieldPoints = 0;
            cooldownTimer = cooldown;
            StartCoroutine(ShieldDestruction());
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
            TakingDamages(50);
        }
    }

    IEnumerator ShieldDestruction()
    {
        animator.SetBool("Destroyed", true);
        yield return new WaitForSeconds(0.5f);
        shieldGenerationActive = false;
        collider.enabled = false;
    }

    IEnumerator ShieldGeneration()
    {
        animator.SetBool("Destroyed", false);
        yield return new WaitForSeconds(0.5f);
        shieldGenerationActive = true;
        collider.enabled = true;
    }
}
