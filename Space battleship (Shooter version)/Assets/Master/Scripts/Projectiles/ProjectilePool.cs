using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public Salve salvePrefab;
    private List<Salve> salveList;
    private bool notEnoughtProjectiles = true;

    // Start is called before the first frame update
    void Start()
    {
        salveList = new List<Salve>();
    }

    public void initializePool(int numberOfProjectile)
    {
        for (int i = 0; i < numberOfProjectile; i++)
        {
            Salve newSalve = Instantiate(salvePrefab);
            newSalve.gameObject.SetActive(false);
            for (int j = 0; j < newSalve.projectiles.Length; j++)
            {
                newSalve.projectiles[j].gameObject.SetActive(false);
            }

            salveList.Add(newSalve);
        }
    }

    public Salve getSalve()
    {
        if (salveList.Count > 0)
        {
            for(int i=0; i < salveList.Count; i++)
            {
                if(!salveList[i].gameObject.activeInHierarchy)
                {
                    Debug.Log("Use salve");
                    return salveList[i];
                }
            }
        }

        if(notEnoughtProjectiles)
        {
            Debug.Log("New salve");
            Salve newSalve = Instantiate(salvePrefab);
            newSalve.gameObject.SetActive(false);
            for (int i = 0; i < newSalve.projectiles.Length; i++)
            {
                newSalve.projectiles[i].gameObject.SetActive(false);
            }

            salveList.Add(newSalve);
            return newSalve;
        }

        Debug.Log("Null");
        return null;
    }
}
