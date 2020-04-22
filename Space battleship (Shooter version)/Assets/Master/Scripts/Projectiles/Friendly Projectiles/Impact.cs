using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    public Beam beam;

    private void Start()
    {
        beam = GetComponentInParent<Beam>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Ennemy>() != null)
        {
            beam.ennemiesToDamage.Add(collision);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Ennemy>() != null)
        {
            beam.ennemiesToDamage.Remove(collision);
        }
    }
}
