using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPopManager : MonoBehaviour
{
    //ゲームのセットアップで残機数を設定

    [SerializeField,Header("敵の残機")]//敵を生成したら要素数を減らす
    public List<GameObject> m_e_stac;
                            //↑↓両方の要素数が0になったらゲームクリア
   
    [Header("敵の生成数"),SerializeField]
    public List<GameObject> m_e_creat;

    [SerializeField]
    private int m_enemyPopMax;

    //　今何人の敵を出現させたか（総数）
    private int m_enemyPopCount;

    //　待ち時間計測フィールド
    private float m_elapsedTime;

    //　次に敵が出現するまでの時間
    [SerializeField] float m_appearNextTime;

    // Start is called before the first frame update
    void Start()
    {
        m_enemyPopCount = 0;
        m_elapsedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_enemyPopCount >= m_enemyPopMax)
        {
            return;
        }

        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime > m_appearNextTime)
        {
            m_elapsedTime = 0;

            PopEnemy();
        }
    }

    //ここで敵の生成と残機の管理
    public void PopEnemy()
    {
        //　出現させる敵をランダムに選ぶ
        var randomValue = Random.Range(0, m_e_stac.Count);

        //　敵の向きをランダムに決定
        var randomRotationY = Random.value * 360f;

        GameObject.Instantiate(m_e_stac[randomValue], transform.position, Quaternion.Euler(0f, randomRotationY, 0f));
        m_enemyPopCount++;

        m_elapsedTime = 0;
    }
}
