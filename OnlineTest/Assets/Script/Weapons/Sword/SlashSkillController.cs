using UnityEngine;
using Mirror;

public class SlashSkillController : NetworkBehaviour
{
    public GameObject hitboxPrefab; // 当たり判定オブジェクト（サーバー用）
    public GameObject visualEffect; // 見た目だけのエフェクト（ローカル用）

    public Transform firePoint; // 発射位置

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
