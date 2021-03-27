using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyBeam : MonoBehaviour
{
    [Header("Beam stats")]
    public float beamThickness;
    public float impactSize;
    public float damage;
    public bool isActive;
    private float beamDuration;
    private float startBeamDuration;

    [Header("Associated objects")]
    public EnnemyAttack ennemyAttack;
    public LineRenderer beamLine;
    public Shield shieldToDamage;
    public Battleship battleshipToDamage;

    // Start is called before the first frame update
    void Awake()
    {
        ennemyAttack = GetComponentInParent<EnnemyAttack>();
        beamDuration = ennemyAttack.beamDuration;

        damage = ennemyAttack.globalDamage;
        startBeamDuration = beamDuration;
        beamLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            beamLine.SetPosition(0, ennemyAttack.transform.position);
            beamLine.SetPosition(1, ennemyAttack.impact.position);
            Invoke("dealDamages", 0.5f);
            beamDurationManager();
        }
        
    }

    private void beamDurationManager()
    {
        if (startBeamDuration > 0)
        {
            startBeamDuration -= Time.deltaTime;
        }
        if (startBeamDuration < 0)
        {
            //Reset cooldownDuration for beam objects
            startBeamDuration = 0;
            startBeamDuration = beamDuration;
            beamLine.enabled = false;
            isActive = false;
            ennemyAttack.isFiring = false;

            //Reset cooldown for turre object
            ennemyAttack.cooldownTimer = ennemyAttack.cooldown;
        }
    }

    private void dealDamages()
    {
        if(shieldToDamage != null)
        {
            shieldToDamage.TakingDamages(damage);
        }
        else if(battleshipToDamage != null)
        {
            battleshipToDamage.TakingDamages(damage);
        }
    }
}
