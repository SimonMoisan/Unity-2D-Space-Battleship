using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject prefab;
    public GameObject GO;
    public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GO = Instantiate(prefab, Input.mousePosition, Quaternion.identity);
        }
        if(Input.GetMouseButtonDown(1) && GO != null)
        {
            isMoving = true;
        }
        if(isMoving)
        {
            var targetPosition = new Vector2(transform.position.x + 10, transform.position.y);
            var movementThisFrame = 10 * Time.deltaTime;
            GO.transform.position = Vector2.MoveTowards(GO.transform.position, targetPosition, movementThisFrame);
        }
    }
}
