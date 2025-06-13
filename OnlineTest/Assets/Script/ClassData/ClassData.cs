using UnityEngine;

/// <summary>
/// 各クラス（職業）の基本情報や能力値を格納するScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NewClassData", menuName = "Game/Class Data")]
public class ClassData : ScriptableObject
{
    [Header("基本情報")]
    public string className; // クラス名（例: ソードマン）
    public int classID;      // 識別用のID
    public Sprite icon;      // クラス選択時に表示するアイコン
    public GameObject weaponPrefab;  // 武器のプレハブ
    public RuntimeAnimatorController animator; // アニメーター

    [Header("ステータス")]
    public float maxHP;
    public float moveSpeed;
    public float attackPower;
    public float attackRange;
    public float attackSpeed;
}
