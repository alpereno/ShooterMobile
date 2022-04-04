using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    float smoothSpeed = 5;
    [SerializeField] Transform playerT;
    Vector3 offset;
    //Vector3 velocity;

    private void Start()
    {
        //playerT = FindObjectOfType<Player>().transform;
        offset = transform.position - playerT.position;
    }

    private void LateUpdate()
    {
        if (playerT != null)
        {
            Vector3 targetCamPos = playerT.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothSpeed * Time.deltaTime);
        }
        //transform.position = Vector3.SmoothDamp(transform.position, targetCamPos, ref velocity, smoothSpeed);
    }
}
