using UnityEngine;
using Mirror;
using Unity.Cinemachine;

public class PlayerCameraFollow : NetworkBehaviour
{
    [Header("カメラが追従・注視するTransform")]
    public Transform cameraFollowTarget; // PlayerCube内のCameraPointなどを設定

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        var cam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (cam != null && cameraFollowTarget != null)
        {
            cam.Follow = cameraFollowTarget;
            cam.LookAt = cameraFollowTarget;
            // これで Tracking Target にも反映される
        }
    }
}
