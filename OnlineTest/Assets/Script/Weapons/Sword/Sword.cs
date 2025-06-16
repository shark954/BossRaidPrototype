using UnityEngine;

/// <summary>
/// 剣武器の動作スクリプト。近接攻撃、チャージ、斬撃スキルを含む。
/// WeaponClassData によって武器性能を外部から管理。
/// </summary>
public class Sword : MonoBehaviour, IWeapon
{
    [Header("武器データ（ScriptableObject）")]
    public WeaponClassData weaponData; // 武器のデータを保持（ScriptableObject）

    [Header("チャージカウント")]
    public int chargeCount = 0;              // 現在のチャージ段階（内部カウント）
   

    [Header("スキル：斬撃飛ばし")]
    [SerializeField] private SlashSkillController m_slashSkillController;

    [Header("当たり判定")]
    [SerializeField] private BoxCollider m_blead; // 刃の判定用コライダー

    [Header("所属チーム名")]
    public string m_team; // 攻撃対象を判別するためのチーム名

    [Header("攻撃用コンポーネント")]
    public AttackCollider m_attackCollider;

    private Rigidbody m_rb;
    public bool triggerOn; // チャージタイミング制御用フラグ

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // ========== IWeaponインターフェース実装 ==========
    public int m_Damage => weaponData.m_baseDamage;
    public float m_AddPower => weaponData.m_chargeMultiplier;
    public int m_ChargeCount { get => chargeCount; set => chargeCount = value; }
    public int m_MaxChargeCount { get => weaponData.m_maxChargeCount; set => weaponData.m_maxChargeCount = value; }
    public GameObject m_AttackEffect => weaponData.m_attackEffectPrefab;
    public float m_AttackEffectDelTime => weaponData.m_AttackEffectDelTime; // エフェクトの持続時間（必要ならWeaponDataに追加）
    public GameObject m_ChargeEffect => weaponData.m_chargeEffectPrefab;
    public float m_ChargeEffectDelTime => weaponData.m_ChargeEffectDelTime; // 同上

    public WeaponType GetWeaponType() => WeaponType.Sword;

    public void Use()
    {
        // 通常攻撃の処理（必要ならここにエフェクト表示やコライダーONなど）
        Debug.Log("剣で攻撃！");
    }

    /// <summary>
    /// 遠距離スラッシュスキルの実行
    /// </summary>
    public void UseSlashSkill()
    {
        if (m_slashSkillController != null)
        {
            m_slashSkillController.Fire();
        }
        else
        {
            Debug.LogWarning("SlashSkillController が設定されていません");
        }
    }

    /// <summary>
    /// チャージ段階を増やし、チャージエフェクトを表示
    /// </summary>
    public void Charge()
    {
        if (triggerOn || chargeCount >= weaponData.m_maxChargeCount)
            return;

        chargeCount++;

        if (m_ChargeEffect != null)
        {
            GameObject dummy = Instantiate(m_ChargeEffect, transform.position, transform.rotation);
            Destroy(dummy, m_ChargeEffectDelTime);
        }
    }
}
