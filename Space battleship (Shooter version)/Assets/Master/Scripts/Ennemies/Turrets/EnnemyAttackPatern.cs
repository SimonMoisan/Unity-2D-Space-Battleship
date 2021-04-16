using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackPaternType { Any, Aggro, Passive, Rush }
public class EnnemyAttackPatern : MonoBehaviour
{
    [Header("Stats :")]
    public AttackPaternType attackPaternType;
    public float globalCooldown;
    public float globalCooldownTimer;
    public bool isReadyToFire;
    public float shotDelay;
    [Header("Associted objects :")]
    public EnnemyAttack[] ennemyAttacks;

    private void OnValidate()
    {
        globalCooldownTimer = globalCooldown;
        shotDelay = 0;
        for (int i = 0; i < ennemyAttacks.Length; i++)
        {
            if(ennemyAttacks[i].beam != null && ennemyAttacks[i].beamDuration > shotDelay)
            {
                shotDelay = ennemyAttacks[i].beamDuration;
            }
            else if(ennemyAttacks[i].fireRate * ennemyAttacks[i].nbrShots > shotDelay)
            {
                shotDelay = ennemyAttacks[i].fireRate * ennemyAttacks[i].nbrShots;
            }
        }
    }

    public void cooldownManager()
    {
        if(globalCooldownTimer <= 0)
        {
            for (int i = 0; i < ennemyAttacks.Length; i++)
            {
                ennemyAttacks[i].cooldownTimer = 0;
            }
            isReadyToFire = true;
        }
        else
        {
            globalCooldownTimer -= Time.deltaTime;
        }
    }

    public IEnumerator fireAllAttacks()
    {
        isReadyToFire = false;
        for (int i = 0; i < ennemyAttacks.Length; i++)
        {
            ennemyAttacks[i].Fire();
        }

        yield return new WaitForSeconds(shotDelay);

        globalCooldownTimer = globalCooldown;
    }
}
