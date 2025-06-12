using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Mirror; // Mirror�Ή��p

public class PlayerMove : NetworkBehaviour
{
    [Header("�ړ��֘A�ݒ�")]
    public float moveSpeed = 5f;         // �ʏ펞�̈ړ����x
    public float dashSpeed = 10f;        // �_�b�V�����̈ړ����x
    public float jumpForce = 5f;         // �W�����v���̗�

    [Header("�`���[�W�U���ݒ�")]
    public float chargeTimeThreshold = 0.4f; // �`���[�W�Ɣ��肷��ŏ�����

    private Rigidbody rb;
    private PlayerControl controls;      // InputSystem�Ŏ��������������̓N���X
    private Vector2 moveInput;           // �ړ����͂̒l
    private bool isGrounded = true;      // �n�㔻��i�W�����v�ہj

    // �_�b�V���ƃX�e�b�v����p
    private bool isEvadeHeld = false;
    private float evadeTimer = 0f;
    private float evadeHoldThreshold = 0.3f; // ����ȏ�̒������Ń_�b�V��

    // �`���[�W����
    private bool isCharging = false;
    float chargeStartTime;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControl(); // �����������ꂽInputActions�̃C���X�^���X��

        // �ړ�����
        controls.GamePlay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.GamePlay.Move.canceled += ctx => moveInput = Vector2.zero;

        // �W�����v�i�n��̂݁j
        controls.GamePlay.Jump.performed += ctx =>
        {
            if (isGrounded && isLocalPlayer)
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        };

        // Evade�i��� or �_�b�V���j�F�����n��
        controls.GamePlay.Evade.started += ctx =>
        {
            if (!isLocalPlayer) return;
            isEvadeHeld = true;
            evadeTimer = 0f;
        };

        // Evade�F�����I��莞�ɔ���
        controls.GamePlay.Evade.canceled += ctx =>
        {
            if (!isLocalPlayer) return;

            if (evadeTimer >= evadeHoldThreshold)
                Dash();    // �_�b�V������
            else
                Step();    // �X�e�b�v����

            isEvadeHeld = false;
        };

        // �U���F�����n�߁i�`���[�W�����j
        controls.GamePlay.Attack.started += ctx =>
        {
            if (!isLocalPlayer) return;
            isCharging = true;
            chargeStartTime = Time.time; // ���������Ԃ��L�^
            StartChargeEffect();
        };

        // �U���F�{�^���𗣂�����
        controls.GamePlay.Attack.canceled += ctx =>
        {
            if (!isLocalPlayer || !isCharging) return;

            float holdTime = Time.time - chargeStartTime;

            if (holdTime >= chargeTimeThreshold)
                CmdChargeAttack();   // �`���[�W�U��
            else
                CmdNormalAttack();   // �ʏ�U��

            isCharging = false;
            EndChargeEffect();
        };

        // ����U���i�����{�^���j
        controls.GamePlay.SpecialAttack.performed += ctx =>
        {
            if (isLocalPlayer)
                CmdSpecialAttack();
        };
    }

    // �v���C���[���������g�̂Ƃ��������͂�L����
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

        // �ړ������i�ʏ� or �_�b�V���j
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        float speed = isEvadeHeld && evadeTimer >= evadeHoldThreshold ? dashSpeed : moveSpeed;
        rb.MovePosition(transform.position + move * speed * Time.fixedDeltaTime);

        // �_�b�V���̉������ԃJ�E���g
        if (isEvadeHeld)
            evadeTimer += Time.deltaTime;
    }

    // ===============================
    // === �T�[�o�[�œ��������� ===
    // ===============================

    [Command]
    void CmdNormalAttack()
    {
        Debug.Log("�ʏ�U���i�T�[�o�[�ŏ����j");
        // �e�����E�U�������Ȃǂ������ɒǉ�
    }

    [Command]
    void CmdChargeAttack()
    {
        Debug.Log("�`���[�W�U���i�T�[�o�[�ŏ����j");
        // �`���[�W�e�⋭���U���̏���
    }

    [Command]
    void CmdSpecialAttack()
    {
        Debug.Log("����U���i�T�[�o�[�ŏ����j");
        // ����X�L���̔��������Ȃ�
    }

    // ===============================
    // === ���[�J���ł����������Ȃ����o�n ===
    // ===============================

    void Step() => Debug.Log("����X�e�b�v�i���[�J���̂݁j");
    void Dash() => Debug.Log("�_�b�V���ړ��i���[�J���̂݁j");
    void StartChargeEffect() => Debug.Log("�`���[�W���o�J�n�iUI/�G�t�F�N�g�j");
    void EndChargeEffect() => Debug.Log("�`���[�W���o�I���iUI/�G�t�F�N�g�j");
}
