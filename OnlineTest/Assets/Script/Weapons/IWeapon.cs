using UnityEngine;

/// <summary>
/// すべての武器に共通するインターフェース。
/// ステータス情報とアクションの両方を含む。
/// </summary>
public interface IWeapon
{
    // === ステータス関連 ===

    /// <summary>基本ダメージ</summary>
    int m_Damage { get; }

    /// <summary>チャージなどで加算される追加攻撃力</summary>
    float m_AddPower { get; }

    /// <summary>現在のチャージ段階</summary>
    int m_ChargeCount { get; set; }

    /// <summary>チャージ段階の最大値</summary>
    int m_MaxChargeCount { get; set; }

    /// <summary>攻撃ヒット時に生成するエフェクト</summary>
    GameObject m_AttackEffect { get; }

    /// <summary>攻撃エフェクトの消滅時間</summary>
    float m_AttackEffectDelTime { get; }

    /// <summary>チャージ時に生成するエフェクト</summary>
    GameObject m_ChargeEffect { get; }

    /// <summary>チャージエフェクトの消滅時間</summary>
    float m_ChargeEffectDelTime { get; }

    // === アクション関連 ===

    /// <summary>武器を使用する（通常攻撃など）</summary>
    void Use();

    /// <summary>武器のタイプ（剣・銃・魔法など）を取得する</summary>
    WeaponType GetWeaponType();
}
