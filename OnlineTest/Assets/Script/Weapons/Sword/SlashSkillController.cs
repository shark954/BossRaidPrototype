using UnityEngine;
using Mirror;

public class SlashSkillController : NetworkBehaviour
{
    public GameObject m_hitboxPrefab; // �����蔻��I�u�W�F�N�g�i�T�[�o�[�p�j
    public GameObject m_visualEffect; // �����ڂ����̃G�t�F�N�g�i���[�J���p�j

    public Transform m_firePoint; // ���ˈʒu

    [Command]
    public void CmdFireSlash()
    {
        GameObject hitbox = Instantiate(m_hitboxPrefab, m_firePoint.position, m_firePoint.rotation);
        NetworkServer.Spawn(hitbox);
    }

    [ClientRpc]
    public void RpcPlayEffect()
    {
        if (m_visualEffect)
        {
            GameObject effect = Instantiate(m_visualEffect, m_firePoint.position, m_firePoint.rotation);
            Destroy(effect, 2f);
        }
    }

    public void Fire()
    {
        if (!isLocalPlayer) return;

        CmdFireSlash();
        RpcPlayEffect();
    }
}
