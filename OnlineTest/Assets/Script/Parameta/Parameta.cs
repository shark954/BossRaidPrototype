using UnityEngine;
using Mirror;
using TMPro;

public class Parameta : NetworkBehaviour
{
    [SyncVar]
    [Header("�w�c")]
    public string m_team;
    
    [SyncVar(hook =nameof(OnHpChanged))]
    [Header("�̗�")]
    public int m_hp;
    [SyncVar]
    [Header("�ő�̗�")]
    public int m_Maxhp;

    [Header("���S����t���O")]
    public bool m_death;

    [Header("���񂾂Ƃ��̃A�j���[�V����")]
    public Animator m_animator;
    [Header("�G�t�F�N�g")]
    public GameObject m_effect;
    [Header("�G�t�F�N�g���Ŏ���")]
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
                m_hp -= 10; // hook���Ă΂�A�X���C�_�[������
        }
    }

    public void HpReset()
    {
        m_hp = m_Maxhp;
        m_death = false;
    }

    [Server]//�T�[�o�[�ŏ���
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
                    Debug.Log("HP��0�ɂȂ�����[�[");
                }
            }
        }
        return flag;
    }

    void OnHpChanged(int oldHp, int newHp)
    {
        if (m_hpBar != null)
        {
            m_hpBar.HpCeack(); // UI�X�V�Ȃ�
        }
    }

    public void Die(float destroyTime)
    {
        if (!this.gameObject.CompareTag("PlayerDummy"))
            Destroy(this.gameObject, destroyTime);
        Debug.Log("������");
    }


    private void OnDestroy()
    {
        if (!m_effect)
            return;
        GameObject Dummy = Instantiate(m_effect, transform.position, transform.rotation);
        Debug.Log("�G�t�F�N�g");
        Destroy(Dummy, m_effectdel);
    }
}
