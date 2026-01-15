using StateMachineAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

namespace AIStateMachine
{
    public enum AI_State
    {
        Idle,
        Chase,
        Attack,
        Dead,
    }



    public class AIstate : StatefulObjectBase<AIstate, AI_State>
    {
        [Header("ナビ")]
        public NavMeshAgent m_agent;
        [Header("ターゲット")]
        public Transform m_target;
        [Header("アニメーター")]
        public Animator m_animator;
        [Header("攻撃可能距離")]
        public float m_AttackKyori = 2.0f;
        [Header("攻撃可能最短")]
        public float m_AttackMin = 2.0f;
        [Header("攻撃可能最長")]
        public float m_AttackMax = 2.0f;
        [Header("パラメーター")]
        public Parameta m_Parameta;
        [Header("攻撃最大クールタイム")]
        public float m_AttackMaxCool=0.0f;

        public float m_AttackCoolTime = 0.0f;
        [Header("ゲームマネージャー")]
        public GameManager m_gameManager;
        public EnemyManager m_enemyManager;
        [Header("攻撃用オブジェクト")]
        public GameObject m_hitbox;
        //攻撃用オブジェクトの出現ポイント
        public Transform m_hitBoxPotion;
        //public Weapon m_weapon;
        //[Header("敵用のGetArms")]
        //public EnemyWeapon m_enemyWeapon;
        //[Header("攻撃用タイムラインリスト")]
        //public List<PlayableDirector> m_directors;
        [Header("敵がプレイヤーに向く速さ")]
        public float m_AttackRotationSpeed = 5.0f;

        public enum EnemyType
        {
            Melee,
            Sword,
            Rifle
        }

        [Header("敵タイプ")]
        public EnemyType m_enemyType;

        private void Start()
        {
            m_agent = GetComponent<NavMeshAgent>();

            stateList.Add(new IdleState1(this));

            stateList.Add(new ChaseState1(this));

            stateList.Add(new AttackState1(this));

            stateList.Add(new DeadState1(this));


            stateMachine = new StateMachine<AIstate>();

            ChangeState(AI_State.Idle);
            if (m_target == null)
            {
                GameObject dummy = GameObject.FindWithTag("Player");

                m_target = dummy.transform;
            }

            if (m_enemyManager == null)
            {
                GameObject popPoint = GameObject.FindWithTag("EnemyManager");

                m_enemyManager = popPoint.GetComponent<EnemyManager>();
            }

            float rand = Random.Range(0f, 1f);
            m_AttackKyori = Mathf.Sqrt(rand) * (m_AttackMax - m_AttackMin) + m_AttackMin;
            Debug.Log($"{name} m_AttackKyori = {m_AttackKyori}");

        }

        public void DeadSystem()
        {
            //HPがなくなったら消す(消すまでの猶予）
            m_Parameta.Die(5.0f);

            m_enemyManager.RemoveEnemy(gameObject);
            //Destroy(this.gameObject, 10.0f);
        }

        public void PerformHitBoxPop()
        {
            Debug.Log("HitBoxPop called!");
            if (!m_gameManager.m_Overflag && !m_gameManager.m_Clearflag)
            {
                if (!m_Parameta.m_death)
                {
                    GameObject dummy = Instantiate(m_hitbox, m_hitBoxPotion.position, m_hitBoxPotion.rotation);
                    dummy.GetComponent<HitDamage>().m_team = GetComponent<Parameta>().m_team;
                    m_animator.SetBool("Attack", false);
                }
            }
        }

        public void PerformSlashBoxPop()
        {
            Debug.Log("HitBoxPop called!");
            if (!m_gameManager.m_Overflag && !m_gameManager.m_Clearflag)
            {
                if (!m_Parameta.m_death)
                {
                    GameObject dummy = Instantiate(m_hitbox, m_hitBoxPotion.position, m_hitBoxPotion.rotation);
                    dummy.GetComponent<HitDamage>().m_team = GetComponent<Parameta>().m_team;
                    m_animator.SetBool("Slash", false);
                }
            }
        }

        public void PerformShootBullet()
        {
            Debug.Log("ShootBullet called!");
            if (!m_Parameta.m_death)
            {
                // 仮の弾丸発射処理（弾丸プレハブがある場合）
                GameObject bullet = Instantiate(m_hitbox, m_hitBoxPotion.position, m_hitBoxPotion.rotation);
                bullet.GetComponent<HitDamage>().m_team = m_Parameta.m_team;
            }
        }
    }
}