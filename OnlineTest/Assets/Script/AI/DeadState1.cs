using AIStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StateMachineAI
{
    public class DeadState1 : State<AIstate>
    {

        public DeadState1(AIstate owner) : base(owner) { }


        public override void Enter()
        {
            owner.m_animator.SetBool("Death", true);
           
            owner.DeadSystem();
        }

        public override void Stay()
        {
        }

        public override void Exit()
        {
        }
    }
}

