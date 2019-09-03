using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mono_Machingun_Turret : Standard_Turret
{
    // Start is called before the first frame update
    void Start()
    {
        fireRate = 0.05f;
        cooldown = 4.0f;
        precisionFactor = 3f;
        nbrBullet = 30;
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
    }
}
