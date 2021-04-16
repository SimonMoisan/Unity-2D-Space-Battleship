﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyRotationTurret : RotationTurret
{
    private void Update()
    {
        CoolDownManager();
    }

    public void aim(Transform target)
    {
        Vector3 dif = target.position - transform.position;
        angleViseur = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
    }

    public new void fire()
    {
        if ((maxAmmo > 0 && actualAmmo > 0) || (maxAmmo == 0 && cooldownTimer == 0))
        {
            //Rotation turret
            if (descritpion.turretSize == TurretSize.Standard && !isFiring) //on lance le burst de tir
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
            }
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
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleViseur - 90));

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

            if (cooldownTimer == 0)
            {
                cooldownTimer = cooldown;
            }
        }
        else //Normal turret
        {
            cooldownTimer = cooldown;
        }

        //Stop looping sound effect
        if (soundEffectsGenerator.isLooping)
        {
            soundEffectsGenerator.stopSoundEffect();
        }

        lockMode = false;
        isFiring = false;
    }
}
