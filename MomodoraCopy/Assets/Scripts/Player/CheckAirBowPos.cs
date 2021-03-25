using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAirBowPos : MonoBehaviour
{
    public bool isColliderEnter;
    
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.tag == "platform")
        {
            Debug.Log("im enter");
            isColliderEnter = true;
            return;
        }
        isColliderEnter = true;
    }
}
