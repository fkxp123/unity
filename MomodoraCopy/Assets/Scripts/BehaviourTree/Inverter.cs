using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Inverter : Node
    {
        protected Node node;

        public Inverter(Node node)
        {
            this.node = node;
        }

        public override NodeState Evaluate()
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    currentNodeState = NodeState.Running;
                    break;
                case NodeState.Success:
                    currentNodeState = NodeState.Failure;
                    break;
                case NodeState.Failure:
                    currentNodeState = NodeState.Success;
                    break;
                default:
                    break;
            }
            
            return currentNodeState;
        }
    }

}