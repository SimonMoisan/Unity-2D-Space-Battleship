using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BasicHommingMissile : MonoBehaviour
{
    //Configuration parameters
    public Transform target;
    private Rigidbody2D rb;
    public float speed;
    public float rotateSpeed;
    public bool isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null)
        { 
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();
            float roatateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -roatateAmount * rotateSpeed;
            rb.velocity = transform.up * speed;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public bool BulletIsDestroyed()
    {
        return isDestroyed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Cible") || collision.name.Equals("BulletDestroyer"))
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}