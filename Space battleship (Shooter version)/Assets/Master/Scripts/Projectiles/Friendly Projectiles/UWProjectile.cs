using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWProjectile : MonoBehaviour
{
    public Animator animator;
    [SerializeField] public int beamState = 0;     //Define the current state of the laser beam (0 = invisible, 1 = init, 2 = idle, 3 = end)
    [SerializeField] public float damagesOverTime;

    [SerializeField] public float attackRangeX;
    [SerializeField] public float attackRangeY;
    [SerializeField] public Transform beamPos;
    public LayerMask whatIsEnnemy;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(beamState == 1 || beamState == 2 || beamState == 3)
        {
            //Inflict damage over time for every ennemy touched in the raybeam
            Invoke("DealDamages", 0.1f);
        }
        animator.SetInteger("currentState", beamState);
    }

    public void setBeamState(int newState)
    {
        beamState = newState;
    }

    private void DealDamages()
    {
        Vector2 attackRange = new Vector2(attackRangeX, attackRangeY);
        Collider2D[] ennemiesToDamage = Physics2D.OverlapBoxAll(beamPos.position, attackRange, whatIsEnnemy);
        for (int i = 0; i < ennemiesToDamage.Length; i++)
        {
            if(ennemiesToDamage[i].GetComponent<Ennemy>() != null)
            {
                ennemiesToDamage[i].GetComponent<Ennemy>().TakingDamage(damagesOverTime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 attackRange = new Vector3(attackRangeX, attackRangeY, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(beamPos.position, attackRange);
    }
}
