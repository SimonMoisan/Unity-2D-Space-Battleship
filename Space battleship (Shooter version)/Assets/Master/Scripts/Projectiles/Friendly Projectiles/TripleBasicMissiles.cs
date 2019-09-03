using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleBasicMissiles : MonoBehaviour
{
    //Configuration parameters
    public BasicHommingMissile[] missiles;
    public TripleBasicMissilesTurret turret;
    public Transform[] targets;
    public int missileDestroyed = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                Debug.Log(targets[i]);
                missiles[i].SetTarget(targets[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {   
        /*if (missileDestroyed >= 3)
        {
            Destroy(gameObject);
        }*/
    }

    public void SetTarget(Transform[] newTarget)
    {
        targets = newTarget;
    }
}
