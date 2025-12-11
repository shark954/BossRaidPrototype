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
    public float jumpForce = 5f;      // ジャンプ力
    public float evadeForce = 10f;    // 回避時の力（未使用）

    [Header("チャージ攻撃設定")]
    public float chargeTimeThreshold = 0.4f; // チャージ攻撃判定時間（0.4秒以上でチャージ攻撃）

    [Header("ジャンプ設定")]
    public int maxJumpCount = 2;      // ジャンプ可能回数（二段ジャンプ対応）

    [Header("地面判定")]
    public float groundCheckDistance = 0.1f; // 地面との距離チェック長さ
    public LayerMask groundLayer;            // 地面と判定するレイヤー

    public AttackAnimationSystem m_animationSystem; // アニメーション制御クラス

    // ===============================
    // === プライベート変数群（m_）===
    // ===============================

    private Rigidbody m_Rigidbody;               // プレイヤーのRigidbody
    private PlayerControl m_Controls;            // 新InputSystemの入力クラス
    private Vector2 m_MoveInput;                 // 移動入力値（X,Z）

    private int m_CurrentJumpCount;              // 残ジャンプ回数
    private bool m_IsGrounded = true;            // 地面に接しているか

    private bool m_CanEvade = true;

    private bool m_IsCharging = false;           // チャージ攻撃中か
    private float m_ChargeStartTime;             // チャージ開始時刻

    // ===============================
    // === Unity ライフサイクル ===
    // ===============================

    void Start()
    {
        // Startでは初期化せず、権限取得時に初期化
    }

    /// <summary>
    /// プレイヤーの初期設定（Rigidbodyや入力クラス、ジャンプ回数）
    /// </summary>
    void Init()
    {
        m_CurrentJumpCount = maxJumpCount;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Controls = new PlayerControl();
        SetupInput(); // 入力イベントを設定
    }

    /// <summary>
    /// クライアントが操作権限を得たときに呼ばれる（自分のプレイヤー時）
    /// </summary>
    public override void OnStartAuthority()
    {
        Init(); // 入力などの初期化
        m_Controls.GamePlay.Enable();
        Cursor.lockState = CursorLockMode.Locked; // カーソル非表示＆ロック
        Cursor.visible = false;
    }

    /// <summary>
    /// 無効化時に入力も無効化
    /// </summary>
    void OnDisable()
    {
        m_Controls?.GamePlay.Disable();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        HandleGroundCheck(); // 地面判定
        HandleMovement();    // 移動処理
    }

    /// <summary>
    /// 地面と接触したときの処理（アニメーションリセット）
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            m_IsGrounded = true;

        // アニメーションリセット
        m_animationSystem.m_Animator.SetBool("JumpPush", false);
        m_animationSystem.m_Animator.SetBool("DoubleJumpPush", false);
    }

    // ===============================
    // === 入力設定関連 ===
    // ===============================

    /// <summary>
    /// 各入力イベントを設定
    /// </summary>
    void SetupInput()
    {
        // 移動
        m_Controls.GamePlay.Move.performed += ctx => m_MoveInput = ctx.ReadValue<Vector2>();
        m_Controls.GamePlay.Move.canceled += _ => m_MoveInput = Vector2.zero;

        // ジャンプ
        m_Controls.GamePlay.Jump.performed += _ =>
        {
            if (isLocalPlayer)
                TryJump();
        };

        // 回避
        m_Controls.GamePlay.Evade.performed += _ =>
        {
            if (!isLocalPlayer) return;
            Step(); // 回避動作
        };

        // ダッシュ
        m_Controls.GamePlay.Sprint.started += _ =>
        {
            if (!isLocalPlayer) return;
            Dash(); // ダッシュ開始
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
                CmdChargeAttack();  // チャージ攻撃
            else
                CmdNormalAttack();  // 通常攻撃

            m_IsCharging = false;
            EndChargeEffect();
        };

        // 特殊攻撃
        m_Controls.GamePlay.SpecialAttack.performed += _ =>
        {
            if (isLocalPlayer)
                CmdSpecialAttack();
        };
        m_Controls.GamePlay.SpecialAttack.canceled += _ =>
        {
            m_animationSystem.m_Animator.SetBool("Skill", false);
        };
    }

    // ===============================
    // === プレイヤー動作処理 ===
    // ===============================

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    void TryJump()
    {
        if (m_CurrentJumpCount <= 0) return;

        m_animationSystem.m_Animator.SetBool("JumpPush", true);
        if (!m_IsGrounded)
            m_animationSystem.m_Animator.SetBool("DoubleJumpPush", true);

        m_Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        m_CurrentJumpCount--;
    }

    /// <summary>
    /// カメラの方向に応じた移動処理
    /// </summary>
    void HandleMovement()
    {
        if (Camera.main == null) return;

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = camForward * m_MoveInput.y + camRight * m_MoveInput.x;

        float moveSpeedValue = m_animationSystem.m_Animator.GetBool("Sprint") ? dashSpeed : moveSpeed;

        m_animationSystem.m_Animator.SetFloat("Speed", m_MoveInput.magnitude);

        if (moveDir != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        m_Rigidbody.MovePosition(transform.position + moveDir.normalized * moveSpeedValue * Time.fixedDeltaTime);
    }

    /// <summary>
    /// 地面との接触チェック、着地時はジャンプ回数をリセット
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

    [Command] void CmdNormalAttack() => DoNormalAttack();
    [Command] void CmdChargeAttack() => DoChargeAttack();
    [Command] void CmdSpecialAttack() => DoSpecialAttack();

    void DoNormalAttack()
    {
        Debug.Log("通常攻撃をサーバーで実行");
        // 弾生成などの処理を書く
    }

    void DoChargeAttack()
    {
        // チャージ攻撃処理を書く
    }

    void DoSpecialAttack()
    {
        Debug.Log("特殊攻撃をサーバーで実行");
        m_animationSystem.m_Animator.SetBool("Skill", true);
        // スキルの発動処理を書く
    }

    // ===============================
    // === ローカルの演出系処理 ===
    // ===============================

    /// <summary>
    /// 回避（ステップ）処理
    /// </summary>
    void Step()
    {
        m_CanEvade = false;

        m_animationSystem.m_Animator.SetTrigger("IsEvading");
    }


    /// <summary>
    /// ダッシュ処理（アニメーション変更のみ）
    /// </summary>
    void Dash()
    {
        Debug.Log("ローカル：ダッシュ");
        m_animationSystem.m_Animator.SetBool("Sprint", true);
    }

    /// <summary>
    /// チャージ演出開始
    /// </summary>
    void StartChargeEffect() => Debug.Log("ローカル：チャージ開始");

    /// <summary>
    /// チャージ演出終了
    /// </summary>
    void EndChargeEffect() => Debug.Log("ローカル：チャージ終了");
}
