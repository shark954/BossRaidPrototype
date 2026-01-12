using UnityEngine;

/// <summary>
/// クラス情報の読み込みとステータス/武器/アニメーター設定（シングルプレイ版）
/// </summary>
public class PlayerClassSetup : MonoBehaviour
{
    public int m_classID; // プレイヤーの選択クラスID

    public ClassDatabase m_database;
    private ClassData m_classData;

    public GameObject m_weaponSlot;
    public Animator m_animator;

    public float m_maxHP;
    public float m_moveSpeed;
    public float m_attackPower;

    void Start()
    {
        // UIなどで保存したクラスIDを取得（シングルプレイなので自分で設定）
        m_classID = PlayerPrefs.GetInt("SelectedClassID", 0);
        ApplyClass(m_classID);
    }

    /// <summary>
    /// クラスIDに基づいてステータスや装備をセット
    /// </summary>
    void ApplyClass(int id)
    {
        m_classData = m_database.GetClassByID(id);
        if (m_classData == null) return;

        // パラメータを設定
        m_maxHP = m_classData.m_maxHP;
        m_moveSpeed = m_classData.m_moveSpeed;
        m_attackPower = m_classData.m_attackPower;

        // 武器を装備
        if (m_weaponSlot.transform.childCount > 0)
            Destroy(m_weaponSlot.transform.GetChild(0).gameObject);
        Instantiate(m_classData.m_weaponPrefab, m_weaponSlot.transform);

        // アニメーターを適用
        if (m_animator != null)
            m_animator.runtimeAnimatorController = m_classData.m_animator;
    }
}
