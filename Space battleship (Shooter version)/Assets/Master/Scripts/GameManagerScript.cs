using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //Associated object
    public StarMapManagement starMapManager;

    // Start is called before the first frame update
    void OnAwake()
    {
        starMapManager = FindObjectOfType<StarMapManagement>();
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
