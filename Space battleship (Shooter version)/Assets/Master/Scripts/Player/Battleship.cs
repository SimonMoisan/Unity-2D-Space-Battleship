using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battleship : MonoBehaviour
{
    //Battleship caracteristics
    [SerializeField] public float MaxHullPoints = 2000f;
    [SerializeField] public float MaxPlatePoints = 500f;
    [SerializeField] public float hullPoints;
    [SerializeField] public float platePoints;
    [SerializeField] public bool hasTakenDamagesRecently;

    public bool uwIsRunning = false;

    //Parameters used for movement
    [SerializeField] float velocity = 0f;        // Current Travelling Velocity
    [SerializeField] float maxVelocity = 0f;     // Maxima Velocity
    [SerializeField] float acc = 0f;             // Current Acceleration
    [SerializeField] float accSpeed = 0f;        // Amount to increase Acceleration with.
    [SerializeField] float maxAcc = 0f;          // Max Acceleration
    [SerializeField] float minAcc = 0f;          // Min Acceleration
    [SerializeField] float accRate = 0f;         // Acceleration and Decceleration rate
    [SerializeField] string actualDirection;     // Indique dans quelle direction se dirige le vaisseau (permt d'éviter d'appuyer sur deux touches en même temps)
    [SerializeField] float minY;                 // Minimum height the battleship can reach
    [SerializeField] float maxY;                 // Maximum height the battleship can reach
    [SerializeField] float defaultX;             // Default X position
    [SerializeField] float maxX;                 // Maximum X position the ship can reach when he decelerate

    //Coroutines
    Coroutine accelerationCoroutine;             // Coroutine d'accélération du vaisseau
    Coroutine deccelerationCoroutine;            // Coroutine de décélération du vaisseau

    //Animations
    Animator animator;

    //Associated objects
    [SerializeField] public Turret[] arsenal;    // Tableau contenant les tourelles du vaisseau
    [SerializeField] public Shield shield;
    [SerializeField] public Gate gate;
    [SerializeField] public UltimateWeapon ultimateWeapon;
    [SerializeField] public UWProjectile uwProjectile;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        hullPoints = MaxHullPoints;
        platePoints = MaxPlatePoints;
        hasTakenDamagesRecently = false;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchTurret();
        NewMoveBattleshipAcc();
        StaticAnimation();
        UWdeployer();

        Vector3 shipPos = new Vector3(transform.position.x, transform.position.y, 2);
        shipPos.y = Mathf.Clamp(shipPos.y, minY, maxY);
        transform.position = shipPos;
    }

    //Fonction de gestion de l'activation des tourelles
    public void SwitchTurret()
    {
        if (Input.GetKeyDown("1"))
        {
            arsenal[0].isActive = true;
        }
        else if (Input.GetKeyDown("2"))
        {
            arsenal[1].isActive = true;
        }
        else if (Input.GetKeyDown("3"))
        {
            arsenal[2].isActive = true;
        }
    }

    //Fonction de déploiement de l'UW
    public void UWdeployer()
    {
        if (Input.GetKeyDown("u") && !uwIsRunning)
        {
            StartCoroutine(UWdeployment());
        }
    }

    //Fonction qui gère les dégats subis par le vaisseau
    public void TakingDamages(int damageInput)
    {
        //Add shield regeneration cooldown if the battleship taken damages
        hasTakenDamagesRecently = true;
        shield.cooldownTimer += 0.5f;

        if (platePoints > 0 && shield.shieldPoints <= 0)
        {
            platePoints -= damageInput;
        }
        else if(platePoints <= 0 && shield.shieldPoints <= 0)
        {
            hullPoints -= damageInput;
        }
    }

    //Fonction qui gère le déplacement du vaisseau
    public void NewMoveBattleshipAcc()
    {
        ////////////////////////////////////////////////////////////////////////////
        //Gère le déplacement vers le haut du vaisseau
        if (Input.GetKeyDown("z") && actualDirection != "DOWN")
        {
            actualDirection = "UP";
            accelerationCoroutine = StartCoroutine(AccelerateContinuously("UP", maxAcc, minAcc, maxVelocity, accSpeed));
            if(deccelerationCoroutine != null)
            {
                StopCoroutine(deccelerationCoroutine);
            }
        }
        if (Input.GetKeyUp("z") && accelerationCoroutine != null)
        {
            StopCoroutine(accelerationCoroutine);
            acc = 0;
            deccelerationCoroutine = StartCoroutine(DeccelerateContinuously("UP", maxAcc, minAcc, maxVelocity, accSpeed));
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        //Gère le déplacement vers le bas du vaisseau
        if (Input.GetKeyDown("s") && actualDirection != "UP")
        {
            actualDirection = "DOWN";
            accelerationCoroutine = StartCoroutine(AccelerateContinuously("DOWN", maxAcc, minAcc, maxVelocity, accSpeed));
            if (deccelerationCoroutine != null)
            {
                StopCoroutine(deccelerationCoroutine);
            }
        }
        if (Input.GetKeyUp("s") && accelerationCoroutine != null)
        {
            StopCoroutine(accelerationCoroutine);
            acc = 0;
            deccelerationCoroutine = StartCoroutine(DeccelerateContinuously("DOWN",maxAcc ,minAcc, maxVelocity ,accSpeed));
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        //Gère le déplacement en cas d'interruption de décelération
        if (Input.GetKeyDown("z") && actualDirection == "DOWN")
        {
            actualDirection = "UP";
            acc = 0;
            StopCoroutine(accelerationCoroutine); 
            if (deccelerationCoroutine != null)
            {
                StopCoroutine(deccelerationCoroutine);
            }
            accelerationCoroutine = StartCoroutine(AccelerateContinuously("UP", maxAcc, minAcc, maxVelocity, accSpeed));
        }
        if (Input.GetKeyDown("s") && actualDirection == "UP")
        {
            actualDirection = "DOWN";
            acc = 0;
            StopCoroutine(accelerationCoroutine);
            if (deccelerationCoroutine != null)
            {
                StopCoroutine(deccelerationCoroutine);
            }
            accelerationCoroutine = StartCoroutine(AccelerateContinuously("DOWN", maxAcc, minAcc, maxVelocity, accSpeed));
        }
        ////////////////////////////////////////////////////////////////////////////
    }

    //Fonction qui ajoute une petite animation lorsque le vaisseau est statique
    public void StaticAnimation()
    {
        if(actualDirection == "NONE")
        { 
            transform.position = transform.position + new Vector3(0, Mathf.Sin(Time.time * 2.0f) * 0.002f, 2);
        }
    }

    IEnumerator AccelerateContinuously(string direction, float maxAcc, float minAcc, float maxVelocity, float accSpeed) //Coroutine d'accelleration du vaisseau
    {
        while (true)
        {
            if(direction == "UP")
            {
                acc += accSpeed;
            }
            else if (direction == "DOWN")
            {
                acc -= accSpeed;
            }

            if (acc > maxAcc)
            {
                acc = maxAcc;
            }
            else if (acc < minAcc)
            {
                acc = minAcc;
            }

            velocity += acc;

            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
            else if (velocity < -maxVelocity)
            {
                velocity = -maxVelocity;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, velocity);
            yield return new WaitForSeconds(accRate);
        }
    }

    IEnumerator DeccelerateContinuously(string direction, float maxAcc, float minAcc, float maxVelocity, float accSpeed) //Coroutine de déccelleration du vaisseau
    {
        while (velocity != 0f)
        {
            if(direction == "UP") //si la direction initiale est UP on réduit la velocity jusqu'à atteindre 0 à coup sur
            {
                velocity -= 0.50f;
                if(velocity < 0) //si on a trop diminué la velocity on remet à 0
                {
                    velocity = 0;
                }
            }
            else if (direction == "DOWN") //si la direction initiale est DOWN on augmente la velocity jusqu'à atteindre 0 à coup sur
            {
                velocity += 0.50f;
                if (velocity > 0) //si on a trop augmenté la velocity on remet à 0
                {
                    velocity = 0;
                }
            }

            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
            else if (velocity < -maxVelocity)
            {
                velocity = -maxVelocity;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, velocity);
            yield return new WaitForSeconds(accRate);
        }
        actualDirection = "NONE";
    }

    IEnumerator MovingBackward(string direction, float maxAcc, float minAcc, float maxVelocity, float accSpeed) //Coroutine de ralentissement du vaisseau ce qu'il lui permet de reculer
    {
        while (true)
        {
            acc += accSpeed;

            if (acc > maxAcc)
            {
                acc = maxAcc;
            }
            else if (acc < minAcc)
            {
                acc = minAcc;
            }

            velocity += acc;

            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
            else if (velocity < -maxVelocity)
            {
                velocity = -maxVelocity;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, velocity);
            yield return new WaitForSeconds(accRate);
        }
    }
     
    IEnumerator BackToInitialPosition(string direction, float maxAcc, float minAcc, float maxVelocity, float accSpeed) //Coroutine où le vaisseau regagne ça position X initiale
    {
        while (true)
        {
            if (direction == "UP")
            {
                acc += accSpeed;
            }
            else if (direction == "DOWN")
            {
                acc -= accSpeed;
            }

            if (acc > maxAcc)
            {
                acc = maxAcc;
            }
            else if (acc < minAcc)
            {
                acc = minAcc;
            }

            velocity += acc;

            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
            else if (velocity < -maxVelocity)
            {
                velocity = -maxVelocity;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, velocity);
            yield return new WaitForSeconds(accRate);
        }
    }

    IEnumerator UWdeployment()
    {
        uwIsRunning = true;

        //Open gate
        gate.SetGateState(true);
        yield return new WaitForSeconds(1);

        //Deploy UW
        ultimateWeapon.SetUWState(true);
        yield return new WaitForSeconds(1);

        //Fire
        uwProjectile.setBeamState(1);
        uwProjectile.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        uwProjectile.setBeamState(2);
        yield return new WaitForSeconds(5);
        uwProjectile.setBeamState(3);
        yield return new WaitForSeconds(0.5f);
        uwProjectile.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        uwProjectile.setBeamState(0);

        //Redeply UW
        ultimateWeapon.SetUWState(false);
        yield return new WaitForSeconds(1);

        //Close gate
        gate.SetGateState(false);

        uwIsRunning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("EnnemyProjectile"))
        {
            TakingDamages(50);
        }
    }
}
