using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        protected List<Node> nodeList = new List<Node>();

        public Sequence(List<Node> nodeList)
        {
            this.nodeList = nodeList;
        }

        public override NodeState Evaluate()
        {
            bool isAnyNodeRunning = false;
            foreach(var node in nodeList)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Running:
                        isAnyNodeRunning = true;
                        break;
                    case NodeState.Success:
                        break;
                    case NodeState.Failure:
                        currentNodeState = NodeState.Failure;
                        return currentNodeState;
                    default:
                        break;
                }
            }
            currentNodeState = isAnyNodeRunning ? NodeState.Running : NodeState.Success;
            return currentNodeState;
        }
    }

}