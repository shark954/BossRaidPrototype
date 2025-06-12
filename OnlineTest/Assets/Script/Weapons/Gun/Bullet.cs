using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    public float bullet_speed = 10f;

    public Renderer visualRenderer;

    [SyncVar(hook = nameof(OnColorChanged))]
    private Color bulletColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
  

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * bullet_speed * Time.deltaTime);
    }

    [Server]
    public void SetColor(Color color)
    {
        bulletColor = color;
    }


    void OnColorChanged(Color oldColor, Color newColor)
    {
        visualRenderer.material.color = newColor;
    }


    [ServerCallback]
    void Start()
    {
        Invoke(nameof(DestroySelf), 2f);
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
