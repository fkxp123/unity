using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public enum NodeState
    {
        Running, Success, Failure
    }

    [System.Serializable]
    public abstract class Node
    {
        protected NodeState currentNodeState;
        public NodeState CurrentNodeState
        {
            get { return currentNodeState; }
        }

        public abstract NodeState Evaluate();
    }

}