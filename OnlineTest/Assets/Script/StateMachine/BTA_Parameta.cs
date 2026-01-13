using UnityEngine;
using StateMachineAI;
public class BTA_Parameta : MonoBehaviour
{
    public BattleTesterAI m_BTA;
    public bool m_ArmsOnFlag;
    public int m_Hp;
    private void Start()
    {
        m_BTA = GetComponent<BattleTesterAI>();
        m_ArmsOnFlag = false;
    }
    public bool TakeDamage(int Damage)
    {
        bool Flag = false;
        if (m_Hp>0)
        {
            int RND_DorH = Random.Range(0, 100);
            if (RND_DorH > 60)
            {
                //‰ñ”ð
                m_BTA.Dodge();
            }
            else
            {
                m_Hp -= Damage;
                if (m_Hp <= 0)
                {
                    //Ž€–S
                    m_BTA.Dead();
                    //Ž€–S‚µ‚½Ž–‚ðUŒ‚ŽÒ‚É’Ê’m
                    Flag = true;
                }
                else
                {
                    //”í’e
                    m_BTA.Hit();
                }
            }
        }
        return Flag;
    }
}
