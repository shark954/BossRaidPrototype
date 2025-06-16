using UnityEngine;
using Mirror;

/// <summary>
/// �K���i�[�̕���X�N���v�g�B
/// �e�ۂ̔��ˁA�`���[�W�i�K�̊Ǘ��A�X�L���G�t�F�N�g�Ȃǂ𐧌䂷��B
/// </summary>
public class Gunner : MonoBehaviour, IWeapon
{
    [Header("����f�[�^�iScriptableObject�j")]
    public GunnerClassData m_weaponData; // �K���i�[��p�̕���p�����[�^

    [SerializeField] private Transform m_firePointLeft; //  ��
    [SerializeField] private Transform m_firePointRight; //�@�E
    [SerializeField] private float m_fireInterval = 0.1f;
    private float m_nextFireTime = 0f;


    [Header("���ˈʒu")]
    [SerializeField] private Transform m_firePoint; // �e�̔��ˈʒu�i�e���Ȃǁj

    [Header("�`���[�W���")]
    public int m_chargeCount = 0;       // ���݂̃`���[�W�i�K�i�З͂ɉe���j
    public bool m_triggerOn = false;    // �`���[�W����p�̃g���K�[�i�A���`���[�W�h�~�j

    private bool m_isLeftNext = true;

    void Awake()
    {
        // �������������K�v�Ȃ炱���ɋL�q
    }

    // IWeapon�C���^�[�t�F�[�X�̎���
    public int m_Damage => m_weaponData.m_baseDamage;
    public float m_AddPower => m_weaponData.m_chargeMultiplier;
    public int m_ChargeCount { get => m_chargeCount; set => m_chargeCount = value; }
    public int m_MaxChargeCount { get => m_weaponData.m_maxChargeCount; set => m_weaponData.m_maxChargeCount = value; }
    public GameObject m_AttackEffect => m_weaponData.m_attackEffectPrefab;
    public float m_AttackEffectDelTime => m_weaponData.m_AttackEffectDelTime;
    public GameObject m_ChargeEffect => m_weaponData.m_chargeEffectPrefab;
    public float m_ChargeEffectDelTime => m_weaponData.m_ChargeEffectDelTime;

    public WeaponType GetWeaponType() => WeaponType.Gunner;

    /// <summary>
    /// �ʏ�U���F�e�ۂ𔭎�
    /// </summary>

    public void Use()
    {
        if (Time.time < m_nextFireTime) return;

        Transform firePoint = m_isLeftNext ? m_firePointLeft : m_firePointRight;
        GameObject bullet = Instantiate(m_weaponData.m_bulletPrefab, firePoint.position, firePoint.rotation);
        NetworkServer.Spawn(bullet);

        m_nextFireTime = Time.time + m_fireInterval;
        m_isLeftNext = !m_isLeftNext;
    }

    /// <summary>
    /// �e�𐶐����A�l�b�g���[�N�z���ɔ��˂���
    /// </summary>
    private void FireBullet()
    {
        if (m_weaponData.m_bulletPrefab == null || m_firePoint == null)
        {
            Debug.LogWarning("�e�܂��͔��ˈʒu�����ݒ�ł�");
            return;
        }

        GameObject bullet = Instantiate(m_weaponData.m_bulletPrefab, m_firePoint.position, m_firePoint.rotation);
        NetworkServer.Spawn(bullet); // Mirror���g���đS�N���C�A���g�ɐ���

        if (bullet.TryGetComponent<Bullet>(out Bullet bulletComp))
        {
            bulletComp.SetColor(Color.red); // �e�̐F�ݒ�ȂǁA�K�v�ɉ����Ē���
        }
    }

    /// <summary>
    /// �`���[�W�����i�i�K�I�ɋ����j
    /// </summary>
    public void Charge()
    {
        if (m_triggerOn || m_chargeCount >= m_weaponData.m_maxChargeCount)
            return;

        m_chargeCount++;

        // �`���[�W�G�t�F�N�g�\��
        if (m_ChargeEffect)
        {
            GameObject effect = Instantiate(m_ChargeEffect, transform.position, transform.rotation);
            Destroy(effect, m_ChargeEffectDelTime);
        }
    }
}
