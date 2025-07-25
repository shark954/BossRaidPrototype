using UnityEngine;
using TMPro;
using Unity.UI;

public class CounterSystem : MonoBehaviour
{
    [Header("受付")]
    public Canvas m_counter;
    [Header("受付の名前")]   //ex.オンライン受付
    public TextMeshProUGUI m_counterName;
    [Header("受付名の表示する範囲")]
    public SphereCollider m_realizeCounterName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_counterName.enabled = false;
        m_counter.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_counterName.enabled = true;
            m_counter.gameObject.SetActive(true);
        }
    }
}
