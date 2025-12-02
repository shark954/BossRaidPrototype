using UnityEngine;

public class PlayerClassHandler : MonoBehaviour
{
    [SerializeField] private ClassData m_CurrentClass;
    private WeaponClassData m_WeaponData;

    /// <summary>
    /// 武器を変更してクラスチェンジ
    /// </summary>
    public void EquipWeapon(WeaponClassData newWeapon)
    {
        m_WeaponData = newWeapon;

        // 対応するClassDataを取得（マップしてる前提）
        ClassData newClass = FindClassForWeapon(m_WeaponData);
        ApplyClassData(newClass);

        Debug.Log($"クラスチェンジ完了：{m_CurrentClass.m_className}");
    }

    private ClassData FindClassForWeapon(WeaponClassData weapon)
    {
        // WeaponClassData → ClassData をマッピングして返す（例：Dictionary, ScriptableObjectリストから検索）
        // 簡易例（手動対応）：
        if (weapon is SwordClassData) return Resources.Load<ClassData>("SwordClass");
        if (weapon is GunnerClassData) return Resources.Load<ClassData>("GunnerClass");
        if (weapon is MageClassData) return Resources.Load<ClassData>("MageClass");
        if (weapon is TankClassData) return Resources.Load<ClassData>("TankClass");

        Debug.LogWarning("未知の武器データ。ClassDataに変換できません。");
        return null;
    }

    private void ApplyClassData(ClassData classData)
    {
        if (classData == null) return;

        m_CurrentClass = classData;

        // アニメーターやモデル差し替え処理などもここで
        GetComponent<Animator>().runtimeAnimatorController = m_CurrentClass.m_animator;

        // 他のステータス反映処理（例：HP、移動速度など）
    }
}
