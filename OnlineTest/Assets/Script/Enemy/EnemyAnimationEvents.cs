using AIStateMachine;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public AIstate ai;

    public void HitBoxPop()
    {
        if (ai != null) ai.PerformHitBoxPop();
    }

    public void ShootBullet()
    {
        if (ai != null) ai.PerformShootBullet();
    }

    public void FlySlash()
    {
        if (ai != null) ai.PerformSlashBoxPop();
    }
}
