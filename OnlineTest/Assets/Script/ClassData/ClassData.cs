using UnityEngine;

/// <summary>
/// �e�N���X�i�E�Ɓj�̊�{����\�͒l���i�[����ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NewClassData", menuName = "Game/Class Data")]
public class ClassData : ScriptableObject
{
    [Header("��{���")]
    public string m_className; // �N���X���i��: �\�[�h�}���j
    public int m_classID;      // ���ʗp��ID
    public Sprite M_icon;      // �N���X�I�����ɕ\������A�C�R��
    public GameObject m_weaponPrefab;  // ����̃v���n�u
    public RuntimeAnimatorController m_animator; // �A�j���[�^�[

    [Header("�X�e�[�^�X")]
    public float m_maxHP;
    public float m_moveSpeed;
    public float m_attackPower;
    public float m_attackRange;
    public float m_attackSpeed;
}
