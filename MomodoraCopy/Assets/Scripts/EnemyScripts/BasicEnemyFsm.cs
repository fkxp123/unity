﻿using System.Collections;
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

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            currentState = State.Idle;
            transitionState = State.Idle;
            executeState = State.None;
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