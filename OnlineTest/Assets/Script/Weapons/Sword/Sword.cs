using System.Collections;
using UnityEngine;

/// <summary>
/// ������̓���X�N���v�g�B�ߐڍU���A�`���[�W�U���A�a���X�L�����Ǘ��B
/// </summary>
public class Sword : MonoBehaviour, IWeapon
{
    [Header("�U���ݒ�")]
    public int damage;                     // ��{�_���[�W
    public float addpower;                // �`���[�W�ǉ��_���[�W
    public int charge_count;              // ���݂̃`���[�W�i�K
    public int maxcharge_count;           // �ő�`���[�W�i�K

    [Header("�G�t�F�N�g�֘A")]
    public GameObject attackEffect;       // �U���G�t�F�N�g
    public float attackEffect_deltime;    // �U���G�t�F�N�g�̎�������
    public GameObject chargeEffect;       // �`���[�W�G�t�F�N�g
    public float chargeEffect_deltime;    // �`���[�W�G�t�F�N�g�̎�������

    // IWeapon�C���^�[�t�F�[�X�̃v���p�e�B����
    public int Damage => damage;
    public float AddPower => addpower;
    public int ChargeCount { get => charge_count; set => charge_count = value; }
    public int MaxChargeCount { get => maxcharge_count; set => maxcharge_count = value; }
    public GameObject AttackEffect => attackEffect;
    public float AttackEffectDelTime => attackEffect_deltime;
    public GameObject ChargeEffect => chargeEffect;
    public float ChargeEffectDelTime => chargeEffect_deltime;

    [Header("�����蔻��")]
    [SerializeField] private BoxCollider m_blead; // �n�̔���
    //[SerializeField] private BoxCollider m_sword; // ���̔���i���g�p�j

    [Header("�X�N���v�g")]
    public AttackCollider m_attackCollider; // �U������N���X�ւ̎Q��
    private Rigidbody m_rb;

    public bool triggerOn; // ���O�ŗp�ӂ���

    [Header("�����`�[����")]
    public string m_team;

    [Header("�X�L���F�a����΂�")]
    [SerializeField] private SlashSkillController m_slashSkillController;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
       
        //if (m_sword) m_sword.enabled = false;
    }

   

    public void Use()
    {
        // �ʏ�U���̏����������ɏ���
        Debug.Log("���ōU���I");
        // ��: GetandDrop(true); �ȂǕK�v�ȏ����������ɏ����Ă��悢
    }

    public WeaponType GetWeaponType()
    {
        return WeaponType.Sword;
    }

    /// <summary>
    /// �������a���X�L���i�X���b�V�����΂��j
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
    /// �`���[�W�����i�`���[�W�i�K�𑝂₵�A�G�t�F�N�g��\���j
    /// </summary>
    public void Charge()
    {
        if (triggerOn) return;

        if (triggerOn)
        {
            triggerOn = false;

            if (charge_count >= maxcharge_count)
            {
                // �ő�`���[�W�i�K�ɒB���Ă���
                return;
            }

            charge_count++;
            GameObject Dummy = Instantiate(chargeEffect, transform.position, transform.rotation);
            Destroy(Dummy, chargeEffect_deltime);
        }
    }
}
