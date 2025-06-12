using UnityEngine;
using Mirror;
using Unity.Cinemachine;

public class PlayerCameraFollow : NetworkBehaviour
{
    [Header("�J�������Ǐ]�E��������Transform")]
    public Transform cameraFollowTarget; // PlayerCube����CameraPoint�Ȃǂ�ݒ�

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        var cam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (cam != null && cameraFollowTarget != null)
        {
            cam.Follow = cameraFollowTarget;
            cam.LookAt = cameraFollowTarget;
            // ����� Tracking Target �ɂ����f�����
        }
    }
}
