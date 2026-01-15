using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    public Slider m_hpBar;

  

    [SerializeField, Header("パラメータ")]
    private Parameta m_parameta;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Parameta側で呼び出して設定する
    public void SetParameta(Parameta param)
    {
        m_parameta = param;
        m_hpBar.maxValue = m_parameta.m_Maxhp;
        m_hpBar.value = m_parameta.m_hp;

      
    }

    public void HpCeack()
    {
        m_hpBar.value = m_parameta.m_hp;
    }
}
