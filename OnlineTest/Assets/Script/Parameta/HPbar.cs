using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    public Slider m_hpBar;

    public TextMeshProUGUI m_teamText; // �ǉ��I

    [SerializeField, Header("�p�����[�^")]
    private Parameta m_parameta;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Parameta���ŌĂяo���Đݒ肷��
    public void SetParameta(Parameta param)
    {
        m_parameta = param;
        m_hpBar.maxValue = m_parameta.m_Maxhp;
        m_hpBar.value = m_parameta.m_hp;

        // �`�[�����\���i�l�b�g���[�N�����ς݂̒l�j
        if (m_teamText != null)
        {
            m_teamText.text = m_parameta.m_team;
        }
    }

    public void HpCeack()
    {
        m_hpBar.value = m_parameta.m_hp;
    }
}
