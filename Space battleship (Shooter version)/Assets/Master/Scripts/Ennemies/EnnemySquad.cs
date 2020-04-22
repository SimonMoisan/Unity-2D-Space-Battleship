using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySquad : MonoBehaviour
{
    public Ennemy[] ennemies;
    public int ennemyAlive;
    public WaveConfig waveConfig;
    public bool isDestroyed;

    //Configuration parameters
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    int waypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        ennemies = GetComponentsInChildren<Ennemy>();
        ennemyAlive = ennemies.Length;

        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    private void Update()
    {
        MoveCible();
    }

    public IEnumerator imDestroyed()
    {
        ennemyAlive--;
        if (ennemyAlive <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            yield return new WaitForSeconds(0.05f);
            Destroy();
        }
    }

    private void MoveCible()
    {
        if (waypointIndex <= waypoints.Length - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            if (waveConfig.dieAtEnd)
            {
                EnnemySpawner ennemySpawner = FindObjectOfType<EnnemySpawner>(); //Send to the ennemy spawn that he has been destroyed
                for (int i = 0; i < ennemyAlive; i++)
                {
                    ennemySpawner.EnnemyDestroyed();
                }
                Destroy();
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
