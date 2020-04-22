using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [Header("Beam stats")]
    public float beamThickness;
    public float impactSize;
    public float damage;
    public bool isActive;
    private float beamDuration;
    private float startBeamDuration;

    [Header("Associated objects")]
    public Turret turret;
    public LineRenderer beamLine;
    public Transform impactPoint;
    public CircleCollider2D impactCollider;
    public List<Collider2D> ennemiesToDamage;

    // Start is called before the first frame update
    void Awake()
    {
        turret = GetComponentInParent<Turret>();
        impactCollider = GetComponentInChildren<CircleCollider2D>();
        beamDuration = turret.beamDuration;

        damage = turret.damage;
        startBeamDuration = beamDuration;
        beamLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            Invoke("dealDamages", 0.5f);
            beamDurationManager();
        }
        beamLine.SetPosition(0, turret.transform.position);
        impactPoint.transform.position = beamLine.GetPosition(1);
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
            turret.isFiring = false;
            turret.animator.SetBool("Firing", false);
            turret.animator.SetBool("ReadyToFire", true);

            //Reset cooldown for turre object
            turret.cooldownTimer = turret.cooldown;
        }
    }

    private void dealDamages()
    {
        for (int i = 0; i < ennemiesToDamage.Count; i++)
        {
            ennemiesToDamage[i].GetComponent<Ennemy>().TakingDamage(damage);
        }
    }
}
