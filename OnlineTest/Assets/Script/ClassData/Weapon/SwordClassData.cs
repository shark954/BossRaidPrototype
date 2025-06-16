using UnityEngine;

[CreateAssetMenu(menuName = "CharacterClass/Sword")]
public class SwordClassData : WeaponClassData
{
    public float m_attackSpeed;               // �ʏ�U���̑��x
    public float m_comboResetTime;            // �R���{�̃��Z�b�g����
    public float m_slashSkillCooldown;        // �X�L���i�a����΂��j�̃N�[���_�E������
    public GameObject m_slashSkillPrefab;     // �X���b�V���X�L���̃G�t�F�N�g or �v���n�u
}
