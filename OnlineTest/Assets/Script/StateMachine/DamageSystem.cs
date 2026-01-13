using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public BTA_Parameta m_Parameta;
    public int DMG = 1;

    public void Update()
    {
        if (m_Parameta.m_ArmsOnFlag)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Parameta>())
        {
            BTA_Parameta P = other.GetComponent<BTA_Parameta>();
            if (P != m_Parameta)
            {
                //ダメージを与える
                if (P.TakeDamage(DMG))
                {
                    //死亡している場合は、ターゲットから除外
                    m_Parameta.m_BTA.m_Player = null;
                }
            }
        }
    }
}
