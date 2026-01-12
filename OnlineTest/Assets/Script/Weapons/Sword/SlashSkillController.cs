using UnityEngine;

/// <summary>
/// スラッシュスキルのエフェクトと当たり判定処理（シングルプレイ版）
/// </summary>
public class SlashSkillController : MonoBehaviour
{
    public GameObject m_hitboxPrefab;   // 当たり判定用オブジェクト
    public GameObject m_visualEffect;   // 見た目用のスラッシュエフェクト

    public Transform m_firePoint;       // スキルの発動位置

    /// <summary>
    /// スキル発動時に呼ばれる
    /// </summary>
    public void Fire()
    {
        FireSlash();     // 当たり判定を生成
        PlayEffect();    // エフェクトを再生
    }

    /// <summary>
    /// 当たり判定オブジェクトを生成
    /// </summary>
    void FireSlash()
    {
        if (m_hitboxPrefab != null)
        {
            Instantiate(m_hitboxPrefab, m_firePoint.position, m_firePoint.rotation);
        }
    }

    /// <summary>
    /// 見た目用エフェクトを再生
    /// </summary>
    void PlayEffect()
    {
        if (m_visualEffect != null)
        {
            GameObject effect = Instantiate(m_visualEffect, m_firePoint.position, m_firePoint.rotation);
            Destroy(effect, 2f); // 一定時間で破棄
        }
    }
}
