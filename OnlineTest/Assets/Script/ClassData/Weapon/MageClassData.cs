using UnityEngine;

[CreateAssetMenu(menuName = "CharacterClass/Mage")]
public class MageClassData : WeaponClassData
{
    public float m_magicRange = 15f;
    public float m_manaCostPerShot = 10f;
    public GameObject m_projectilePrefab;

    // WeaponClassData���� chargeEffectPrefab �Ɂu���͏W���G�t�F�N�g�v�Ȃǂ�ݒ�
}
