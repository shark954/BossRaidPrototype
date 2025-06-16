using UnityEngine;
using Mirror;

/// <summary>
/// プレイヤーまたは武器から発射される弾丸の挙動を管理するクラス。
/// 弾の移動、色の同期、寿命管理を含む。
/// </summary>
public class Bullet : NetworkBehaviour
{
    [Header("弾の設定")]
    [SerializeField] private float m_bulletSpeed = 10f;             // 弾の移動速度
    [SerializeField] private float m_lifeTime = 2f;                 // 弾の寿命（秒）

    [Header("ビジュアル")]
    [SerializeField] private Renderer m_visualRenderer;             // 弾の見た目（マテリアルの色を変更）

    [SyncVar(hook = nameof(OnColorChanged))]
    private Color m_bulletColor;                                    // サーバーからクライアントに色を同期

    /// <summary>
    /// 毎フレーム、前方に移動。
    /// </summary>
    void Update()
    {
        transform.Translate(Vector3.forward * m_bulletSpeed * Time.deltaTime);
    }

    /// <summary>
    /// サーバー側で弾の色を設定。同期される。
    /// </summary>
    /// <param name="color">弾に適用する色</param>
    [Server]
    public void SetColor(Color color)
    {
        m_bulletColor = color;
    }

    /// <summary>
    /// クライアント側で色が変わったときにマテリアルの色を変更。
    /// </summary>
    void OnColorChanged(Color oldColor, Color newColor)
    {
        if (m_visualRenderer != null)
        {
            m_visualRenderer.material.color = newColor;
        }
    }

    /// <summary>
    /// サーバーで開始されたときに自動で寿命タイマーをセット。
    /// </summary>
    [ServerCallback]
    void Start()
    {
        Invoke(nameof(DestroySelf), m_lifeTime);
    }

    /// <summary>
    /// ネットワーク越しに弾を削除する。
    /// </summary>
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
