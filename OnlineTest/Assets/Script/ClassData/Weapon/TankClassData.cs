using UnityEngine;

[CreateAssetMenu(menuName = "CharacterClass/Tank")]
public class TankClassData : WeaponClassData
{
    public float m_defenseMultiplier = 1.5f;         // ��_���y��
    public float m_aggroBonus = 2.0f;                // �w�C�g�{��
    public GameObject m_shieldEffectPrefab;          // �X�L�����̏����o
    // WeaponClassData���� chargeEffectPrefab �Ɂu�����G�t�F�N�g�v�Ȃǂ����蓖�Ă�
}
