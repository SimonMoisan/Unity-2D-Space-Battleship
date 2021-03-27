using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public static MouseCursor current;

    private void Awake()
    {
        current = this;
    }

    void Update()
    {
        Vector2 viseurPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = viseurPos;
    }
}
