using UnityEngine;
using Unity.Cinemachine;
using Mirror;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーに追従する Cinemachine カメラ制御
/// ・プレイヤー後方の CameraPoint に追従・注視
/// ・マウス操作でカメラを水平方向に回転
/// ・プレイヤーが生成された後に CameraPoint を検索して設定する
/// </summary>
public class PlayerCameraFollow : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera cineCam; // シーン上の Cinemachine カメラ
    public float rotationSpeed = 100f;                  // カメラ回転速度（マウス感度）

    private PlayerControl m_Controls;                   // 新InputSystemの操作マップ
    private bool isCameraSet = false;                   // カメラが一度設定されたかどうか

    // Start：ローカルプレイヤーのみ Input の初期化
    void Start()
    {
        if (isLocalPlayer)
        {
            m_Controls = new PlayerControl();
            m_Controls.GamePlay.Enable(); // Look入力を有効化
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return; // 自分のキャラ以外は無視

        // カメラが未設定なら CameraPoint を探して設定（毎フレーム1回チェック）
        if (!isCameraSet && cineCam != null)
        {
            GameObject cameraPoint = GameObject.FindGameObjectWithTag("CameraPoint");
            if (cameraPoint != null)
            {
                cineCam.Follow = cameraPoint.transform;
                cineCam.LookAt = cameraPoint.transform;
                isCameraSet = true;
                Debug.Log("CameraPoint をカメラに設定しました");
            }
        }

        // カメラ設定済みなら、マウスのX入力でカメラを回転
        if (isCameraSet && m_Controls != null)
        {
            Vector2 look = m_Controls.GamePlay.Look.ReadValue<Vector2>();

            if (Mathf.Abs(look.x) > 0.01f)
            {
                // カメラのTransformを Y軸まわりに回転
                cineCam.transform.Rotate(Vector3.up, look.x * rotationSpeed * Time.deltaTime);
            }
        }
    }
}
