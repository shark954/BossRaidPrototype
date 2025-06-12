using UnityEngine;
using Mirror;
using TMPro;

public class Parameta : NetworkBehaviour
{
    [SyncVar]
    [Header("陣営")]
    public string m_team;
    
    [SyncVar(hook =nameof(OnHpChanged))]
    [Header("体力")]
    public int m_hp;
    [SyncVar]
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

    //public GameData gameData;
    public GameManager m_gameManager;

    // Start is called before the first frame update
    void Start()
    {
        HpReset();

        if (isLocalPlayer && m_hpBar != null)
        {
            m_hpBar.SetParameta(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.H))
        {
            if (isServer)
                m_hp -= 10; // hookが呼ばれ、スライダーも動く
        }
    }

    public void HpReset()
    {
        m_hp = m_Maxhp;
        m_death = false;
    }

    [Server]//サーバーで処理
    public bool Hitdamage(int damage, string teams)
    {
        bool flag = false;
        if (!m_death)
        {
            if (m_team != teams)
            {
                flag = true;
                m_hp -= damage;
                if (m_hpBar != null)
                {
                    //m_hpBar.HpCeack();
                }
                if (m_hp <= 0)
                {
                    m_hp = 0;
                    m_death = true;
                    //animator.SetBool("Death", death);
                    Debug.Log("HPが0になったよーー");
                }
            }
        }
        return flag;
    }

    void OnHpChanged(int oldHp, int newHp)
    {
        if (m_hpBar != null)
        {
            m_hpBar.HpCeack(); // UI更新など
        }
    }

    public void Die(float destroyTime)
    {
        if (!this.gameObject.CompareTag("PlayerDummy"))
            Destroy(this.gameObject, destroyTime);
        Debug.Log("消えた");
    }


    private void OnDestroy()
    {
        if (!m_effect)
            return;
        GameObject Dummy = Instantiate(m_effect, transform.position, transform.rotation);
        Debug.Log("エフェクト");
        Destroy(Dummy, m_effectdel);
    }
}
