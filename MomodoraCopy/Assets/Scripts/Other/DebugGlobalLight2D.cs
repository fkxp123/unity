using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyNamespace
{
    public class DebugGlobalLight2D : MonoBehaviour
    {
        void Awake()
        {
            DestroyImmediate(gameObject);
        }
    }

}