
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeInOut : MonoBehaviour
{
    [Header("色はスクリプトで設定")]
    public float m_alpha;

    [Header("フェード用のImage")]
    public Image m_fade;
    [Header("タイトル")]
    public TextMeshProUGUI m_text;
    [Header("スタートボタン")]
    public TextMeshProUGUI m_text2;

    [Header("true:フェードイン,false:フェードアウト")]
    public bool m_flag;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_fade)
        {
            Fade();
        }
    }

    public void Fade()
    {
        //m_flagがtrueでフェードイン
        if (m_flag)
        {
            m_alpha -= Time.deltaTime;
            if (m_alpha <= 0)
                m_alpha = 0;

        }
        //m_flagがfalseでフェードアウト
        else
        {
            m_alpha += Time.deltaTime;
            if (m_alpha >= 1)
                m_alpha = 1;
        }

        m_fade.color = new Color(1.0f, 1.0f, 1.0f, m_alpha);

        if (m_text)
            m_text.color = new Color(1.0f, 0.0f, 0.0f, m_alpha);
        if (m_text2)
            m_text2.color = m_text.color;
    }
}
