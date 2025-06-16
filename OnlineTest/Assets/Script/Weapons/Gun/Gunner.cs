using UnityEngine;
using Mirror;

/// <summary>
/// ガンナーの武器スクリプト。
/// 弾丸の発射、チャージ段階の管理、スキルエフェクトなどを制御する。
/// </summary>
public class Gunner : MonoBehaviour, IWeapon
{
    [Header("武器データ（ScriptableObject）")]
    public GunnerClassData m_weaponData; // ガンナー専用の武器パラメータ

    [SerializeField] private Transform m_firePointLeft; //  左
    [SerializeField] private Transform m_firePointRight; //　右
    [SerializeField] private float m_fireInterval = 0.1f;
    private float m_nextFireTime = 0f;


    [Header("発射位置")]
    [SerializeField] private Transform m_firePoint; // 弾の発射位置（銃口など）

    [Header("チャージ状態")]
    public int m_chargeCount = 0;       // 現在のチャージ段階（威力に影響）
    public bool m_triggerOn = false;    // チャージ制御用のトリガー（連続チャージ防止）

    private bool m_isLeftNext = true;

    void Awake()
    {
        // 初期化処理が必要ならここに記述
    }

    // IWeaponインターフェースの実装
    public int m_Damage => m_weaponData.m_baseDamage;
    public float m_AddPower => m_weaponData.m_chargeMultiplier;
    public int m_ChargeCount { get => m_chargeCount; set => m_chargeCount = value; }
    public int m_MaxChargeCount { get => m_weaponData.m_maxChargeCount; set => m_weaponData.m_maxChargeCount = value; }
    public GameObject m_AttackEffect => m_weaponData.m_attackEffectPrefab;
    public float m_AttackEffectDelTime => m_weaponData.m_AttackEffectDelTime;
    public GameObject m_ChargeEffect => m_weaponData.m_chargeEffectPrefab;
    public float m_ChargeEffectDelTime => m_weaponData.m_ChargeEffectDelTime;

    public WeaponType GetWeaponType() => WeaponType.Gunner;

    /// <summary>
    /// 通常攻撃：弾丸を発射
    /// </summary>

    public void Use()
    {
        if (Time.time < m_nextFireTime) return;

        Transform firePoint = m_isLeftNext ? m_firePointLeft : m_firePointRight;
        GameObject bullet = Instantiate(m_weaponData.m_bulletPrefab, firePoint.position, firePoint.rotation);
        NetworkServer.Spawn(bullet);

        m_nextFireTime = Time.time + m_fireInterval;
        m_isLeftNext = !m_isLeftNext;
    }

    /// <summary>
    /// 弾を生成し、ネットワーク越しに発射する
    /// </summary>
    private void FireBullet()
    {
        if (m_weaponData.m_bulletPrefab == null || m_firePoint == null)
        {
            Debug.LogWarning("弾または発射位置が未設定です");
            return;
        }

        GameObject bullet = Instantiate(m_weaponData.m_bulletPrefab, m_firePoint.position, m_firePoint.rotation);
        NetworkServer.Spawn(bullet); // Mirrorを使って全クライアントに生成

        if (bullet.TryGetComponent<Bullet>(out Bullet bulletComp))
        {
            bulletComp.SetColor(Color.red); // 弾の色設定など、必要に応じて調整
        }
    }

    /// <summary>
    /// チャージ処理（段階的に強化）
    /// </summary>
    public void Charge()
    {
        if (m_triggerOn || m_chargeCount >= m_weaponData.m_maxChargeCount)
            return;

        m_chargeCount++;

        // チャージエフェクト表示
        if (m_ChargeEffect)
        {
            GameObject effect = Instantiate(m_ChargeEffect, transform.position, transform.rotation);
            Destroy(effect, m_ChargeEffectDelTime);
        }
    }
}
