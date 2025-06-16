using UnityEngine;

public abstract class WeaponClassData : ScriptableObject
{
    [Header("基本情報")]
    public string m_className;                // クラス名（例: Sword, Gunner）

    [Header("ダメージ設定")]
    public int m_baseDamage;                  // 基本ダメージ
    public float m_chargeMultiplier;          // チャージ倍率

    [Header("チャージ設定")]
    public int m_maxChargeCount = 3;          // 最大チャージ段階
    public float m_ChargeEffectDelTime;       // チャージエフェクトの削除までの時間 

    [Header("エフェクト設定")]
    public float m_AttackEffectDelTime;       // 攻撃エフェクトの削除までの時間
    public GameObject m_attackEffectPrefab;   // 通常攻撃エフェクト
    public GameObject m_chargeEffectPrefab;   // チャージ時のエフェクト
}
