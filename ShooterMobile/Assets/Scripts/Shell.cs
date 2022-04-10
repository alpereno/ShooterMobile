using System.Collections;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Shell : MonoBehaviour, IPooledObject
{
    [SerializeField] private Rigidbody shellRigidbody;
    [SerializeField] private Vector2 forceMinMax;

    bool initialColorSaved;
    Material material;
    Color initialColor;
    float lifeTime = 3.5f;
    float fadeTime = 2.5f;

    void Awake() {
        material = GetComponent<Renderer>().material;
        setInitialColor();
    }

    private void setInitialColor()
    {
        if (!initialColorSaved)
        {
            initialColor = material.color;
            initialColorSaved = true;
        }
    }

    public void onObjectSpawn()
    {
        shellRigidbody.WakeUp();
        float force = Random.Range(forceMinMax.x, forceMinMax.y);
        shellRigidbody.AddForce(transform.right * force);
        shellRigidbody.AddTorque(Random.insideUnitSphere * force);
        material.color = initialColor;
        StartCoroutine(fade());
    }

    IEnumerator fade() {
        yield return new WaitForSeconds(lifeTime);
        float fadePercent = 0;
        float fadeSpeed = 1 / fadeTime;

        while (fadePercent <= 1) {
            fadePercent += Time.deltaTime * fadeSpeed;
            material.color = Color.Lerp(initialColor, Color.clear, fadePercent);
            yield return null;
        }

        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    void OnDisable() {
        shellRigidbody.Sleep();
    }
}
