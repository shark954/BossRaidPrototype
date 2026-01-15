using UnityEngine;

public class IceAttack : MonoBehaviour
{
    public int damage = 20;
    public string targetTag = "Enemy"; // 対象のタグ（敵）

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Parameta parameta = other.GetComponent<Parameta>();
            if (parameta != null)
            {
                parameta.HitDamage(damage, "Player"); // "Player" = 攻撃側のチーム名
            }
        }
    }
}
