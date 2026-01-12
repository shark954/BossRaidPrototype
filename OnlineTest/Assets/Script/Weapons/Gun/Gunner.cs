using UnityEngine;

/// <summary>
/// ガンナーの武器スクリプト（シングルプレイ用）
/// 弾丸発射、チャージ管理、スキルエフェクト制御
/// </summary>
public class Gunner : MonoBehaviour, IWeapon
{
    [Header("武器データ（ScriptableObject）")]
    public GunnerClassData m_weaponData; // 武器ステータス

    [SerializeField] private Transform m_firePointLeft;
    [SerializeField] private Transform m_firePointRight;
    [SerializeField] private float m_fireInterval = 0.1f;
    private float m_nextFireTime = 0f;

    [Header("発射位置")]
    [SerializeField] private Transform m_firePoint;

    [Header("チャージ状態")]
    public int m_chargeCount = 0;
    public bool m_triggerOn = false;

    private bool m_isLeftNext = true;

    void Awake()
    {
        // 必要に応じて初期化
    }

    // IWeapon インターフェース実装
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
    /// 通常攻撃：左右交互に弾丸発射
    /// </summary>
    public void Use()
    {
        if (Time.time < m_nextFireTime) return;

        Transform firePoint = m_isLeftNext ? m_firePointLeft : m_firePointRight;

        GameObject bullet = Instantiate(m_weaponData.m_bulletPrefab, firePoint.position, firePoint.rotation);
        SetupBullet(bullet);

        m_nextFireTime = Time.time + m_fireInterval;
        m_isLeftNext = !m_isLeftNext;
    }

    /// <summary>
    /// 単独の弾発射処理（中央発射点使用）
    /// </summary>
    private void FireBullet()
    {
        if (m_weaponData.m_bulletPrefab == null || m_firePoint == null)
        {
            Debug.LogWarning("弾または発射位置が未設定です");
            return;
        }

        GameObject bullet = Instantiate(m_weaponData.m_bulletPrefab, m_firePoint.position, m_firePoint.rotation);
        SetupBullet(bullet);
    }

    /// <summary>
    /// 弾に色や設定を適用
    /// </summary>
    private void SetupBullet(GameObject bullet)
    {
        if (bullet.TryGetComponent<Bullet>(out Bullet bulletComp))
        {
            bulletComp.SetColor(Color.red); // 必要に応じて変更
        }
    }

    /// <summary>
    /// チャージ段階を1つ進める（最大チャージ回数まで）
    /// </summary>
    public void Charge()
    {
        if (m_triggerOn || m_chargeCount >= m_weaponData.m_maxChargeCount)
            return;

        m_chargeCount++;

        if (m_ChargeEffect)
        {
            GameObject effect = Instantiate(m_ChargeEffect, transform.position, transform.rotation);
            Destroy(effect, m_ChargeEffectDelTime);
        }
    }
}
