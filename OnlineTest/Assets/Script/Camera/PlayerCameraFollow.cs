using UnityEngine;
using Unity.Cinemachine;
using Mirror;
using UnityEngine.InputSystem;

/// <summary>
/// ◆ プレイヤーカメラ制御（簡易 TPS）
///
/// ・CinemachineCamera をプレイヤーの後方位置に固定
/// ・マウスの左右移動でカメラを水平回転
/// ・ローカルプレイヤーの時だけカメラ制御を有効化（Mirror）
/// </summary>
public class PlayerCameraFollow : NetworkBehaviour
{
    // ─────────────────────────────────────
    // インスペクター設定項目
    // ─────────────────────────────────────

    [SerializeField] private CinemachineCamera cineCam;
    // 追従させる Transform（プレイヤーの頭・背後の空オブジェクトなど）
    [SerializeField] private Transform cameraFollowTarget;

    // マウス感度（回転速度）
    public float rotationSpeed = 100f;

    // 新 Input System で自動生成される操作クラス
    private PlayerControl m_Controls;


    // ─────────────────────────────────────
    // ローカルプレイヤーが権限を得たときに実行される（Mirror の仕様）
    // OnStartAuthority() は override 必須！
    // ─────────────────────────────────────
    public override void OnStartAuthority()
    {
        base.OnStartAuthority(); // 親クラスの処理も実行

        // InputSystem 有効化
        m_Controls = new PlayerControl();
        m_Controls.GamePlay.Enable();

        // CinemachineCamera が設定されていない場合はエラー
        if (cineCam == null)
        {
            Debug.LogError("CinemachineCamera がセットされていません");
            return;
        }

        // カメラが追従するターゲットを設定（Transform を直接渡す）
        if (cameraFollowTarget != null)
        {
            // ※ Cinemachine 6.x の Target は CameraTarget 型 → Transform を直渡し可能
            cineCam.Target.TrackingTarget = cameraFollowTarget;
        }
    }


    // ─────────────────────────────────────
    // 毎フレーム実行
    // ※カメラ操作はローカルプレイヤーだけが行う
    // ─────────────────────────────────────
    void Update()
    {
        // 他人のキャラではカメラを動かさない
        if (!isLocalPlayer || m_Controls == null) return;

        // マウス移動量（Vector2）取得
        Vector2 look = m_Controls.GamePlay.Look.ReadValue<Vector2>();

        // 水平方向の移動が一定以上なら回転開始
        if (Mathf.Abs(look.x) > 0.01f)
        {
            // ◆ カメラ本体を回転させる（シンプル方式）
            // Y軸まわりの回転 → TPS の「カメラを左右に回す」動きになる
            cineCam.transform.Rotate(
                Vector3.up,
                look.x * rotationSpeed * Time.deltaTime
            );
        }
    }
}
