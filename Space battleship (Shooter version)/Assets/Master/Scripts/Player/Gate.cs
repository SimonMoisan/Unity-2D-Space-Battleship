using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    //Caracteristics
    [SerializeField] public bool gateState;     // The gate is open (True) or close (False)

    //Animator
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        gateState = false;
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Gate_is_open", gateState);
    }

    public void SetGateState(bool newState)
    {
        gateState = newState;
    }

    public bool GetGateState()
    {
        return gateState;
    }
}
