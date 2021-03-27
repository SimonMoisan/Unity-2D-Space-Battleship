using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTurret : Turret
{
    [Header("Rotation turret parameters :")]
    public int turretButtonId;
    [HideInInspector] public float angleMouse;              //angle de visé de la tourelle par rapport à la sourie
    [HideInInspector] public float angleViseur;             //angle de visé de la tourelle par rapport à son viseur
    public bool lockMode = false;                           //indique si la tourelle est en mode vérouillage ou 
    public Viseur viseur;

    private void OnValidate()
    {
        base.OnValidate();
        turretButtonId = idTurret % 4;
    }

    void Update()
    {
        if (PlayerStats.current.isPlaying)
        {
            CoolDownManager();
            //fire();
        }

        if (isFiring && PlayerStats.current.turretActualySelected == this)
        {
            SetViseurLocation();
        }
        else if (viseur != null && isFiring && !viseur.isLocked)
        {
            viseur.isLocked = true;
            viseur.GetComponent<Animator>().Play("Locked");
        }
    }

    public void fire()
    {
        if ((maxAmmo > 0 && actualAmmo > 0) || (maxAmmo == 0 && cooldownTimer == 0))
        {
            //Rotation turret
            int key = idTurret + 1;
            if (descritpion.turretSize == TurretSize.Standard /*&& Input.GetKeyDown(key.ToString())*/ && !isFiring) //on lance le burst de tir
            {
                isFiring = true;
                SetViseurLocation();

                if (isTurretBeam && beam != null && viseur != null) //Beam turret
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

                PlayerStats.current.turretActualySelected = this;
                lockMode = true;
            }
        }

        //Lockmode management
        if (isFiring && lockMode)
        {
            FollowViseur();
        }
    }

    IEnumerator BurstFire()
    {
        yield return new WaitForSeconds(0.1f);

        //Looping sound effect
        if (soundEffectsGenerator.isLooping)
        {
            soundEffectsGenerator.playDefaultSoundEffect();
        }

        // rate of fire in weapons is in rounds per minute (RPM), therefore we should calculate how much time passes before firing a new round in the same burst.
        for (int i = 0; i < nbrBullet; i++)
        {
            animator.Play("Firing");

            //Mono sound effect
            if (!soundEffectsGenerator.isLooping)
            {
                soundEffectsGenerator.playDefaultSoundEffect();
            }

            //Calculate angle
            float angle = angleViseur - 90 + Random.Range(-deviationFactor, deviationFactor);

            //Change turret angle
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            //Set bullet position and turret rotation according to its angle
            Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 0);

            //Create salve gameobject
            Salve salve = new Salve();
            salve = pooler.getSalve();
            salve.transform.position = bulletPosition;
            salve.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (angleViseur - 90) + Random.Range(-deviationFactor, deviationFactor)));
            salve.gameObject.SetActive(true);

            for (int j = 0; j < salve.projectiles.Length; j++)
            {
                salve.projectiles[j].transform.localPosition = salve.projectilesInitialPosition[j];
                salve.projectiles[j].transform.localRotation = salve.projectilesInitialRotation[j];
                salve.projectiles[j].gameObject.SetActive(true);
                salve.projectiles[j].col.enabled = true;
            }

            //Initialize objects stats
            salve.globalDamage = damage;
            salve.globalHealth = bulletHealth;
            salve.globalBulletSpeed = bulletSpeed;
            for (int j = 0; j < salve.projectiles.Length; j++)
            {
                salve.projectiles[j].damage = salve.globalDamage;
                salve.projectiles[j].health = salve.globalHealth;
                salve.projectiles[j].bulletSpeed = salve.globalBulletSpeed;
            }

            yield return new WaitForSeconds(fireRate); // wait till the next round
        }

        if (maxAmmo > 0) //Turret with ammunitions
        {
            actualAmmo -= nbrBullet;
            animator.Play("OnCD");

            if (cooldownTimer == 0)
            {
                cooldownTimer = cooldown;
            }
        }
        else //Normal turret
        {
            animator.Play("OnCD");
            cooldownTimer = cooldown;
        }

        //Stop looping sound effect
        if (soundEffectsGenerator.isLooping)
        {
            soundEffectsGenerator.stopSoundEffect();
        }

        //Reset viseur
        if (viseur != null)
        {
            viseur.GetComponent<Animator>().Play("Idle");
            viseur.isLocked = false;
        }

        isFiring = false;
    }

    //Fonction qui permet à la tourelle active de suivre la sourie
    public void FollowMouse()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - pos;
        angleMouse = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angleMouse - 90, Vector3.forward);
    }

    //Fonction qui permet à la tourelle de rester lock sur le viseur (même si le vaisseau bouge)
    public void FollowViseur()
    {
        Vector3 dif = viseur.transform.position - transform.position;
        angleViseur = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleViseur - 90);
    }

    public void SetViseurLocation()
    {
        Vector2 viseurPos = MouseCursor.current.transform.position;

        if (viseur != null)
        {
            viseur.transform.position = viseurPos;

            FollowViseur();
        }
    }
}
