using UnityEngine;
using Mirror;
using Unity.Cinemachine;

public class PlayerCameraFollow : NetworkBehaviour
{
    [Header("ƒJƒƒ‰‚ª’Ç]E’‹‚·‚éTransform")]
    public Transform cameraFollowTarget; // PlayerCube“à‚ÌCameraPoint‚È‚Ç‚ğİ’è

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        var cam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (cam != null && cameraFollowTarget != null)
        {
            cam.Follow = cameraFollowTarget;
            cam.LookAt = cameraFollowTarget;
            // ‚±‚ê‚Å Tracking Target ‚É‚à”½‰f‚³‚ê‚é
        }
    }
}
