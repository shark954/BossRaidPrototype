using UnityEngine;

[CreateAssetMenu(menuName = "CharacterClass/Mage")]
public class MageClassData : WeaponClassData
{
    public float m_magicRange = 15f;
    public float m_manaCostPerShot = 10f;
    public GameObject m_projectilePrefab;

    // WeaponClassData内の chargeEffectPrefab に「魔力集中エフェクト」などを設定
}
