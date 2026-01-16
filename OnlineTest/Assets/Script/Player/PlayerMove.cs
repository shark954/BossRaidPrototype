using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// シングルプレイヤー用のプレイヤー移動・操作スクリプト
/// ジャンプ、ダッシュ、回避、通常攻撃、チャージ攻撃、特殊攻撃をサポート
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // === インスペクター設定項目 ===
    [Header("移動設定")]
    public float moveSpeed = 5f;       // 通常移動速度
    public float dashSpeed = 10f;      // ダッシュ時の移動速度
    public float jumpForce = 5f;       // ジャンプ力
    public float evadeForce = 10f;     // 回避力（未使用）

    [Header("チャージ攻撃設定")]
    public float chargeTimeThreshold = 0.4f; // チャージ攻撃になるまでのホールド時間
    [Header("スキル使用条件")]
    public int m_killCount = 0;
    public int m_requiredKillsForSkill = 3;
    public bool m_canUseSpecialAttack = false;
    [Header("スキルUI")]
    public Slider m_skillGaugeSlider;
    public TextMeshProUGUI m_skillReadyText;

    [Header("ジャンプ設定")]
    public int maxJumpCount = 2;       // 最大ジャンプ回数（二段ジャンプ対応）

    [Header("地面判定")]
    public float groundCheckDistance = 0.1f; // 地面との距離で接地を判断
    public LayerMask groundLayer;            // 地面判定対象のレイヤー

    [SerializeField]
    private Transform m_startPos; // リスポーン位置

    public AttackAnimationSystem m_animationSystem; // アニメーション制御クラス参照

    // === プライベート変数 ===
    private Rigidbody m_Rigidbody;            // Rigidbody参照
    private PlayerControl m_Controls;         // 新InputSystemの入力スクリプト
    private Vector2 m_MoveInput;              // 移動入力（横・縦）

    private int m_CurrentJumpCount;           // 現在のジャンプ残数
    private bool m_IsGrounded = true;         // 接地状態
    private bool m_CanEvade = true;           // 回避可能状態（未使用）
    private bool m_IsCharging = false;        // チャージ攻撃中か
    private float m_ChargeStartTime;          // チャージ開始時間

    /// <summary>
    /// 初期化処理（Start時）
    /// </summary>
    void Start()
    {
        Init(); // 必要な変数や入力初期化
        m_Controls.GamePlay.Enable(); // 入力を有効化
        Cursor.lockState = CursorLockMode.Locked; // カーソルを非表示・固定
        Cursor.visible = false;
    }

    /// <summary>
    /// 初期化：入力・ジャンプカウント・Rigidbody取得など
    /// </summary>
    void Init()
    {
        m_CurrentJumpCount = maxJumpCount;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Controls = new PlayerControl();
        SetupInput(); // 入力イベント登録
    }

    /// <summary>
    /// 無効化時に入力を無効化
    /// </summary>
    void OnDisable()
    {
        m_Controls?.GamePlay.Disable();
    }

    /// <summary>
    /// 毎フレーム固定間隔で呼ばれる処理（物理系）
    /// </summary>
    void FixedUpdate()
    {
        HandleGroundCheck(); // 接地チェック
        HandleMovement();    // 移動処理
    }

    /// <summary>
    /// 地面に着地したらジャンプ関連のアニメーションをリセット
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            m_IsGrounded = true;

        m_animationSystem.m_Animator.SetBool("JumpPush", false);
        m_animationSystem.m_Animator.SetBool("DoubleJumpPush", false);
    }

    // リスポーン（初期位置に戻る）
    public void PosReset()
    {
        transform.position = m_startPos.position;
        transform.rotation = m_startPos.rotation;
    }

    /// <summary>
    /// 入力イベントを設定
    /// </summary>
    void SetupInput()
    {
        // 移動
        m_Controls.GamePlay.Move.performed += ctx => m_MoveInput = ctx.ReadValue<Vector2>();
        m_Controls.GamePlay.Move.canceled += _ => m_MoveInput = Vector2.zero;

        // ジャンプ
        m_Controls.GamePlay.Jump.performed += _ => TryJump();

        // 回避
        m_Controls.GamePlay.Evade.performed += _ => Step();

        // ダッシュ開始・終了
        m_Controls.GamePlay.Sprint.started += _ => Dash();
        m_Controls.GamePlay.Sprint.canceled += _ => m_animationSystem.m_Animator.SetBool("Sprint", false);

        // 通常/チャージ攻撃
        m_Controls.GamePlay.Attack.started += _ =>
        {
            m_IsCharging = true;
            m_ChargeStartTime = Time.time;
            StartChargeEffect();
        };
        m_Controls.GamePlay.Attack.canceled += _ =>
        {
            if (!m_IsCharging) return;

            float held = Time.time - m_ChargeStartTime;
            if (held >= chargeTimeThreshold)
                DoChargeAttack();
            else
                DoNormalAttack();

            m_IsCharging = false;
            EndChargeEffect();
        };

        // 特殊攻撃
        m_Controls.GamePlay.SpecialAttack.performed += _ => DoSpecialAttack();
        m_Controls.GamePlay.SpecialAttack.canceled += _ =>
        {
            m_animationSystem.m_Animator.SetBool("Skill", false);
        };
    }

    /// <summary>
    /// ジャンプ処理（ジャンプ回数消費＋アニメ）
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
    /// カメラの向きに合わせた移動処理
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
    /// 地面との接触をRayで判定。着地時はジャンプ回数をリセット。
    /// </summary>
    void HandleGroundCheck()
    {
        bool wasGrounded = m_IsGrounded;
        m_IsGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundLayer);

        if (!wasGrounded && m_IsGrounded)
            m_CurrentJumpCount = maxJumpCount;
    }

    /// <summary>
    /// 通常攻撃の処理
    /// </summary>
    void DoNormalAttack()
    {
        Debug.Log("通常攻撃を実行");
        // 弾生成や近接処理などを追加
    }

    /// <summary>
    /// チャージ攻撃の処理
    /// </summary>
    void DoChargeAttack()
    {
        Debug.Log("チャージ攻撃を実行");
        // チャージエフェクトや強攻撃など
    }

    void UpdateSkillUI()
    {
        // ゲージの更新
        if (m_skillGaugeSlider != null)
        {
            float ratio = (float)m_killCount / m_requiredKillsForSkill;
            m_skillGaugeSlider.value = Mathf.Clamp01(ratio);
        }

        // Ready テキスト表示制御
        if (m_skillReadyText != null)
        {
            m_skillReadyText.gameObject.SetActive(m_canUseSpecialAttack);
        }
    }

    public void OnEnemyKilled()
    {
        m_killCount++;

        if (m_killCount >= m_requiredKillsForSkill)
        {
            m_canUseSpecialAttack = true;
            Debug.Log("特殊攻撃が使用可能になりました！");
            // UI点灯などの演出があればここ
        }

        UpdateSkillUI();
    }


    /// <summary>
    /// 特殊攻撃の処理
    /// </summary>
    void DoSpecialAttack()
    {
        if (!m_canUseSpecialAttack)
        {
            Debug.Log("まだ特殊攻撃は使えません！");
            return;
        }

        Debug.Log("特殊攻撃を実行");
        m_animationSystem.m_Animator.SetBool("Skill", true);

        m_canUseSpecialAttack = false;
        m_killCount = 0;
        UpdateSkillUI();
    }


    /// <summary>
    /// 回避（ステップ）処理
    /// </summary>
    void Step()
    {
        m_CanEvade = false;
        m_animationSystem.m_Animator.SetTrigger("IsEvading");
    }

    /// <summary>
    /// ダッシュアニメーションのトリガー
    /// </summary>
    void Dash()
    {
        Debug.Log("ダッシュ");
        m_animationSystem.m_Animator.SetBool("Sprint", true);
    }

    /// <summary>
    /// チャージ開始演出
    /// </summary>
    void StartChargeEffect() => Debug.Log("チャージ開始");

    /// <summary>
    /// チャージ終了演出
    /// </summary>
    void EndChargeEffect() => Debug.Log("チャージ終了");
}
