using UnityEngine;

public abstract class WeaponClassData : ScriptableObject
{
    [Header("��{���")]
    public string m_className;                // �N���X���i��: Sword, Gunner�j

    [Header("�_���[�W�ݒ�")]
    public int m_baseDamage;                  // ��{�_���[�W
    public float m_chargeMultiplier;          // �`���[�W�{��

    [Header("�`���[�W�ݒ�")]
    public int m_maxChargeCount = 3;          // �ő�`���[�W�i�K
    public float m_ChargeEffectDelTime;       // �`���[�W�G�t�F�N�g�̍폜�܂ł̎��� 

    [Header("�G�t�F�N�g�ݒ�")]
    public float m_AttackEffectDelTime;       // �U���G�t�F�N�g�̍폜�܂ł̎���
    public GameObject m_attackEffectPrefab;   // �ʏ�U���G�t�F�N�g
    public GameObject m_chargeEffectPrefab;   // �`���[�W���̃G�t�F�N�g
}
