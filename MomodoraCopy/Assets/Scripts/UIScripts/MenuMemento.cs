using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class MenuMemento
    {
        GameObject gameObject;
        MonoBehaviour component;
        public Stack<MonoBehaviour> componentsStack = new Stack<MonoBehaviour>();

        public void SetMenuMemento(GameObject gameObject)
        {
            this.gameObject = gameObject;
            component = gameObject.GetComponent<MonoBehaviour>();
            componentsStack.Push(component);
        }
    }
}