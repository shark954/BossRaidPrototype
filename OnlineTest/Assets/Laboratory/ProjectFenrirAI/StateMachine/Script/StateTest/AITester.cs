using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Reflection;

namespace StateMachineAI
{
    /// <summary>
    /// 敵のステートリスト
    /// ここでステートを登録していない場合、
    /// 該当する行動が全くでなきい。
    /// </summary>
    /// 
    public enum AIState_ABType
    {
        A_Mode,
        B_Mode,
    }


    public class AITester 
        : StatefulObjectBase<AITester, AIState_ABType>
    {
        public int DDDD;
        void Start()
        {
            //S_TypeA ステートを登録する(ステートリスト0番目)
            stateList.Add(new S_TypeA(this));
            //S_TypeB ステートを登録する(ステートリスト1番目)
            stateList.Add(new S_TypeB(this));

            //ステートマシーンを自身として設定
            stateMachine = new StateMachine<AITester>();

            //初期起動時は、A_Modeに移行させる
            ChangeState(AIState_ABType.A_Mode);
        }
    }
}
