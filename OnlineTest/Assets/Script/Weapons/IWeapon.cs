using UnityEngine;

/// <summary>
/// ���ׂĂ̕���ɋ��ʂ���C���^�[�t�F�[�X�B
/// �X�e�[�^�X���ƃA�N�V�����̗������܂ށB
/// </summary>
public interface IWeapon
{
    // === �X�e�[�^�X�֘A ===

    /// <summary>��{�_���[�W</summary>
    int Damage { get; }

    /// <summary>�`���[�W�Ȃǂŉ��Z�����ǉ��U����</summary>
    float AddPower { get; }

    /// <summary>���݂̃`���[�W�i�K</summary>
    int ChargeCount { get; set; }

    /// <summary>�`���[�W�i�K�̍ő�l</summary>
    int MaxChargeCount { get; set; }

    /// <summary>�U���q�b�g���ɐ�������G�t�F�N�g</summary>
    GameObject AttackEffect { get; }

    /// <summary>�U���G�t�F�N�g�̏��Ŏ���</summary>
    float AttackEffectDelTime { get; }

    /// <summary>�`���[�W���ɐ�������G�t�F�N�g</summary>
    GameObject ChargeEffect { get; }

    /// <summary>�`���[�W�G�t�F�N�g�̏��Ŏ���</summary>
    float ChargeEffectDelTime { get; }

    // === �A�N�V�����֘A ===

    /// <summary>������g�p����i�ʏ�U���Ȃǁj</summary>
    void Use();

    /// <summary>����̃^�C�v�i���E�e�E���@�Ȃǁj���擾����</summary>
    WeaponType GetWeaponType();
}
