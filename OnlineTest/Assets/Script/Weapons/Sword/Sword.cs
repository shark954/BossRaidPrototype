using UnityEngine;

/// <summary>
/// 剣武器の動作スクリプト。近接攻撃、チャージ、斬撃スキルを含む。
/// WeaponClassData によって武器性能を外部から管理。
/// </summary>
public class Sword : MonoBehaviour, IWeapon
{
    [Header("武器データ（ScriptableObject）")]
    public SwordClassData m_weaponData; // 武器のデータを保持（ScriptableObject）

    [Header("チャージカウント")]
    public int m_chargeCount = 0;              // 現在のチャージ段階（内部カウント）
   
    [Header("当たり判定")]
    [SerializeField] private BoxCollider m_blead; // 刃の判定用コライダー

    [Header("所属チーム名")]
    public string m_team; // 攻撃対象を判別するためのチーム名

    [Header("攻撃用コンポーネント")]
    public AttackCollider m_attackCollider;

    private Rigidbody m_rb;
   
    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // ========== IWeaponインターフェース実装 ==========
    public int m_Damage => m_weaponData.m_baseDamage;
    public float m_AddPower => m_weaponData.m_chargeMultiplier;
    public int m_ChargeCount { get => m_chargeCount; set => m_chargeCount = value; }
    public int m_MaxChargeCount { get => m_weaponData.m_maxChargeCount; set => m_weaponData.m_maxChargeCount = value; }
    public GameObject m_AttackEffect => m_weaponData.m_attackEffectPrefab;
    public float m_AttackEffectDelTime => m_weaponData.m_AttackEffectDelTime; // エフェクトの持続時間（必要ならWeaponDataに追加）
    public GameObject m_ChargeEffect => m_weaponData.m_chargeEffectPrefab;
    public float m_ChargeEffectDelTime => m_weaponData.m_ChargeEffectDelTime; // 同上

    public WeaponType GetWeaponType() => WeaponType.Sword;

    public void Use()
    {
        // 通常攻撃の処理（必要ならここにエフェクト表示やコライダーONなど）
        Debug.Log("剣で攻撃！");
    }

  
}
