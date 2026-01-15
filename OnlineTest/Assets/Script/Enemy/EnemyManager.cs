using UnityEngine;
using System.Collections.Generic;
using AIStateMachine;


public class EnemyManager : MonoBehaviour
{

    [Header("場所ごとで種類を変える")]
    public GameObject m_enemyPrefab; // 敵のPrefab (インスペクターで設定)
    public int m_enemyCount = 5; // 初期残機数

    [SerializeField]
    private List<GameObject> m_remainingEnemiesList = new List<GameObject>(); // 残機用リスト
    [SerializeField]
    private List<GameObject> m_spawnedEnemiesList = new List<GameObject>(); // 出現済み敵リスト

    public GameManager m_gameManager;

    public GameObject m_PopPos;

    public float m_spawnTime = 0.0f;

    public float m_spawnCool=0.0f;

    [SerializeField,Header("出現ポータルエフェクト")]
    private GameObject m_portalEffect;

    [SerializeField,Header("ポータルエフェクト削除")]
    private float m_portalDel;

    // ゲーム開始時の初期化
    void Start()
    {
        InitializeGame(); // ゲームの初期設定を行う
    }

    // ゲームを初期化 (残機リストの再登録)
    public void InitializeGame()
    {
        ClearLists(); // 既存リストをクリアする
        RegisterEnemies(); // 残機リストに敵を登録する
    }

    private void Update()
    {
        if (!m_gameManager.m_Clearflag && !m_gameManager.m_Overflag)
        {
            SpawnEnemy();
        }

        if (m_gameManager.m_Clearflag || m_gameManager.m_Overflag)
        {
            ClearLists();
        }

        CheckGameEnd();
    }

    // リストをクリアして残っている敵を破棄
    public void ClearLists()
    {
        // 残機リストの null じゃない敵だけ Destroy
        foreach (var enemy in m_remainingEnemiesList)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        // 生成済みリストから Missing な GameObject を削除（null のみじゃダメ）
        m_spawnedEnemiesList.RemoveAll(item => item == null); // ← これ重要！

        foreach (var enemy in m_spawnedEnemiesList)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        m_remainingEnemiesList.Clear();
        m_spawnedEnemiesList.Clear();
    }

    // 敵を残機リストに登録する
    private void RegisterEnemies()
    {
        for (int i = 0; i < m_enemyCount; i++) // 残機数分ループ
        {
              // 敵Prefabから新しい敵オブジェクトを生成
                GameObject enemy = Instantiate(m_enemyPrefab, m_PopPos.transform.position,m_PopPos.transform.rotation);
                // 初期状態では非アクティブ
                enemy.SetActive(false); 

                // 残機リストに追加
                m_remainingEnemiesList.Add(enemy);
        }
    }

    // 敵を出現させる
    public void SpawnEnemy()
    {
        if (m_remainingEnemiesList.Count > 0)
        {
            if (m_spawnCool > m_spawnTime)
            {
                // 残機リストから敵を取得
                GameObject enemy = m_remainingEnemiesList[0];
                m_remainingEnemiesList.RemoveAt(0);

                // ランダムなスポーン位置（中心から半径1〜3の円内）
                Vector2 offset2D = Random.insideUnitCircle * Random.Range(1.0f, 3.0f);
                Vector3 spawnOffset = new Vector3(offset2D.x, 0f, offset2D.y);
                Vector3 spawnPos = m_PopPos.transform.position + spawnOffset;

                // 敵をその位置に移動し、アクティブ化
                enemy.transform.position = spawnPos;
                enemy.transform.rotation = m_PopPos.transform.rotation;
                enemy.SetActive(true);

                // ここでプレイヤーをセット
                enemy.GetComponent<AIstate>().m_target = GameObject.FindWithTag("Player").transform;


                // エフェクトをその位置に生成
                CreatPortal(spawnPos);

                // リストに追加
                m_spawnedEnemiesList.Add(enemy);
                m_spawnCool = 0;
            }
            else
            {
                m_spawnCool += Time.deltaTime;
            }
        }
    }

    // 敵の削除処理 (HPが0になった場合など)
    public void RemoveEnemy(GameObject enemy)
    {
        if (m_spawnedEnemiesList.Contains(enemy))
        {
            m_spawnedEnemiesList.Remove(enemy);
            Destroy(enemy); // Delayを付けたければ Destroy(enemy, 5f);
        }

        // nullが混じってると CheckGameEnd が動かないので、毎回リストをクリーンアップ
        m_spawnedEnemiesList.RemoveAll(item => item == null);

        CheckGameEnd();
    }

    // ゲーム終了条件をチェック
    private void CheckGameEnd()
    {
        // ★ nullになった要素をリストから除外
        m_remainingEnemiesList.RemoveAll(e => e == null);
        m_spawnedEnemiesList.RemoveAll(e => e == null);

        Debug.Log($"[CheckGameEnd] Remaining: {m_remainingEnemiesList.Count}, Spawned: {m_spawnedEnemiesList.Count}");

        if (m_remainingEnemiesList.Count == 0 && m_spawnedEnemiesList.Count == 0)
        {
            Debug.Log("Game Clear!");
            m_gameManager.m_Clearflag = true;
        }
    }

    //出現エフェクト
    public void CreatPortal(Vector3 pos)
    {
        if (m_portalEffect != null)
        {
            GameObject dummy = Instantiate(m_portalEffect, pos, Quaternion.identity);
            dummy.transform.localScale = Vector3.one;

            // Y座標を-1（地下に埋もれすぎる場合は調整）
            Vector3 adjustedPosition = dummy.transform.position;
            adjustedPosition.y -= 1f;
            dummy.transform.position = adjustedPosition;

            Destroy(dummy, m_portalDel);
        }
    }
}
