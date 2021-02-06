using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class BasicEnemyFsm : MonoBehaviour
    {
        public enum State
        {
            None, Idle, Patrol, Chase, Attack, Interact, Hurt, Die
        }

        public State currentState;       
        [SerializeField]
        protected State transitionState;
        [SerializeField]
        protected State executeState;

        public State none;
        public State idle;
        public State patrol;
        public State chase;
        public State attack;
        public State interact;
        public State hurt;
        public State die;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            currentState = State.Idle;
            transitionState = State.Idle;
            executeState = State.None;

            none = State.None;
            idle = State.Idle;
            patrol = State.Patrol;
            chase = State.Chase;
            attack = State.Attack;
            interact = State.Interact;
            hurt = State.Hurt;
            die = State.Die;
        }

        protected virtual void Update()
        {
            switch (currentState)
            {
                case State.Idle:
                    DoIdle();
                    break;
                case State.Patrol:
                    DoPatrol();
                    break;
                case State.Chase:
                    DoChase();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.Hurt:
                    DoHurt();
                    break;
                case State.Die:
                    DoDie();
                    break;
                default:
                    break;
            }
        }

        protected virtual void ExecuteState(State executeState)
        {
            switch (executeState)
            {
                case State.Idle:
                    DoIdle();
                    break;
                case State.Patrol:
                    DoPatrol();
                    break;
                case State.Chase:
                    DoChase();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.Hurt:
                    DoHurt();
                    break;
                case State.Die:
                    DoDie();
                    break;
                default:
                    break;
            }
        }
        protected virtual void DoIdle()
        {

        }
        protected virtual void DoPatrol()
        {

        }
        protected virtual void DoChase()
        {

        }
        protected virtual void DoAttack()
        {

        }
        protected virtual void DoHurt()
        {

        }
        protected virtual void DoDie()
        {

        }
    }
}
