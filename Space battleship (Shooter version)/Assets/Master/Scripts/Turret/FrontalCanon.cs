using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontalCanon : MonoBehaviour
{
    //Configuration parameters
    public float fireRate;
    public float obusSpeed;
    public GameObject Obus;

    //Coroutines
    Coroutine firingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FireBullet();
    }

    IEnumerator FireObusContinuously()
    {
        while (true)
        {
            Vector3 obusPosition = new Vector3(transform.position.x, transform.position.y, 0);
            GameObject obus = Instantiate
                (Obus,
                obusPosition,
                Quaternion.identity);
            obus.transform.GetComponent<Rigidbody2D>().AddForce(obus.transform.right * obusSpeed);
            yield return new WaitForSeconds(fireRate);
        }
    }

    void FireBullet()
    {
        if (Input.GetKeyDown("space"))
        {
            firingCoroutine = StartCoroutine(FireObusContinuously());
        }
        if (Input.GetKeyUp("space") && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }
    }
}
