using UnityEngine;
using Mirror;

public class SlashSkillController : NetworkBehaviour
{
    public GameObject m_hitboxPrefab; // 当たり判定オブジェクト（サーバー用）
    public GameObject m_visualEffect; // 見た目だけのエフェクト（ローカル用）

    public Transform m_firePoint; // 発射位置

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
