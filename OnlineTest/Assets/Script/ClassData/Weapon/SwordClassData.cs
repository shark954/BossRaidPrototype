using UnityEngine;

[CreateAssetMenu(menuName = "CharacterClass/Sword")]
public class SwordClassData : WeaponClassData
{
    public float m_attackSpeed;               // 通常攻撃の速度
    public float m_comboResetTime;            // コンボのリセット時間
    public float m_slashSkillCooldown;        // スキル（斬撃飛ばし）のクールダウン時間
    public GameObject m_slashSkillPrefab;     // スラッシュスキルのエフェクト or プレハブ
}
