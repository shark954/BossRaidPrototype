using UnityEngine;

public class BulletVisual : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    private float timer = 0f;

    public void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        timer = 0f;
        gameObject.SetActive(true);
    }

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            gameObject.SetActive(false); // ƒv[ƒ‹‚É–ß‚é
        }
    }
}
