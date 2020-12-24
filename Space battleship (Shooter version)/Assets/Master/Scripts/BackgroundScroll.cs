using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeed;
    public Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(Time.time * scrollSpeed, 0);
        rend.material.mainTextureOffset = offset;
    }
}
