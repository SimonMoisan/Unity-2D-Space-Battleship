using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    //Configuration parameters
    public Transform target;        //emplacement de la cible (le joueur)
    public Ennemy ennemy;       //tourelle associé à ce viseur laser

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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Equals("Vaisseau"))
        {
            target = null;
        }
    }
}
