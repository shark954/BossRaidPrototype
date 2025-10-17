using Mirror; // Mirror対応用
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    [Header("移動関連設定")]
    public float moveSpeed = 5f;         // 通常時の移動速度
    public float dashSpeed = 10f;        // ダッシュ時の移動速度
    public float jumpForce = 5f;         // ジャンプ時の力

    [Header("チャージ攻撃設定")]
    public float chargeTimeThreshold = 0.4f; // チャージと判定する最小時間

    private Rigidbody rb;
    private PlayerControl controls;      // InputSystemで自動生成される入力クラス
    private Vector2 moveInput;           // 移動入力の値
    private bool isGrounded = true;      // 地上判定（ジャンプ可否）

    // ダッシュとステップ判定用
    private bool isEvadeHeld = false;
    private float evadeTimer = 0f;
    private float evadeHoldThreshold = 0.3f; // これ以上の長押しでダッシュ

    [Header("地面判定用設定")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    [Header("ジャンプ設定")]
    public int maxJumpCount = 2;  // 最大ジャンプ回数（二段ジャンプなら2）
    private int currentJumpCount;

    // チャージ判定
    private bool isCharging = false;
    float chargeStartTime;

    //[SerializeField]
    //private GunTest m_gunTest;
   
    [System.Obsolete]
    void Awake()
    {
        currentJumpCount = maxJumpCount;
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControl(); // 自動生成されたInputActionsのインスタンス化

        // 移動入力
        controls.GamePlay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.GamePlay.Move.canceled += ctx => moveInput = Vector2.zero;

        // ジャンプ（地上のみ）
        controls.GamePlay.Jump.performed += ctx =>
        {
            if (!isLocalPlayer) return;

            if (currentJumpCount > 0)
            {
              
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                currentJumpCount--;
            }
        };

        // Evade（回避 or ダッシュ）：押し始め
        controls.GamePlay.Evade.started += ctx =>
        {
            if (!isLocalPlayer) return;
            isEvadeHeld = true;
            evadeTimer = 0f;
        };

        // Evade：押し終わり時に判定
        controls.GamePlay.Evade.canceled += ctx =>
        {
            if (!isLocalPlayer) return;

            if (evadeTimer >= evadeHoldThreshold)
                Dash();    // ダッシュ発動
            else
                Step();    // ステップ発動

            isEvadeHeld = false;
        };

        // 攻撃：押し始め（チャージ準備）
        controls.GamePlay.Attack.started += ctx =>
        {
            if (!isLocalPlayer) return;
            isCharging = true;
            chargeStartTime = Time.time; // 押した時間を記録
            StartChargeEffect();
        };

        // 攻撃：ボタンを離した時
        controls.GamePlay.Attack.canceled += ctx =>
        {
            if (!isLocalPlayer || !isCharging) return;

            float holdTime = Time.time - chargeStartTime;

            if (holdTime >= chargeTimeThreshold)
                CmdChargeAttack();   // チャージ攻撃
            else
                CmdNormalAttack();   // 通常攻撃

            isCharging = false;
            EndChargeEffect();
        };

        // 特殊攻撃（ワンボタン）
        controls.GamePlay.SpecialAttack.performed += ctx =>
        {
            if (isLocalPlayer)
                CmdSpecialAttack();
        };
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // プレイヤーが自分自身のときだけ入力を有効化
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

        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            // 着地時にジャンプ回数をリセット
            currentJumpCount = maxJumpCount;
        }

        // 移動処理
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        float speed = isEvadeHeld && evadeTimer >= evadeHoldThreshold ? dashSpeed : moveSpeed;
        rb.MovePosition(transform.position + move * speed * Time.fixedDeltaTime);

        if (isEvadeHeld)
            evadeTimer += Time.deltaTime;
    }

    // ===============================
    // === サーバーで動かす処理 ===
    // ===============================

    [Command]
    void CmdNormalAttack()
    {
        Debug.Log("通常攻撃（サーバーで処理）");
        // 弾生成・攻撃処理などをここに追加
        if (isLocalPlayer)
        {

        }

    }

    [Command]
    void CmdChargeAttack()
    {
        Debug.Log("チャージ攻撃（サーバーで処理）");
        // チャージ弾や強化攻撃の処理
    }

    [Command]
    void CmdSpecialAttack()
    {
        Debug.Log("特殊攻撃（サーバーで処理）");
        // 特殊スキルの発動処理など
    }

    // ===============================
    // === ローカルでしか動かさない演出系 ===
    // ===============================

    void Step() => Debug.Log("回避ステップ（ローカルのみ）");
    void Dash() => Debug.Log("ダッシュ移動（ローカルのみ）");
    void StartChargeEffect() => Debug.Log("チャージ演出開始（UI/エフェクト）");
    void EndChargeEffect() => Debug.Log("チャージ演出終了（UI/エフェクト）");
}
