using UnityEngine;
using Mirror;

public class PlayerClassSetup : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnClassChanged))]
    public int classID;

    public ClassDatabase database;
    private ClassData classData;

    public GameObject weaponSlot;
    public Animator animator;

    public float maxHP;
    public float moveSpeed;
    public float attackPower;

    public override void OnStartLocalPlayer()
    {
        // �N���C�A���g�������̃N���XID���T�[�o�[�ɑ���i��: ���O��UI�I���ŕۑ��j
        CmdSetClass(PlayerPrefs.GetInt("SelectedClassID", 0));
    }

    [Command]
    void CmdSetClass(int id)
    {
        classID = id; // �����SyncVar���S�N���C�A���g�ɓ`���
    }

    void OnClassChanged(int oldID, int newID)
    {
        classData = database.GetClassByID(newID);
        if (classData == null) return;

        // �p�����[�^���f
        maxHP = classData.maxHP;
        moveSpeed = classData.moveSpeed;
        attackPower = classData.attackPower;

        // ���푕��
        if (weaponSlot.transform.childCount > 0)
            Destroy(weaponSlot.transform.GetChild(0).gameObject);
        Instantiate(classData.weaponPrefab, weaponSlot.transform);

        // �A�j���[�^�[�K�p
        if (animator != null)
            animator.runtimeAnimatorController = classData.animator;
    }
}
