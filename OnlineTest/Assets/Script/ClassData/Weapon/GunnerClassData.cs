using UnityEngine;

[CreateAssetMenu(menuName = "CharacterClass/Gunner")]
public class GunnerClassData : WeaponClassData
{
    public float m_fireRate;
    public float m_bulletSpread;
    public float m_reloadTime;
    public GameObject m_bulletPrefab;
    public GameObject m_grenadePrefab;
}

