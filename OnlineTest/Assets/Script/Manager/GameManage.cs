using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region ▼【UI・オブジェクト参照】
    public GameObject m_game;         // ゲームプレイ中のUIや要素を格納する親オブジェクト
    public GameObject m_Clear;        // ゲームクリア画面のUI
    public GameObject m_Over;         // ゲームオーバー画面のUI
    public FadeInOut inOut;           // タイトル⇔ゲーム間のフェード演出用
    public PlayerMove m_player;           // プレイヤースクリプトの参照
    public List<EnemyManager> m_enemyManagers = new List<EnemyManager>(); // 敵出現を管理するEnemyManagerのリスト
    public AudioSource m_bgmSource;   // ゲーム用BGM音源
    public TextMeshProUGUI m_quitApp; //ゲーム終了ナビ
    public Slider m_hpBar;
    #endregion

    #region ▼【タイマー関連】
    public TextMeshProUGUI m_timerText; // タイマー表示用テキスト（TMP）
    public float m_timeLimit = 180f;    // 制限時間（秒）
    private float m_timer = 0f;         // 現在の経過時間
    #endregion

    #region ▼【状態フラグ】
    public bool m_gameFlag;   // ゲーム中かどうか
    public bool m_resetFlag;  // リセットされた直後かどうか
    public bool m_Clearflag;  // ゲームクリア状態か
    public bool m_Overflag;   // ゲームオーバー状態か
    #endregion

    #region ▶【初期化処理：Awake】
    void Awake()
    {
        m_game.SetActive(false);
        m_Clear.SetActive(false);
        m_Over.SetActive(false);

        m_resetFlag = false;
        m_gameFlag = false;
        m_Clearflag = false;
        m_Overflag = false;
        m_timerText.enabled = false;
        m_quitApp.enabled = false;
        m_hpBar.gameObject.SetActive(false);

        Title(); // タイトル画面処理へ移行（プレイヤーダミー生成）
    }
    #endregion

    #region ▶【フレーム更新処理：Update】
    void Update()
    {
        // 制限時間タイマー更新
        if (m_gameFlag && !m_Clearflag && !m_Overflag)
            UpdateGameTimer();

        // ゲーム終了処理（クリア or オーバー）
        if (m_Clearflag) GameClear();
        if (m_Overflag) GameOver();

        // タイトル画面でのAボタン入力待機
        if (!inOut.m_flag)
            ButtonCheker();
    }
    #endregion

    #region ▶【タイトル画面：Aボタンで開始】
    public void ButtonCheker()
    {
        if (Input.GetKey(KeyCode.A))
        {
            inOut.m_flag = true; // フェードアウト開始
            SetUP();             // ゲームセットアップへ
        }
    }
    #endregion

    #region ▶【ゲームクリア処理】
    public void GameClear()
    {
        m_gameFlag = false;
        m_timerText.enabled = false;
        m_hpBar.gameObject.SetActive(false);
        m_Clear.SetActive(true); // クリア画面表示

        // Xキー/ボタンでタイトルへ戻る
        if (Input.GetKey(KeyCode.T))
        {
            m_game.SetActive(false);
            m_resetFlag = true;
            ReTitle();
        }
    }
    #endregion

    #region ▶【ゲームオーバー処理】
    public void GameOver()
    {
        m_gameFlag = false;
        m_timerText.enabled = false;
        m_hpBar.gameObject.SetActive(false);
        m_Over.SetActive(true); // オーバー画面表示

        // Xキーでタイトルへ戻る
        if (Input.GetKey(KeyCode.T))
        {
            m_game.SetActive(false);
            m_resetFlag = true;
            ReTitle();
        }

    }
    #endregion

    #region ▶【リスタート処理】
    public void ReStart()
    {
        if (m_bgmSource.isPlaying)
            m_bgmSource.Stop();

        
        m_Clearflag = false;
        m_Overflag = false;

        m_Clear.SetActive(false);
        m_Over.SetActive(false);
        m_player.PosReset();          // プレイヤー初期位置に戻す

        SetUP();                      // ゲーム再セットアップ
    }
    #endregion

    #region ▶【タイトル画面へ戻る処理】
    public void ReTitle()
    {
        if (m_bgmSource.isPlaying)
            m_bgmSource.Stop();
        m_Clearflag = false;
        m_Overflag = false;

        m_Clear.SetActive(false);
        m_Over.SetActive(false);
        m_player.PosReset();

        m_timerText.enabled = false; // ← タイトル戻り時にタイマー非表示
        m_quitApp.enabled = false;
        m_hpBar.gameObject.SetActive(false);
        inOut.m_flag = false; // フェードリセット
    }

    public void Title()
    {
    }
    #endregion

    #region ▶【ゲームセットアップ処理（初期化）】
    public void SetUP()
    {
        m_resetFlag = false;
        m_game.SetActive(true);
        m_timer = 0f; // タイマーリセット

        foreach (EnemyManager enemyManager in m_enemyManagers)
        {
            enemyManager.ClearLists();       // 残機リスト初期化
            enemyManager.InitializeGame();   // 敵生成の初期設定
        }

        GameStart(); // ゲーム開始へ移行
    }
    #endregion

    #region ▶【ゲーム開始時の処理】
    public void GameStart()
    {
        m_gameFlag = true;
        m_timerText.enabled = true;
        m_quitApp.enabled = true;
        m_hpBar.gameObject.SetActive(true);

        if (!m_bgmSource.isPlaying)
            m_bgmSource.Play();
    }
    #endregion

    #region ▶【制限時間タイマー処理】
    void UpdateGameTimer()
    {
        m_timer += Time.deltaTime;

        float remaining = Mathf.Max(m_timeLimit - m_timer, 0f);
        m_timerText.text = $"{Mathf.FloorToInt(remaining)}";

        // 10秒以下で赤く表示
        m_timerText.color = (remaining <= 20.0f) ? Color.red : Color.white;

        if (remaining <= 0f && !m_Overflag)
        {
            m_Overflag = true;
            Debug.Log("Time's up! Game Over.");
        }
    }
    #endregion
}
