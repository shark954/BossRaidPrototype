using UnityEngine;
using Mirror;

public class SlashHitbox : NetworkBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public int damage = 30;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        Destroy(gameObject, lifetime);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Parameta>(out var param))
        {
            param.Hitdamage(damage, "YourTeam"); // �K�v�ɉ����ă`�[���ݒ�
            NetworkServer.Destroy(gameObject); // �q�b�g��ɏ�����
        }
    }
}
