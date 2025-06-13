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
        // クライアントが自分のクラスIDをサーバーに送る（例: 事前にUI選択で保存）
        CmdSetClass(PlayerPrefs.GetInt("SelectedClassID", 0));
    }

    [Command]
    void CmdSetClass(int id)
    {
        classID = id; // これでSyncVarが全クライアントに伝わる
    }

    void OnClassChanged(int oldID, int newID)
    {
        classData = database.GetClassByID(newID);
        if (classData == null) return;

        // パラメータ反映
        maxHP = classData.maxHP;
        moveSpeed = classData.moveSpeed;
        attackPower = classData.attackPower;

        // 武器装備
        if (weaponSlot.transform.childCount > 0)
            Destroy(weaponSlot.transform.GetChild(0).gameObject);
        Instantiate(classData.weaponPrefab, weaponSlot.transform);

        // アニメーター適用
        if (animator != null)
            animator.runtimeAnimatorController = classData.animator;
    }
}
