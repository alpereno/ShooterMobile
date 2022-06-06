using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportController : MonoBehaviour
{
    [SerializeField] Transform targetTeleportPoint;
    [SerializeField] ParticleSystem teleportParticle;
    [SerializeField] float secondsBetweenTeleport;
    [SerializeField] private Slider cooldownSlider;

    static float lastTeleportTime;

    private void Start()
    {
        StartCoroutine(tpUITimer());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time > lastTeleportTime)
            {
                //print("last tp time = " + lastTeleportTime);
                //print(secondsBetweenTeleport);
                //print(Time.timeSinceLevelLoad);
                //print("next tp time = " + (Time.timeSinceLevelLoad + secondsBetweenTeleport));
                lastTeleportTime = Time.time + secondsBetweenTeleport;
                StartCoroutine(teleport(other));
            }
        }
    }

    IEnumerator teleport(Collider other)
    {
        Player playerObject = other.GetComponent<Player>();
        playerObject.disabled = true;
        //teleporting
        teleportParticle.Play();
        yield return new WaitForSeconds(1);

        if (playerObject != null)
        {
            other.transform.position = targetTeleportPoint.position;
        }        

        // after teleporting
        yield return new WaitForSeconds(1);
        playerObject.disabled = false;
    }

    IEnumerator tpUITimer()
    {
        float tpTimer;
        while (true)
        {
            tpTimer = lastTeleportTime - Time.time;
            if (tpTimer < 0) tpTimer = 0;
            cooldownSlider.value = tpTimer;
            yield return new WaitForSeconds(1);
        }        
    }
}
