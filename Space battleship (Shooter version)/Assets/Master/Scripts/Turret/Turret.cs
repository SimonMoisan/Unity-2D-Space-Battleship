using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    //Configuration parameters
    [SerializeField] public string idTurret;               //id de la tourelle
    [SerializeField] public float damage;
    [SerializeField] public float angleMouse;              //angle de visé de la tourelle par rapport à la sourie
    [SerializeField] public float angleViseur;             //angle de visé de la tourelle par rapport à son viseur
    [SerializeField] public float fireRate;                //cadence de tir de le tourelle
    [SerializeField] public float nbrBullet;               //indique le nombre de tir par rafale
    [SerializeField] public float precisionFactor;         //valeur qui indique l'amplitude de la disperion aléatoire des tirs
    [SerializeField] public float cooldown;                //time to wait between two burst
    [SerializeField] public float cooldownTimer;      
    [SerializeField] public bool isActive = false;         //indique si la tourelle est active (contrôlé par le joueur) ou non
    [SerializeField] public bool isFiring = false;         //indique si la tourelle est en train de tirer ou non
    [SerializeField] public bool lockMode = false;         //indique si la tourelle est en mode vérouillage ou 

    //Screen parameters
    [SerializeField] float widthUnits = 42.6667f;
    [SerializeField] float minX = 0f;
    [SerializeField] float maxX = 42.6667f;
    [SerializeField] float minY = 0f;
    [SerializeField] float maxY = 24f;

    //Coroutines
    public Coroutine manualFiringCoroutine;                //coroutine de tir manuel de la tourelle 

    //Associated gameobjects
    public GameObject Bullet;                              //type de balle tiré par la tourelle
    public Viseur viseur;
    public Image cooldownImage;

    //Animator
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownImage.fillAmount = cooldownTimer / cooldown;
        Fire();
    }

    //Fonction qui gère l'intégralité de la routine de tir de la tourelle
    public void Fire()
    {
        CoolDownManager();
        if (isActive && cooldownTimer == 0) //quand la tourelle est active et que l'autofire est désactivé
        { 
            if (Input.GetKeyDown(idTurret) && !isFiring) //on lance le burst de tir
            {
                SetViseurLocation();
                isActive = false;
                animator.SetBool("Firing", true);
                animator.SetBool("ReadyToFire", true);
                isFiring = true;
                manualFiringCoroutine = StartCoroutine(BurstFire());
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
            cooldownTimer = 0;
            animator.SetBool("Firing", false);
            animator.SetBool("ReadyToFire", true);
        }
    }

    IEnumerator BurstFire()
    {
        yield return new WaitForSeconds(0.1f);
        // rate of fire in weapons is in rounds per minute (RPM), therefore we should calculate how much time passes before firing a new round in the same burst.
        for (int i = 0; i < nbrBullet; i++)
        {
            Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 1);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleViseur - 90));
            GameObject bullet = Instantiate
                (Bullet,
                bulletPosition,
                Quaternion.Euler(new Vector3(0, 0, (angleViseur - 90) + Random.Range(-precisionFactor, precisionFactor))));
            bullet.GetComponent<Salve>().SetSalveDamage(damage);
            yield return new WaitForSeconds(fireRate); // wait till the next round
        }
        animator.SetBool("Firing", false);
        animator.SetBool("ReadyToFire", false);
        isFiring = false;
        cooldownTimer = cooldown;
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

    //Fonction qui renvoie si la tourelle est active ou non
    public bool GetTurretState()
    {
        return isActive;
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
