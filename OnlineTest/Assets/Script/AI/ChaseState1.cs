using AIStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace StateMachineAI
{
    public class ChaseState1 : State<AIstate>
    {
        public ChaseState1(AIstate owner) : base(owner) { }

        public override void Enter()
        {
            owner.m_animator.SetBool("Run", true);
        }

        public override void Stay()
        {
            Tracker();
            Dead();
        }

        public override void Exit() 
        {
            owner.m_animator.SetBool("Run", false);
        }

        private void Tracker()
        {
            if (!owner.m_gameManager.m_Clearflag && !owner.m_gameManager.m_Overflag)
            { 
                // プレイヤーがターゲットとして設定されている場合
                if (owner.m_target)
                {
                    float distanceToPlayer = Vector3.Distance(owner.m_target.position, owner.transform.position);

                    if (distanceToPlayer > owner.m_AttackKyori)
                    {
                        // ランダムにばらけさせるため、プレイヤー周辺にオフセットを加える
                        Vector3 offset = new Vector3(
                            Random.Range(-10.0f, 10.0f),
                            0,
                            Random.Range(-10.0f, 10.0f)
                        );

                        Vector3 targetPos = owner.m_target.position + offset;
                        owner.m_agent.destination = targetPos;
                    }
                    else
                    {
                        // 射程内 → 攻撃ステートに移行
                        owner.m_agent.destination = owner.transform.position; // 停止
                                                                              // ★カーリング防止：明示的に止める
                        if (owner.m_agent.remainingDistance <= owner.m_agent.stoppingDistance && !owner.m_agent.pathPending)
                        {
                            owner.m_agent.velocity = Vector3.zero;
                        }

                        owner.ChangeState(AI_State.Attack);
                    }
                }
            }
            else
            {
                // ターゲットがいない場合、Idle ステートに切り替え
                owner.m_agent.destination = owner.transform.position;
                owner.ChangeState(AI_State.Idle);
            }
        }

       
        

        private void Dead()
        {
            if (owner.m_Parameta.m_death)
            {
                owner.ChangeState(AI_State.Dead);
            }
        }

    }
}
