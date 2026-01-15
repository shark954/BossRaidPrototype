using AIStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
namespace StateMachineAI
{
    public class AttackState1 : State<AIstate>
    {
        

        GameObject knuckle;

        public AttackState1(AIstate owner) : base(owner)
        {

        }

        public override void Enter()
        {
          
            Debug.Log($"[AttackState] {owner.name} has entered Attack State");
            owner.m_agent.isStopped = true; // 攻撃中は移動を止める
            owner.m_AttackCoolTime = owner.m_AttackMaxCool;

            switch (owner.m_enemyType)
            {
                case AIstate.EnemyType.Melee:
                    owner.m_animator.SetBool("Attack", false);
                    break;
                case AIstate.EnemyType.Sword:
                    owner.m_animator.SetBool("Slash", false);
                    break;

                case AIstate.EnemyType.Rifle:
                    
                    break;
            }
        }

        public override void Stay()
        {
            Tracker();
          
            Dead();
        }

        public override void Exit()
        {
            owner.m_agent.isStopped = false; // 攻撃終了後に再開
            owner.m_animator.SetBool("Attack", false);
            owner.m_animator.ResetTrigger("Shoot");
        }

        #region 攻撃処理
        public void Tracker()
        {
            // ゲームが終了またはクリア状態でないか確認
            if (!owner.m_gameManager.m_Clearflag && !owner.m_gameManager.m_Overflag)
            {
                // ターゲットが存在するか確認
                if (owner.m_target)
                {
                    LookAtTarget(); // ★クールタイム中でも常に向かせる

                    // クールタイムが経過していれば攻撃実行
                    if (owner.m_AttackCoolTime >= owner.m_AttackMaxCool)
                    {
                        LookAtTarget();
                        // 敵タイプに応じてアニメーションパラメータを設定
                        switch (owner.m_enemyType)
                        {
                            case AIstate.EnemyType.Melee:
                                // 近接攻撃アニメーションを再生（Animator上に"Attack"パラメータが必要）
                                owner.m_animator.SetBool("Attack", true);
                                break;

                            case AIstate.EnemyType.Sword:
                                owner.m_animator.SetBool("Slash", true);
                                break;

                            case AIstate.EnemyType.Rifle:
                                // 射撃アニメーションをトリガー（Animator上に"Shoot"トリガーが必要）
                                owner.m_animator.SetTrigger("Shoot");
                                break;

                            default:
                                Debug.LogWarning("未定義の敵タイプです");
                                return; // ここで処理終了
                        }

                        // クールタイムをランダム範囲で再設定（自然な攻撃タイミングにするため）
                        owner.m_AttackCoolTime = owner.m_AttackMaxCool +
                            UnityEngine.Random.Range(-owner.m_AttackMaxCool / 2, owner.m_AttackMaxCool / 2);
                    }
                    else
                    {
                        // クールタイム中 → 時間を加算して待機
                        owner.m_AttackCoolTime += Time.deltaTime;
                    }
                }
            }
            else
            {
                // ゲーム終了状態またはターゲットがいない → Idle状態へ戻す
                owner.m_agent.destination = owner.transform.position;
                owner.ChangeState(AI_State.Idle);
            }
        }
        #endregion

        #region プレイヤーの方向を向く処理
        private void LookAtTarget()
        {
            // プレイヤーの方向を計算（高さ方向を無視）
            Vector3 directionToTarget = owner.m_target.position - owner.transform.position;
            directionToTarget.y = 0; // 水平面での方向のみにする

            if (directionToTarget != Vector3.zero)
            {
                // 現在の回転からプレイヤーの方向へ徐々に向く
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                owner.transform.rotation = Quaternion.Slerp(
                    owner.transform.rotation,
                    targetRotation,
                    owner.m_AttackRotationSpeed * Time.deltaTime
                );
            }
        }
        #endregion

        private void Dead()
        {
            if (owner.m_Parameta.m_death)
            {
                owner.ChangeState(AI_State.Dead);
            }
        }

    }
}


