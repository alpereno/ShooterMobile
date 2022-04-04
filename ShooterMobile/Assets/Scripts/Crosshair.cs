using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private SpriteRenderer dotSpriteRenderer;
    [SerializeField] private Color dotColor;
    [SerializeField] private float rotateSpeed = -45;
    Color originalColor;

    void Start()
    {
        Cursor.visible = false;
        originalColor = dotSpriteRenderer.color;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    public bool detectTarget(Ray ray, float rayDistance) {
        if (Physics.Raycast(ray, rayDistance, targetLayerMask))
        {
            dotSpriteRenderer.color = dotColor;
            return true;
        }
        else 
        {
            dotSpriteRenderer.color = originalColor;
            return false;
        }           
    }
}
