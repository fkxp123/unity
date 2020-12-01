using UnityEngine;
using System.Reflection;

namespace MomodoraCopy
{
    public class PlayerStateMachine
    {
        public IState CurrentState { get; private set; }//read only
        public IState PreviousState { get; private set; }
        public PlayerStateMachine(IState defaultState)
        {
            CurrentState = defaultState;
        }
        public void SetState(IState state)
        {
            if (CurrentState == state)
            {
#if SHOW_DEBUG_MESSAGE
                Debug.Log("이미 " + state + "상태입니다");
#endif
            }
            else
            {
                ChangeState(state);
            }
        }
        public void DoOperateUpdate()
        {
            CurrentState.OperateUpdate();
        }
        public void ChangeState(IState state)
        {
#if STATE_DEBUG_MOD
            ClearLog();
#endif
            CurrentState.OperateExit();
            PreviousState = CurrentState;
            CurrentState = state;
            CurrentState.OperateEnter();
        }
        public void ClearLog()
        {
            //var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            //var type = assembly.GetType("UnityEditor.LogEntries");
            //var method = type.GetMethod("Clear");
            //method.Invoke(new object(), null);
        }
    }

}