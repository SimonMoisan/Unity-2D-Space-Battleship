using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWProjectile : MonoBehaviour
{
    public Animator animator;
    public BoxCollider2D collider;
    private Ennemy[] ennemies;
    private int ennemyIndex = 0;
    [SerializeField] public int beamState = 0;     //Define the current state of the laser beam (0 = invisible, 1 = init, 2 = idle, 3 = end)
    [SerializeField] public float damagesOverTime;

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

            //Inflict damage over time for every ennemy touched in the raybeam
            if(ennemies != null)
            {
                for(int i = 0;i < ennemyIndex; i++)
                {
                    if(ennemies[i] != null)
                    {
                        ennemies[i].TakingDamage(damagesOverTime * Time.deltaTime);
                    }
                }
            }
        }
        else
        {
            ennemies = null;
            collider.enabled = false;
        }
        animator.SetInteger("currentState", beamState);
    }

    public void setBeamState(int newState)
    {
        beamState = newState;
    }

    public void removeSpecificEnnemy(Ennemy ennemyToCheck)
    {
        for(int i = 0;i < ennemyIndex; i++)
        {
            if(ennemies[i] == ennemyToCheck)
            {
                ennemies = null;
            }
        }
        ennemyIndex--;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Ennemy")
        {
            Ennemy ennemyGO = collider.GetComponent<Ennemy>();
            ennemies[ennemyIndex] = ennemyGO;
            ennemyIndex++;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Ennemy")
        {
            Ennemy ennemyGO = collider.GetComponent<Ennemy>();
            removeSpecificEnnemy(ennemyGO);
        }
    }
}
