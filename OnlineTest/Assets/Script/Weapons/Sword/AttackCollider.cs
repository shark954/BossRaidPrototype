using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// �U���p�R���C�_�[: �Փˏ������Ǘ����A�_���[�W��K�p����N���X�B
/// �����^�[�Q�b�g�ɒZ���Ԃŕ�����_���[�W������̂�h���d�g�݂��܂ށB
/// </summary>
public class AttackCollider : MonoBehaviour
{
    // �U�������Đ����邽�߂�AudioSource
    public AudioSource m_audioSource;

    // �`�[�����ʎq�i�����̃`�[�����G�`�[��������ʂ��邽�߁j
    public string m_team;

    public string m_targetTeam;

    // �ŋ߃_���[�W��^�����^�[�Q�b�g���L�^���邽�߂�HashSet
    private HashSet<Collider> m_recentlyHitTargets = new HashSet<Collider>();

    // �^�[�Q�b�g�����G��ԂɂȂ鎞�ԁi�b�P�ʁj
    public float m_invincibilityDuration = 1.0f;

    // ������Ǘ�����C���^�[�t�F�[�X
    private IWeapon m_weapon;

    /// <summary>
    /// �����������B�e�I�u�W�F�N�g���畐������擾����B
    /// </summary>
    void Start()
    {
        m_targetTeam = null;

        //��ԏ�̐e�ɂ��Ă���Iweapon�R���|�[�l���g���擾����
        m_weapon = transform.parent.GetComponentInParent<IWeapon>();
        if (m_weapon == null)
        {
            Debug.LogError("�e�I�u�W�F�N�g�ɕ��킪������܂���I"); // ���킪������Ȃ��ꍇ�ɃG���[���O��\��
        }

        // audioSource �� null �`�F�b�N
        if (m_audioSource == null)
        {
            Debug.LogError("AudioSource ���ݒ肳��Ă��܂���I", this);
        }
    }

    /// <summary>
    /// �U���R���C�_�[�����̃R���C�_�[�ɏՓ˂����ۂɎ��s����鏈���B
    /// </summary>
    /// <param name="other">�Փ˂����R���C�_�[</param>
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {

        // ���킪���ݒ�A�������͑Ώۂ�Parameta�R���|�[�l���g�������Ȃ��ꍇ�͉������Ȃ�
        if (m_weapon == null || !other.TryGetComponent<Parameta>(out Parameta parameta))
            return;

        // ���łɍŋ߃q�b�g�����^�[�Q�b�g�ł���΁A�������X�L�b�v
        if (m_recentlyHitTargets.Contains(other))
            return;

        m_targetTeam = parameta.m_team;

        if (m_team == m_targetTeam || m_targetTeam == null)
        {
            Debug.Log("�^�[�Q�b�g�Ǝ����傪����");
            return;
        }

        // �_���[�W�v�Z: ����̊�{�_���[�W�ƃ`���[�W�ɉ������ǉ��p���[���܂߂�
        int DMG = CalculateDamage(m_weapon.Damage, m_weapon.AddPower, m_weapon.ChargeCount);
        parameta.Hitdamage(DMG, m_team);

        // �ŋ߃q�b�g�����^�[�Q�b�g�̃��X�g�ɒǉ����A���G���Ԃ��J�n
        m_recentlyHitTargets.Add(other);
        StartCoroutine(RemoveFromRecentlyHitTargets(other));

        // �G�t�F�N�g�𐶐����A�U�������Đ�
        if (m_weapon.AttackEffect != null)
        {
            // �U���G�t�F�N�g�𐶐����A��莞�Ԍ�ɔj�󂷂�
            GameObject dummy = Instantiate(m_weapon.AttackEffect, this.transform.position, this.transform.rotation);


            if (m_audioSource != null)
            {
                m_audioSource.Play(); // �����Đ�
            }
            else
            {
                Debug.LogWarning("AudioSource ���ݒ肳��Ă��Ȃ����߁A�U�������Đ��ł��܂���B");
            }

            Destroy(dummy, m_weapon.AttackEffectDelTime);
            // �U����A����̃`���[�W�������Z�b�g
            m_weapon.ChargeCount = 0;
        }
        else
        {
            Debug.LogWarning("AttackEffect ���ݒ肳��Ă��܂���B�G�t�F�N�g�𐶐��ł��܂���B");
        }

        if (other.CompareTag("EnemyAttack"))
        {
            Destroy(other.gameObject);
        }


        m_targetTeam = null;
    }

    private int CalculateDamage(float baseDamage, float addPower, int chargeCount)
    {

        float chargePower = m_weapon.AddPower * m_weapon.ChargeCount;
        float randomFactor = Random.Range((baseDamage * chargePower) / 2, baseDamage * chargePower);
        // ���v�_���[�W���l�̌ܓ����Đ����ɕϊ�
        return Mathf.RoundToInt(baseDamage + randomFactor);
    }

    /// <summary>
    /// �w�莞�Ԍ�ɑΏۂ��u�ŋ߃q�b�g�����^�[�Q�b�g�v�̃��X�g����폜����R���[�`���B
    /// </summary>
    /// <param name="target">���X�g����폜����R���C�_�[</param>
    /// <returns>�R���[�`���̑ҋ@����</returns>
    private IEnumerator RemoveFromRecentlyHitTargets(Collider target)
    {
        // �w�肵�����G���Ԃ����ҋ@
        yield return new WaitForSeconds(m_invincibilityDuration);

        // ���G���Ԍo�ߌ�ɁA�Ώۂ����X�g����폜
        m_recentlyHitTargets.Remove(target);
    }
}
