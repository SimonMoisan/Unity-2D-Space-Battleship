using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script description :
/*This script is an AI for an ennemy which have the following behaviour :
 *Chase the player if it enter his observation area until he touch/kill the player or until the player is out of sight
 */

//The game object require, those elements to use this script
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]

public class WanderBeahaviour : MonoBehaviour
{
    //Parameters
    public float pursuitSpeed;
    public float wanderSpeed;
    float currentSpeed;
    public float directionChangeInterval;
    public bool followPlayer;
    Coroutine moveCoroutine;
    Rigidbody2D rb2D;
    CircleCollider2D circleCollider;
    Animator animator;
    Transform targetTransform = null;
    Vector3 endPosition;

    float currentAngle = 0;
    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();

        animator = GetComponent<Animator>();
        currentSpeed = wanderSpeed;
        rb2D = GetComponent<Rigidbody2D>();
        StartCoroutine(WanderRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(rb2D.position, endPosition, Color.red);
    }

    void OnDrawGizmos()
    {
        if (circleCollider != null)
        {
        Gizmos.DrawWireSphere(transform.position,
        circleCollider.radius);
        }
    }

    void ChooseNewEndpoint()
    {
        currentAngle += Random.Range(0, 360);
        currentAngle = Mathf.Repeat(currentAngle, 360);
        endPosition += Vector3FromAngle(currentAngle);
    }

    Vector3 Vector3FromAngle(float inputAngleDegrees)
    {
        float inputAngleRadians = inputAngleDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(inputAngleRadians), Mathf.Sin(inputAngleRadians), 0);
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            ChooseNewEndpoint();
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(Move(rb2D, currentSpeed));
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

    IEnumerator Move(Rigidbody2D rigidBodyToMove, float speed)
    {
        float remainingDistance = (transform.position - endPosition).sqrMagnitude;
        while (remainingDistance > float.Epsilon)
        {
            if (targetTransform != null)
            {
                endPosition = targetTransform.position;
            }
            if (rigidBodyToMove != null)
            {
                animator.SetBool("moving", true);
                Vector3 newPosition = Vector3.MoveTowards(rigidBodyToMove.position, endPosition,speed * Time.deltaTime);
                rb2D.MovePosition(newPosition);
                remainingDistance = (transform.position - endPosition).sqrMagnitude;
            }
            yield return new WaitForFixedUpdate();
        }
        animator.SetBool("moving", false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Viseur1") && followPlayer)
        {
            animator.SetBool("isTriggered", true);
            currentSpeed = pursuitSpeed;
            targetTransform = collision.gameObject.transform;
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(Move(rb2D,currentSpeed));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Equals("Viseur1"))
        {
            animator.SetBool("moving", false);
            currentSpeed = wanderSpeed;
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            targetTransform = null;
            animator.SetBool("isTriggered", false);
        }
    }


}
