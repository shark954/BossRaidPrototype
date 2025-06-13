using UnityEngine;

/// <summary>
/// すべての武器に共通するインターフェース。
/// ステータス情報とアクションの両方を含む。
/// </summary>
public interface IWeapon
{
    // === ステータス関連 ===

    /// <summary>基本ダメージ</summary>
    int Damage { get; }

    /// <summary>チャージなどで加算される追加攻撃力</summary>
    float AddPower { get; }

    /// <summary>現在のチャージ段階</summary>
    int ChargeCount { get; set; }

    /// <summary>チャージ段階の最大値</summary>
    int MaxChargeCount { get; set; }

    /// <summary>攻撃ヒット時に生成するエフェクト</summary>
    GameObject AttackEffect { get; }

    /// <summary>攻撃エフェクトの消滅時間</summary>
    float AttackEffectDelTime { get; }

    /// <summary>チャージ時に生成するエフェクト</summary>
    GameObject ChargeEffect { get; }

    /// <summary>チャージエフェクトの消滅時間</summary>
    float ChargeEffectDelTime { get; }

    // === アクション関連 ===

    /// <summary>武器を使用する（通常攻撃など）</summary>
    void Use();

    /// <summary>武器のタイプ（剣・銃・魔法など）を取得する</summary>
    WeaponType GetWeaponType();
}
