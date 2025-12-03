using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Mirrorを使ったネットワーク対応のプレイヤー操作スクリプト
/// ジャンプ・移動・回避・攻撃（通常/チャージ/特殊）に対応
/// </summary>
public class PlayerMove : NetworkBehaviour
{
    // ===============================
    // === インスペクター設定項目 ===
    // ===============================

    [Header("移動設定")]
    public float moveSpeed = 5f;      // 通常移動速度
    public float dashSpeed = 10f;     // ダッシュ速度
    public float jumpForce = 5f;      // ジャンプ時の上方向の力

    [Header("チャージ攻撃設定")]
    public float chargeTimeThreshold = 0.4f; // チャージ攻撃判定時間（これ以上でチャージ攻撃）

    [Header("ジャンプ設定")]
    public int maxJumpCount = 2;      // ジャンプ可能回数（二段ジャンプなど）

    [Header("地面判定")]
    public float groundCheckDistance = 0.1f; // 地面との距離チェック長さ
    public LayerMask groundLayer;            // 地面判定に使うレイヤー
    
  
    public AttackAnimationSystem m_animationSystem;
    // ===============================
    // === プライベート変数群（m_）===
    // ===============================

   
    private Rigidbody m_Rigidbody;               // プレイヤーのRigidbody
    private PlayerControl m_Controls;            // InputSystemで生成される操作クラス
    private Vector2 m_MoveInput;                 // 入力方向（X,Z）

    private int m_CurrentJumpCount;              // 残ジャンプ回数
    private bool m_IsGrounded = true;            // 地面に接触しているか

    
    private bool m_IsCharging = false;           // チャージ攻撃中かどうか
    private float m_ChargeStartTime;             // チャージ開始時刻
    private bool m_IsAttack = false;

    // ===============================
    // === Unity ライフサイクル ===
    // ===============================

    void Start()
    {
        //Init(); // 初期化処理
        Debug.Log("Start(): 初期化完了");

    }

    /// <summary>
    /// プレイヤーの初期設定（Rigidbody、Input、ジャンプ回数など）
    /// </summary>
    void Init()
    {
        m_CurrentJumpCount = maxJumpCount;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Controls = new PlayerControl();
      
        SetupInput(); // 入力イベントの登録
    }

    /// <summary>
    /// クライアントが操作権限を持った時に呼ばれる（入力有効化）
    /// </summary>
    public override void OnStartAuthority()
    {
        Init();
        m_Controls.GamePlay.Enable();
    }

    /// <summary>
    /// 無効化時に入力を解除
    /// </summary>
    void OnDisable()
    {
        m_Controls?.GamePlay.Disable();
    }

    /// <summary>
    /// 固定更新：移動・地面チェック・回避時間の更新を行う
    /// </summary>
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        HandleGroundCheck();
        HandleMovement();
        
    }

    /// <summary>
    /// 地面と衝突したときに接地状態にする
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            m_IsGrounded = true;
        m_animationSystem.m_Animator.SetBool("JumpPush", false);
        m_animationSystem.m_Animator.SetBool("DoubleJumpPush", false);
    }

    // ===============================
    // === 入力設定関連 ===
    // ===============================

    /// <summary>
    /// 新InputSystemでの各入力イベントを登録
    /// </summary>
    void SetupInput()
    {
        // 移動入力
        m_Controls.GamePlay.Move.performed += ctx => m_MoveInput = ctx.ReadValue<Vector2>();
        m_Controls.GamePlay.Move.canceled += _ => m_MoveInput = Vector2.zero;

        // ジャンプ入力
        m_Controls.GamePlay.Jump.performed += _ =>
        {
            if (isLocalPlayer)
                TryJump();
        };

        // ステップ（Evade は performed のみでOK）
        m_Controls.GamePlay.Evade.performed += _ =>
        {
            if (!isLocalPlayer) return;
            Step();
        };

        m_Controls.GamePlay.Sprint.started += _ =>
        {
            if (!isLocalPlayer) return;
            Dash();
        };

        m_Controls.GamePlay.Sprint.canceled += _ =>
        {
            if (!isLocalPlayer) return;
            m_animationSystem.m_Animator.SetBool("Sprint", false);
        };

        // 通常/チャージ攻撃
        m_Controls.GamePlay.Attack.started += _ =>
        {
            if (!isLocalPlayer) return;
            m_IsCharging = true;
            m_ChargeStartTime = Time.time;
            StartChargeEffect();
        };

        m_Controls.GamePlay.Attack.canceled += _ =>
        {
            if (!isLocalPlayer || !m_IsCharging) return;

            float held = Time.time - m_ChargeStartTime;
            if (held >= chargeTimeThreshold)
                CmdChargeAttack();
            else
                CmdNormalAttack();

            m_IsCharging = false;
            EndChargeEffect();
        };

        // 特殊攻撃（ボタン1回で発動）
        m_Controls.GamePlay.SpecialAttack.performed += _ =>
        {
            if (isLocalPlayer)
                CmdSpecialAttack();
        };
    }

    // ===============================
    // === プレイヤー動作処理 ===
    // ===============================

    /// <summary>
    /// ジャンプ処理（残ジャンプ数がある場合のみ）
    /// </summary>
    void TryJump()
    {
        if (m_CurrentJumpCount <= 0) return;
        m_animationSystem.m_Animator.SetBool("JumpPush",true);
        if(!m_IsGrounded)
        {
            m_animationSystem.m_Animator.SetBool("DoubleJumpPush", true);
        }
        m_Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        m_CurrentJumpCount--;
    }

    /// <summary>
    /// プレイヤーの移動（回避中は速度が変わる）
    /// </summary>
    void HandleMovement()
    {
        Vector3 direction = new Vector3(m_MoveInput.x, 0f, m_MoveInput.y);
        float moveSpeedValue = m_animationSystem.m_Animator.GetBool("Sprint") ? dashSpeed : moveSpeed;


        // アニメーション用のパラメーター送信
        float animSpeed = m_MoveInput.magnitude;
        m_animationSystem.m_Animator.SetFloat("Speed", animSpeed);
        m_animationSystem.m_Animator.SetFloat("MoveX", m_MoveInput.x); // ← 追加
        m_animationSystem.m_Animator.SetFloat("MoveY", m_MoveInput.y); // ← 追加

       
        m_Rigidbody.MovePosition(transform.position + direction * moveSpeedValue * Time.fixedDeltaTime);
    }

    /// <summary>
    /// 地面判定用Raycast。着地時はジャンプ回数リセット。
    /// </summary>
    void HandleGroundCheck()
    {
        bool wasGrounded = m_IsGrounded;
        m_IsGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundLayer);

        if (!wasGrounded && m_IsGrounded)
            m_CurrentJumpCount = maxJumpCount;
    }

  
    // ===============================
    // === サーバー側の攻撃処理 ===
    // ===============================

    [Command]
    void CmdNormalAttack() => DoNormalAttack();

    [Command]
    void CmdChargeAttack() => DoChargeAttack();

    [Command]
    void CmdSpecialAttack() => DoSpecialAttack();

    void DoNormalAttack()
    {
        Debug.Log("通常攻撃をサーバーで実行");
        m_animationSystem.m_Animator.SetBool("AttackFlag",true);
        m_animationSystem.AttackTrigger();

        // 弾生成など
    }

    void DoChargeAttack()
    {
        Debug.Log("チャージ攻撃をサーバーで実行");

        // チャージ弾処理
    }

    void DoSpecialAttack()
    {
        Debug.Log("特殊攻撃をサーバーで実行");
        m_animationSystem.m_Animator.SetTrigger("Skill");
        // スキル処理
    }

    // ===============================
    // === ローカルのエフェクト演出 ===
    // ===============================

    void Step() => Debug.Log("ローカル：ステップ");
    void Dash()
    {
        Debug.Log("ローカル：ダッシュ");
        m_animationSystem.m_Animator.SetBool("Sprint", true);
    }  
    void StartChargeEffect() => Debug.Log("ローカル：チャージ開始");
    void EndChargeEffect() => Debug.Log("ローカル：チャージ終了");
}
