using System.Collections;
using UnityEngine;

/// <summary>
/// 剣武器の動作スクリプト。近接攻撃、チャージ攻撃、斬撃スキルを管理。
/// </summary>
public class Sword : MonoBehaviour, IWeapon
{
    [Header("攻撃設定")]
    public int damage;                     // 基本ダメージ
    public float addpower;                // チャージ追加ダメージ
    public int charge_count;              // 現在のチャージ段階
    public int maxcharge_count;           // 最大チャージ段階

    [Header("エフェクト関連")]
    public GameObject attackEffect;       // 攻撃エフェクト
    public float attackEffect_deltime;    // 攻撃エフェクトの持続時間
    public GameObject chargeEffect;       // チャージエフェクト
    public float chargeEffect_deltime;    // チャージエフェクトの持続時間

    // IWeaponインターフェースのプロパティ実装
    public int Damage => damage;
    public float AddPower => addpower;
    public int ChargeCount { get => charge_count; set => charge_count = value; }
    public int MaxChargeCount { get => maxcharge_count; set => maxcharge_count = value; }
    public GameObject AttackEffect => attackEffect;
    public float AttackEffectDelTime => attackEffect_deltime;
    public GameObject ChargeEffect => chargeEffect;
    public float ChargeEffectDelTime => chargeEffect_deltime;

    [Header("当たり判定")]
    [SerializeField] private BoxCollider m_blead; // 刃の判定
    //[SerializeField] private BoxCollider m_sword; // 柄の判定（未使用）

    [Header("スクリプト")]
    public AttackCollider m_attackCollider; // 攻撃判定クラスへの参照
    private Rigidbody m_rb;

    public bool triggerOn; // 自前で用意する

    [Header("所属チーム名")]
    public string m_team;

    [Header("スキル：斬撃飛ばし")]
    [SerializeField] private SlashSkillController m_slashSkillController;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
       
        //if (m_sword) m_sword.enabled = false;
    }

   

    public void Use()
    {
        // 通常攻撃の処理をここに書く
        Debug.Log("剣で攻撃！");
        // 例: GetandDrop(true); など必要な処理をここに書いてもよい
    }

    public WeaponType GetWeaponType()
    {
        return WeaponType.Sword;
    }

    /// <summary>
    /// 遠距離斬撃スキル（スラッシュを飛ばす）
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
    /// チャージ処理（チャージ段階を増やし、エフェクトを表示）
    /// </summary>
    public void Charge()
    {
        if (triggerOn) return;

        if (triggerOn)
        {
            triggerOn = false;

            if (charge_count >= maxcharge_count)
            {
                // 最大チャージ段階に達している
                return;
            }

            charge_count++;
            GameObject Dummy = Instantiate(chargeEffect, transform.position, transform.rotation);
            Destroy(Dummy, chargeEffect_deltime);
        }
    }
}
