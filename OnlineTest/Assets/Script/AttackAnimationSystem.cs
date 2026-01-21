using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationSystem : MonoBehaviour
{
    [Header("Animationリンク")]
    public Animator m_Animator;
    [Header("攻撃実行許可フラグ")]
    public bool m_AttackFlag = false;
    [Header("攻撃中の移動管理")]
    public bool m_isAttackAnimationPlaying = false;
    [Header("攻撃時に有効にする当たり判定")]
    public List<Collider> m_attackColliders = new List<Collider>();
   
    private void Awake()
    {
        DisableAttackColliders();
    }

    void Update()
    {
        //攻撃時の判定(マウス入力)
        AttackTrigger();
    }

    /// <summary>
    /// 攻撃時
    /// </summary>
    public void AttackTrigger()
    {
        //Debug.Log(m_Animator.GetInteger("Attack"));
        //Debug.Log(m_AttackFlag);
        //攻撃許可チェック
        if (m_AttackFlag)
        {
            //マウス左ボタンを押す
            if (Input.GetMouseButtonDown(0))
            {
                //Animatorに、【攻撃】のInt値に+1
                m_Animator.SetInteger("Attack", m_Animator.GetInteger("Attack") + 1);
                //攻撃許可を撤回
                m_AttackFlag = false;
            }
        }
    }

    // 攻撃判定ON（アニメーションイベントで呼び出す）
    public void EnableAttackColliders()
    {
        m_isAttackAnimationPlaying = true;
        foreach (var col in m_attackColliders)
        {
            if (col != null) col.enabled = true;
        }
    }

    // 攻撃判定OFF（アニメーションイベントで呼び出す）
    public void DisableAttackColliders()
    {
        m_isAttackAnimationPlaying = false;
        foreach (var col in m_attackColliders)
        {
            if (col != null) col.enabled = false;
        }
    }

    public void CancelAttack()
    {
        if (m_isAttackAnimationPlaying)
        {
            Debug.Log("攻撃キャンセル");
            m_isAttackAnimationPlaying = false;
            m_Animator.SetInteger("Attack", 0);
            DisableAttackColliders();
        }
    }

    /// <summary>
    /// Animator側からの攻撃許可・不許可フラグ
    /// </summary>
    /// <param name="No"> 0 攻撃不許可　1 攻撃許可</param>
    /// 2-1-2　最後のアニメーションは0有
    public void AttackFlagOnOff(int No)
    {
        Debug.Log($"AttackFlagOnOff({No}) called.");
        
        switch (No)
        {
            //初期化・ファーストアタック
            case 0:
                m_Animator.SetInteger("Attack", 0);
                m_AttackFlag = true;
                break;
            //攻撃可能・入力可
            case 1:
                m_AttackFlag = true;
                break;
            //持続攻撃・入力不可
            case 2:
                m_AttackFlag = false;
                break;
        }
    }

    /// <summary>
    /// SE出し(ダミー)
    /// </summary>
    /// <param name="SE"></param>
    public void SEPop(GameObject SE)
    {
        GameObject Dummy = Instantiate(SE, transform.position, Quaternion.identity);
        Destroy(Dummy,2.0f);
    }
}
