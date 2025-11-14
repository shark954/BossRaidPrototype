using Mirror;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    [Header("移動関連設定")]
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float jumpForce = 5f;

    [Header("チャージ攻撃設定")]
    public float chargeTimeThreshold = 0.4f;

    private Rigidbody rb;
    private PlayerControl controls;
    private Vector2 moveInput;
    private bool isGrounded = true;

    private bool isEvadeHeld = false;
    private float evadeTimer = 0f;
    private float evadeHoldThreshold = 0.3f;

    [Header("地面判定用設定")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    [Header("ジャンプ設定")]
    public int maxJumpCount = 2;
    private int currentJumpCount;

    private bool isCharging = false;
    float chargeStartTime;

    [System.Obsolete]
    void Awake()
    {
        currentJumpCount = maxJumpCount;
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControl();

        SetupInput();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public override void OnStartAuthority()
    {
        controls.GamePlay.Enable();
    }

    void OnDisable()
    {
        if (controls != null)
            controls.GamePlay.Disable();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        HandleGroundCheck();
        HandleMovement();
        UpdateEvadeTimer();
    }

    // ===============================
    // === 入力設定 ===
    // ===============================
    private void SetupInput()
    {
        controls.GamePlay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.GamePlay.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.GamePlay.Jump.performed += ctx =>
        {
            if (!isLocalPlayer) return;
            TryJump();
        };

        controls.GamePlay.Evade.started += ctx =>
        {
            if (!isLocalPlayer) return;
            isEvadeHeld = true;
            evadeTimer = 0f;
        };

        controls.GamePlay.Evade.canceled += ctx =>
        {
            if (!isLocalPlayer) return;
            if (evadeTimer >= evadeHoldThreshold)
                Dash();
            else
                Step();
            isEvadeHeld = false;
        };

        controls.GamePlay.Attack.started += ctx =>
        {
            if (!isLocalPlayer) return;
            isCharging = true;
            chargeStartTime = Time.time;
            StartChargeEffect();
        };

        controls.GamePlay.Attack.canceled += ctx =>
        {
            if (!isLocalPlayer || !isCharging) return;

            float holdTime = Time.time - chargeStartTime;

            if (holdTime >= chargeTimeThreshold)
                CmdChargeAttack();
            else
                CmdNormalAttack();

            isCharging = false;
            EndChargeEffect();
        };

        controls.GamePlay.SpecialAttack.performed += ctx =>
        {
            if (isLocalPlayer)
                CmdSpecialAttack();
        };
    }

    // ===============================
    // === 個別の機能関数 ===
    // ===============================
    private void TryJump()
    {
        if (currentJumpCount > 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            currentJumpCount--;
        }
    }

    private void PerformNormalAttack()
    {
        // 通常攻撃のロジック（例：弾の生成）
        Debug.Log("→ 通常攻撃の実行処理");
        // 例：SpawnBullet(normalBulletPrefab);
    }

    private void PerformChargeAttack()
    {
        // チャージ攻撃のロジック（例：強化弾）
        Debug.Log("→ チャージ攻撃の実行処理");
        // 例：SpawnBullet(chargeBulletPrefab);
    }

    private void PerformSpecialAttack()
    {
        // 特殊攻撃のロジック（例：広範囲攻撃やスキル）
        Debug.Log("→ 特殊攻撃の実行処理");
        // 例：ExecuteAreaAttack();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        float speed = isEvadeHeld && evadeTimer >= evadeHoldThreshold ? dashSpeed : moveSpeed;
        rb.MovePosition(transform.position + move * speed * Time.fixedDeltaTime);
    }

    private void HandleGroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            currentJumpCount = maxJumpCount;
        }
    }

    private void UpdateEvadeTimer()
    {
        if (isEvadeHeld)
            evadeTimer += Time.deltaTime;
    }

    // ===============================
    // === サーバー処理 ===
    // ===============================
    [Command]
    void CmdNormalAttack()
    {
        Debug.Log("通常攻撃（サーバーで処理）");
        PerformNormalAttack();
    }

    [Command]
    void CmdChargeAttack()
    {
        Debug.Log("チャージ攻撃（サーバーで処理）");
        PerformChargeAttack();
    }

    [Command]
    void CmdSpecialAttack()
    {
        Debug.Log("特殊攻撃（サーバーで処理）");
        PerformSpecialAttack();
    }
    // ===============================
    // === ローカル演出処理 ===
    // ===============================
    void Step() => Debug.Log("回避ステップ（ローカルのみ）");
    void Dash() => Debug.Log("ダッシュ移動（ローカルのみ）");
    void StartChargeEffect() => Debug.Log("チャージ演出開始（UI/エフェクト）");
    void EndChargeEffect() => Debug.Log("チャージ演出終了（UI/エフェクト）");
}
