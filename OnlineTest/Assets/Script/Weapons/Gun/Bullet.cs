using UnityEngine;

/// <summary>
/// 弾の移動、色変更、寿命管理（シングルプレイ版）
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("弾の設定")]
    [SerializeField] private float m_bulletSpeed = 10f;   // 弾の速度
    [SerializeField] private float m_lifeTime = 2f;       // 弾の寿命（秒）

    [Header("ビジュアル")]
    [SerializeField] private Renderer m_visualRenderer;   // 弾の見た目（マテリアル色）

    private Color m_bulletColor = Color.white;            // 弾の色（ローカル変数）

    void Start()
    {
        // 寿命で自動破棄
        Destroy(gameObject, m_lifeTime);
    }

    void Update()
    {
        // 前方に移動
        transform.Translate(Vector3.forward * m_bulletSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 弾の色を設定し、見た目に反映
    /// </summary>
    /// <param name="color">設定する色</param>
    public void SetColor(Color color)
    {
        m_bulletColor = color;

        if (m_visualRenderer != null)
        {
            m_visualRenderer.material.color = m_bulletColor;
        }
    }
}
