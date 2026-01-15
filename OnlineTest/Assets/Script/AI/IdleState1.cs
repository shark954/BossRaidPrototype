using AIStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StateMachineAI
{
    public class IdleState1 : State<AIstate>
    {

        public IdleState1(AIstate owner) : base(owner) { }


        public override void Enter()
        {
            Debug.Log("IdolStart");
        }

        public override void Stay()
        {
            Tracker();
            Dead();
        }

        public override void Exit() 
        {
            Debug.Log("IdolEnd");
        }
        private void Tracker()
        {
            if (owner.m_target)
            {
                owner.m_agent.destination = owner.transform.position;
                //プレイヤーとターゲットの距離が、指定した攻撃距離より遠かった場合、追跡へステート切り替え
                if (owner.m_AttackKyori <= Vector3.Distance(owner.m_target.position, owner.transform.position) && owner.m_target)
                    owner.ChangeState(AI_State.Chase);
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

