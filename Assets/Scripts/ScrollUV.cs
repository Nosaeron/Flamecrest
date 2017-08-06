using UnityEngine;

public class ScrollUV : MonoBehaviour
{
    Vector2 direction;
    public float velocity = 0.01f;

    void Awake()
    {
        float rng = Random.Range(0.0f, 1.0f);
        direction = new Vector2(1.0f*rng, 1.0f-rng);
    }

	void Update()
    {
        GetComponent<MeshRenderer>().material.mainTextureOffset += velocity * direction * Time.deltaTime;
	}
}
