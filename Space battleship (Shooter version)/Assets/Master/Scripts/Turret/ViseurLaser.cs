using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViseurLaser : MonoBehaviour
{
    //Configuration parameters
    public Transform[] targets;     //tableau contenant les positions des cibles identifiés par le viseur laser
    public int indice = 0;          //indice qui indique le nombre de cible identifiée
    public GameObject turret;       //tourelle associé à ce viseur laser

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = turret.transform.position;
        //Si on relache l'acquisition de cible, on resset le tableau des cibles
        if(indice == targets.Length && !this.GetComponent<Renderer>().enabled)
        {
            indice = 0;
            targets[0] = null;
            targets[1] = null;
            targets[2] = null;
        }
    }

    //Fonction qui permet au viseur laser de suivre la sourie
    public void RotateViseur(float angleMouse)
    {
        transform.rotation = Quaternion.AngleAxis(angleMouse - 90, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Cible") && this.GetComponent<Renderer>().enabled && indice < targets.Length)
        {
            targets[indice] = collision.GetComponent<Transform>();
            indice++;
        }
    }

    public Transform[] GetTargets()
    {
        return targets;
    }

    public int GetNumberOfTarget()
    {
        return indice;
    }
}
