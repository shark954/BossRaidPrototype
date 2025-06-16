using UnityEngine;

/// <summary>
/// ������̓���X�N���v�g�B�ߐڍU���A�`���[�W�A�a���X�L�����܂ށB
/// WeaponClassData �ɂ���ĕ��퐫�\���O������Ǘ��B
/// </summary>
public class Sword : MonoBehaviour, IWeapon
{
    [Header("����f�[�^�iScriptableObject�j")]
    public WeaponClassData weaponData; // ����̃f�[�^��ێ��iScriptableObject�j

    [Header("�`���[�W�J�E���g")]
    public int chargeCount = 0;              // ���݂̃`���[�W�i�K�i�����J�E���g�j
   

    [Header("�X�L���F�a����΂�")]
    [SerializeField] private SlashSkillController m_slashSkillController;

    [Header("�����蔻��")]
    [SerializeField] private BoxCollider m_blead; // �n�̔���p�R���C�_�[

    [Header("�����`�[����")]
    public string m_team; // �U���Ώۂ𔻕ʂ��邽�߂̃`�[����

    [Header("�U���p�R���|�[�l���g")]
    public AttackCollider m_attackCollider;

    private Rigidbody m_rb;
    public bool triggerOn; // �`���[�W�^�C�~���O����p�t���O

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // ========== IWeapon�C���^�[�t�F�[�X���� ==========
    public int m_Damage => weaponData.m_baseDamage;
    public float m_AddPower => weaponData.m_chargeMultiplier;
    public int m_ChargeCount { get => chargeCount; set => chargeCount = value; }
    public int m_MaxChargeCount { get => weaponData.m_maxChargeCount; set => weaponData.m_maxChargeCount = value; }
    public GameObject m_AttackEffect => weaponData.m_attackEffectPrefab;
    public float m_AttackEffectDelTime => weaponData.m_AttackEffectDelTime; // �G�t�F�N�g�̎������ԁi�K�v�Ȃ�WeaponData�ɒǉ��j
    public GameObject m_ChargeEffect => weaponData.m_chargeEffectPrefab;
    public float m_ChargeEffectDelTime => weaponData.m_ChargeEffectDelTime; // ����

    public WeaponType GetWeaponType() => WeaponType.Sword;

    public void Use()
    {
        // �ʏ�U���̏����i�K�v�Ȃ炱���ɃG�t�F�N�g�\����R���C�_�[ON�Ȃǁj
        Debug.Log("���ōU���I");
    }

    /// <summary>
    /// �������X���b�V���X�L���̎��s
    /// </summary>
    public void UseSlashSkill()
    {
        if (m_slashSkillController != null)
        {
            m_slashSkillController.Fire();
        }
        else
        {
            Debug.LogWarning("SlashSkillController ���ݒ肳��Ă��܂���");
        }
    }

    /// <summary>
    /// �`���[�W�i�K�𑝂₵�A�`���[�W�G�t�F�N�g��\��
    /// </summary>
    public void Charge()
    {
        if (triggerOn || chargeCount >= weaponData.m_maxChargeCount)
            return;

        chargeCount++;

        if (m_ChargeEffect != null)
        {
            GameObject dummy = Instantiate(m_ChargeEffect, transform.position, transform.rotation);
            Destroy(dummy, m_ChargeEffectDelTime);
        }
    }
}
