using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    [SerializeField] private bool useRelativeLocation = true;
    Quaternion relativeRotation;
    // HealthUI stays in the same rotation that it was start.
    // find roration of parent(canvas) (start func)
    // and use that every update sets current local rotation 

    void Start()
    {
        relativeRotation = transform.parent.localRotation;
    }

    void Update()
    {
        if (useRelativeLocation)
        {
            transform.rotation = relativeRotation;
        }        
    }
}
