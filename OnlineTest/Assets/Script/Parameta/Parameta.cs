using UnityEngine;
using TMPro;

/// <summary>
/// プレイヤーのステータス管理（シングルプレイ版）
/// </summary>
public class Parameta : MonoBehaviour
{
    [Header("陣営")]
    public string m_team;

    [Header("体力")]
    public int m_hp;

    [Header("最大体力")]
    public int m_Maxhp;

    [Header("死亡判定フラグ")]
    public bool m_death;

    [Header("死んだときのアニメーション")]
    public Animator m_animator;

    [Header("エフェクト")]
    public GameObject m_effect;

    [Header("エフェクト消滅時間")]
    public float m_effectdel;

    public HPbar m_hpBar;

    public GameManager m_gameManager;

    void Start()
    {
        HpReset();

        if (m_hpBar != null)
        {
            m_hpBar.SetParameta(this); // HPバーに自身をセット
        }
    }

    void Update()
    {
        // テスト用：Hキーで10ダメージ
        if (Input.GetKeyDown(KeyCode.H))
        {
            ApplyDamage(10, "Enemy");
        }
    }

    /// <summary>
    /// HPを最大にリセットし、死亡状態も初期化
    /// </summary>
    public void HpReset()
    {
        m_hp = m_Maxhp;
        m_death = false;
        OnHpChanged(); // HPバー更新
    }

    /// <summary>
    /// ダメージを受けて体力を減らす。敵味方の判定あり。
    /// </summary>
    public bool ApplyDamage(int damage, string attackerTeam)
    {
        if (m_death) return false;
        if (m_team == attackerTeam) return false;

        m_hp -= damage;
        if (m_hp <= 0)
        {
            m_hp = 0;
            m_death = true;

            // 死亡アニメーションなど
            if (m_animator != null)
                m_animator.SetBool("Death", true);

            Debug.Log("HPが0になったよーー");
        }

        OnHpChanged(); // HPバー更新
        return true;
    }

    /// <summary>
    /// HPが変化したときに呼ぶ。UIなどを更新。
    /// </summary>
    void OnHpChanged()
    {
        if (m_hpBar != null)
        {
            m_hpBar.HpCeack(); // UI反映など
        }
    }

    /// <summary>
    /// 一定時間後にプレイヤーを削除
    /// </summary>
    public void Die(float destroyTime)
    {
        if (!this.gameObject.CompareTag("PlayerDummy"))
            Destroy(this.gameObject, destroyTime);

        Debug.Log("消えた");
    }

    /// <summary>
    /// 死亡時のエフェクトを生成して削除
    /// </summary>
    private void OnDestroy()
    {
        if (!m_effect) return;

        GameObject Dummy = Instantiate(m_effect, transform.position, transform.rotation);
        Debug.Log("エフェクト");
        Destroy(Dummy, m_effectdel);
    }
}
