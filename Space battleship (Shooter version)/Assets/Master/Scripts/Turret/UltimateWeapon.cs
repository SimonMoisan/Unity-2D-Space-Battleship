using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateWeapon : MonoBehaviour
{
    //Caracteristics
    public bool uwState;     // The gate is open (True) or close (False)

    //Animator
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        uwState = false;
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isDeployed", uwState);
    }

    public void SetUWState(bool newState)
    {
        uwState = newState;
    }

    public bool GetUWState()
    {
        return uwState;
    }
}
