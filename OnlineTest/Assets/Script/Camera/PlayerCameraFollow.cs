using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraOrbit : MonoBehaviour
{
    public Transform target;                 // CameraPointi’‹“_j
    public float distance = 5.0f;            // ƒ^[ƒQƒbƒg‚Æ‚Ì‹——£
    public float xSpeed = 120.0f;            // …•½•ûŒü‰ñ“]‘¬“x
    public float ySpeed = 120.0f;            // ‚’¼•ûŒü‰ñ“]‘¬“x
    public float yMinLimit = -20f;           // ‚’¼‰ñ“]‚Ì‰ºŒÀ
    public float yMaxLimit = 80f;            // ‚’¼‰ñ“]‚ÌãŒÀ

    private float x = 0.0f;                  // Œ»İ‚ÌX‰ñ“]
    private float y = 0.0f;                  // Œ»İ‚ÌY‰ñ“]

    private PlayerControl m_Controls;

    void Start()
    {
        m_Controls = new PlayerControl();
        m_Controls.GamePlay.Enable();

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("CameraPoint");
            if (obj != null)
            {
                target = obj.transform;
            }
        }

        if (target != null)
        {
            Vector2 look = m_Controls.GamePlay.Look.ReadValue<Vector2>();

            x += look.x * xSpeed * Time.deltaTime;
            y -= look.y * ySpeed * Time.deltaTime;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}
