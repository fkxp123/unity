using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Selector : Node
    {
        protected List<Node> nodeList = new List<Node>();

        public Selector(List<Node> nodeList)
        {
            this.nodeList = nodeList;
        }

        public override NodeState Evaluate()
        {
            foreach (var node in nodeList)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Running:
                        currentNodeState = NodeState.Running;
                        return currentNodeState;
                     case NodeState.Success:
                        currentNodeState = NodeState.Success;
                        return currentNodeState;
                    case NodeState.Failure:
                        break;
                    default:
                        break;
                }
            }
            currentNodeState = NodeState.Failure;
            return currentNodeState;
        }
    }

}