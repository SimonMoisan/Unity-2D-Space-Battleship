using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    //Configuration parameters
    public Transform target;        //emplacement de la cible (le joueur)
    public Ennemy ennemy;       //tourelle associé à ce viseur laser

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = ennemy.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Vaisseau"))
        {
            target = collision.GetComponent<Transform>();
        }
    }

    public Transform GetTarget()
    {
        return target;
    }
}
