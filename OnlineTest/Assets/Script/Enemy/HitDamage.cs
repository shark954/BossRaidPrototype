using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDamage : MonoBehaviour
{
    public string m_team;
    public int m_damage = 2;
    public GameObject m_effct;
    public float m_destroyTime = 2.0f;
    public float m_hitdesTime = 1.0f;
    public float m_firepower = 1000.0f;
    public AudioSource m_audioSource;

    // Start is called before the first frame update
    void Start()
    {
       
        GetComponent<Rigidbody>().AddForce(transform.forward * m_firepower);
        m_audioSource = GetComponent<AudioSource>();
        //音の再生タイミングよくわからん
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
       // m_audioSource.Play();
        if (other.GetComponent<Parameta>())
        {

            if (other.GetComponent<Parameta>().HitDamage(m_damage, m_team))
            {
                if (m_audioSource != null)
                {
                    m_audioSource.Play();
                }

                if (m_effct != null)
                {

                    GameObject Dummy = Instantiate(m_effct, transform.position, transform.rotation);
                    Destroy(Dummy, 2.0f);

                }
                Destroy(gameObject, m_hitdesTime);
            }
        }

        if (m_team == "Player")
        {
            if (other.CompareTag("EnemyAttack"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
