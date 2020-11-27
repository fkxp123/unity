using UnityEngine;
using System.Reflection;

namespace MomodoraCopy
{
    public class PlayerStateMachine
    {
        public IState CurState { get; private set; }//read only
        public IState OldState { get; private set; }
        public PlayerStateMachine(IState defaultState)
        {
            CurState = defaultState;
        }
        public void SetState(IState state)
        {
            if (CurState == state)
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
            CurState.OperateUpdate();
        }
        public void ChangeState(IState state)
        {
#if STATE_DEBUG_MOD
            ClearLog();
#endif
            CurState.OperateExit();
            OldState = CurState;
            CurState = state;
            CurState.OperateEnter();
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