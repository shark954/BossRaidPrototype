using UnityEngine;

/// <summary>
/// 各クラス（職業）の基本情報や能力値を格納するScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NewClassData", menuName = "Game/Class Data")]
public class ClassData : ScriptableObject
{
    [Header("基本情報")]
    public string m_className; // クラス名（例: ソードマン）
    public int m_classID;      // 識別用のID
    public Sprite M_icon;      // クラス選択時に表示するアイコン
    public GameObject m_weaponPrefab;  // 武器のプレハブ
    public RuntimeAnimatorController m_animator; // アニメーター

    [Header("ステータス")]
    public float m_maxHP;
    public float m_moveSpeed;
    public float m_attackPower;
    public float m_attackRange;
    public float m_attackSpeed;
}
