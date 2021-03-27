using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleBasicMissilesTurret : Turret
{
    /*//Configuration parameters
    [SerializeField] public Transform[] targets;            //tableau de cible lu par le viseur
    [SerializeField] public int numberOfTarget = 0;
    [SerializeField] public bool viseurIsActive = false;    //indique si le viseur laser est affiché ou non
    [SerializeField] public bool salveReady = false;

    //Coroutines
    public Coroutine FiringMissileCoroutine;                //coroutine de tir manuel de la tourelle lance missile
    public Coroutine AcquiringTargetsCoroutine;

    //Associated gameobjects
    public TripleBasicMissiles Missile;                     //type de balle tiré par la tourelle
    public ViseurLaser viseurLaser;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isActive) //quand la tourelle est active et que l'autofire est désactivé
        {
            FollowMouse();
            viseurLaser.RotateViseur(angleMouse);
            FireMissile();
        }
    }

    //Fonction qui permet de tirer des missiles et d'afficher le viseur laser
    public void FireMissile()
    {
        if (Input.GetMouseButtonDown(0)) //on affiche le viseur laser quand on appuie sur le clic gauche de la sourie
        {
            viseurLaser.GetComponent<Renderer>().enabled = true;
        }
        if(viseurLaser.GetComponent<Renderer>().enabled && !salveReady) //si le viseur laser est affiché, on acquérit les cibles
        {
            numberOfTarget = viseurLaser.GetNumberOfTarget();
            if(numberOfTarget == 3)
            {
                salveReady = true;
            }
            else
            {
                targets = viseurLaser.GetTargets();
            }
        }
        if (Input.GetMouseButtonUp(0)) //quand on relève le clic de sourie, la coroutine manuel s'arrête
        {
            if (salveReady)
            {
                Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 0);
                TripleBasicMissiles missile = Instantiate
                    (Missile,
                    bulletPosition,
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleMouse - 90)));
                missile.SetTarget(targets);
                salveReady = false;
            }
            viseurLaser.GetComponent<Renderer>().enabled = false;
        }

    }

    public Transform[] GetTargets()
    {
        return targets;
    }

    //Coroutine de tir continu
    /*IEnumerator FireMissileContinuously()
    {
        while (true)
        {
            Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 0);
            TripleBasicMissiles missile = Instantiate
                (Missile,
                bulletPosition,
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleMouse-90)));
            missile.SetTarget(target);
            yield return new WaitForSeconds(fireRate);
        }
    }*/

}
