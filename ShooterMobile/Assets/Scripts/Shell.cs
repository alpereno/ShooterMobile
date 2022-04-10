using System.Collections;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Shell : MonoBehaviour
{
    [SerializeField] private Rigidbody shellRigidbody;
    [SerializeField] private Vector2 forceMinMax;

    float lifeTime = 3.5f;
    float fadeTime = 2.5f;
    void Start()
    {
        float force = Random.Range(forceMinMax.x, forceMinMax.y);
        shellRigidbody.AddForce(transform.right * force);
        shellRigidbody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(fade());
    }

    IEnumerator fade() {
        yield return new WaitForSeconds(lifeTime);

        float fadePercent = 0;
        float fadeSpeed = 1 / fadeTime;
        Material material = GetComponent<Renderer>().material;
        Color initialColor = material.color;

        while (fadePercent <= 1) {
            fadePercent += Time.deltaTime * fadeSpeed;
            material.color = Color.Lerp(initialColor, Color.clear, fadePercent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
