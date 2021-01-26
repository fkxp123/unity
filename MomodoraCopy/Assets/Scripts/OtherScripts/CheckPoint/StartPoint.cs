using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class StartPoint : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GameManager.instance.playerObject.transform.position = transform.position;   
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
