using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Double_Laser_Turret : Standard_Turret
{
    // Start is called before the first frame update
    void Start()
    {
        fireRate = 0.2f;
        cooldown = 4.0f;
        precisionFactor = 0f;
        nbrBullet = 10;
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
    }
}
