using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    //Configuration parameters
    public Transform target;        //emplacement de la cible (le joueur)

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
