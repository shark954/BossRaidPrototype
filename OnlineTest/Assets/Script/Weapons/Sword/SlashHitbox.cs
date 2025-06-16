using UnityEngine;
using Mirror;

public class SlashHitbox : NetworkBehaviour
{
    public float m_speed = 10f;
    public float m_lifetime = 2f;
    public int m_damage = 30;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        Destroy(gameObject, m_lifetime);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Parameta>(out var param))
        {
            param.Hitdamage(m_damage, "YourTeam"); // 必要に応じてチーム設定
            NetworkServer.Destroy(gameObject); // ヒット後に消える
        }
    }
}
