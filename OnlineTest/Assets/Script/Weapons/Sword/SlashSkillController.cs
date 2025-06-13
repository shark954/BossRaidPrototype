using UnityEngine;
using Mirror;

public class SlashSkillController : NetworkBehaviour
{
    public GameObject hitboxPrefab; // �����蔻��I�u�W�F�N�g�i�T�[�o�[�p�j
    public GameObject visualEffect; // �����ڂ����̃G�t�F�N�g�i���[�J���p�j

    public Transform firePoint; // ���ˈʒu

    [Command]
    public void CmdFireSlash()
    {
        GameObject hitbox = Instantiate(hitboxPrefab, firePoint.position, firePoint.rotation);
        NetworkServer.Spawn(hitbox);
    }

    [ClientRpc]
    public void RpcPlayEffect()
    {
        if (visualEffect)
        {
            GameObject effect = Instantiate(visualEffect, firePoint.position, firePoint.rotation);
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
