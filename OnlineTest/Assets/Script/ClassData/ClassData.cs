using UnityEngine;

/// <summary>
/// �e�N���X�i�E�Ɓj�̊�{����\�͒l���i�[����ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NewClassData", menuName = "Game/Class Data")]
public class ClassData : ScriptableObject
{
    [Header("��{���")]
    public string className; // �N���X���i��: �\�[�h�}���j
    public int classID;      // ���ʗp��ID
    public Sprite icon;      // �N���X�I�����ɕ\������A�C�R��
    public GameObject weaponPrefab;  // ����̃v���n�u
    public RuntimeAnimatorController animator; // �A�j���[�^�[

    [Header("�X�e�[�^�X")]
    public float maxHP;
    public float moveSpeed;
    public float attackPower;
    public float attackRange;
    public float attackSpeed;
}
