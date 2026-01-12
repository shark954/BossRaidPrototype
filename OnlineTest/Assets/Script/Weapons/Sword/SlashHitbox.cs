using UnityEngine;

/// <summary>
/// スラッシュの当たり判定処理（シングルプレイ版）
/// </summary>
public class SlashHitbox : MonoBehaviour
{
    public float m_speed = 10f;       // 飛ぶ速度（未使用なら削除可）
    public float m_lifetime = 2f;     // 自動削除までの時間
    public int m_damage = 30;         // 与えるダメージ

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero; // 動かない設定（自動追尾しない場合）
        Destroy(gameObject, m_lifetime); // 一定時間後に削除
    }

    void OnTriggerEnter(Collider other)
    {
        // Parameta が付いているオブジェクトに当たったらダメージを与える
        if (other.TryGetComponent<Parameta>(out var param))
        {
            param.ApplyDamage(m_damage, "YourTeam"); // チーム名はスキル側から渡すのが理想
            Destroy(gameObject); // ヒット後に削除
        }
    }
}
