using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWProjectile : MonoBehaviour
{
    public Animator animator;
    public BoxCollider2D collider;
    [SerializeField] public int beamState = 0;     //Define the current state of the laser beam (0 = invisible, 1 = init, 2 = idle, 3 = end)

    // Start is called before the first frame update
    void Start()
    {
        collider = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(beamState == 1 || beamState == 2 || beamState == 3)
        {
            collider.enabled = true;
        }
        else
        {
            collider.enabled = false;
        }
        animator.SetInteger("currentState", beamState);
    }

    public void setBeamState(int newState)
    {
        beamState = newState;
    }
}
