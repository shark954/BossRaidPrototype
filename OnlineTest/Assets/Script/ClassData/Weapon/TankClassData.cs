using UnityEngine;

[CreateAssetMenu(menuName = "CharacterClass/Tank")]
public class TankClassData : WeaponClassData
{
    public float m_defenseMultiplier = 1.5f;         // 被ダメ軽減
    public float m_aggroBonus = 2.0f;                // ヘイト倍率
    public GameObject m_shieldEffectPrefab;          // スキル時の盾演出
    // WeaponClassData内の chargeEffectPrefab に「挑発エフェクト」などを割り当てる
}
