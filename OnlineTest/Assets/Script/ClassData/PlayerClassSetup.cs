using UnityEngine;
using Mirror;

public class PlayerClassSetup : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnClassChanged))]
    public int m_classID;

    public ClassDatabase m_database;
    private ClassData m_classData;

    public GameObject m_weaponSlot;
    public Animator m_animator;

    public float m_maxHP;
    public float m_moveSpeed;
    public float m_attackPower;

    public override void OnStartLocalPlayer()
    {
        // クライアントが自分のクラスIDをサーバーに送る（例: 事前にUI選択で保存）
        CmdSetClass(PlayerPrefs.GetInt("SelectedClassID", 0));
    }

    [Command]
    void CmdSetClass(int id)
    {
        m_classID = id; // これでSyncVarが全クライアントに伝わる
    }

    void OnClassChanged(int oldID, int newID)
    {
        m_classData = m_database.GetClassByID(newID);
        if (m_classData == null) return;

        // パラメータ反映
        m_maxHP = m_classData.m_maxHP;
        m_moveSpeed = m_classData.m_moveSpeed;
        m_attackPower = m_classData.m_attackPower;

        // 武器装備
        if (m_weaponSlot.transform.childCount > 0)
            Destroy(m_weaponSlot.transform.GetChild(0).gameObject);
        Instantiate(m_classData.m_weaponPrefab, m_weaponSlot.transform);

        // アニメーター適用
        if (m_animator != null)
            m_animator.runtimeAnimatorController = m_classData.m_animator;
    }
}
