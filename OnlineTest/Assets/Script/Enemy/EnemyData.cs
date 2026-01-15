using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public EnemyType m_enemyType; // “G‚ÌŽí—Þ
    public GameObject m_prefab; // “G‚ÌPrefab
    public int m_health; // “G‚ÌHP
    public float m_speed; // “G‚ÌˆÚ“®‘¬“x
}
