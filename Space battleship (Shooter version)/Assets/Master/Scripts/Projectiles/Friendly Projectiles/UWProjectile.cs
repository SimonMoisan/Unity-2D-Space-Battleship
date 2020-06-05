using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWProjectile : MonoBehaviour
{
    [Header("UW projectile stats :")]
    public int beamState = 0;     //Define the current state of the laser beam (0 = invisible, 1 = init, 2 = idle, 3 = end)
    public float damagesOverTime;
    public LayerMask whatIsEnnemy;

    [Header("Associated objects :")]
    public Transform beamPos;
    public Animator animator;
    public new BoxCollider2D collider;
    public List<Collider2D> ennemiesToDamage;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(beamState == 1 || beamState == 2 || beamState == 3)
        {
            collider.enabled = true;
            //Inflict damage over time for every ennemy touched in the raybeam
            Invoke("DealDamages", 0.1f);
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

    private void DealDamages()
    {
        for (int i = 0; i < ennemiesToDamage.Count; i++)
        {
            ennemiesToDamage[i].GetComponent<Ennemy>().TakingDamage(damagesOverTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Ennemy>() != null)
        {
            ennemiesToDamage.Add(collision);
        }
        
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Ennemy>() != null)
        {
            ennemiesToDamage.Remove(collision);
        }
    }
}
