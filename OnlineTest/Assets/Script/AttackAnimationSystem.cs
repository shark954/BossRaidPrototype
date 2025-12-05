using UnityEngine;

public class AttackAnimationSystem : MonoBehaviour
{
    [Header("Animationリンク")]
    public Animator m_Animator;
    [Header("攻撃実行許可フラグ")]
    public bool m_AttackFlag = false;

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
        //攻撃許可チェック
        if (m_AttackFlag)
        {
            //Animatorに、【攻撃】のInt値に+1
            m_Animator.SetInteger("Attack", m_Animator.GetInteger("Attack") + 1);
            //攻撃許可を撤回
            m_AttackFlag = false;
        }
    }

    // イベント用に作った、引数なしの関数
    public void AnimEvent_EnableAttack() => AttackFlagOnOff(1);
    public void AnimEvent_DisableAttack() => AttackFlagOnOff(2);
    public void AnimEvent_ResetAttack() => AttackFlagOnOff(0);

    /// <summary>
    /// Animator側からの攻撃許可・不許可フラグ
    /// </summary>
    /// <param name="No"> 0 攻撃不許可　1 攻撃許可</param>
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
