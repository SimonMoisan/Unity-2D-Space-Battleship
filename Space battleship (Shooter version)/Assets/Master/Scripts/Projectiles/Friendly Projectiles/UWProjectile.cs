using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWProjectile : MonoBehaviour
{
    public Animator animator;
    public BoxCollider2D collider;
    private List<Transform> ennemies;
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
                foreach(Transform ennemy in ennemies)
                {
                    Ennemy ennemyGO = ennemy.GetComponent<Ennemy>();
                    ennemyGO.TakingDamage(damagesOverTime * Time.deltaTime);
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Ennemy")
        {
            ennemies.Add(collider.GetComponent<Transform>());
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Ennemy")
        {
            ennemies.Remove(collider.GetComponent<Transform>());
        }
    }
}
