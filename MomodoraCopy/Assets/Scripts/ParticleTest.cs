using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    public ParticleSystem pc;
    void Start()
    {
        pc = GetComponent<ParticleSystem>();    
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log(other);
            Debug.Log("platform!!!!!!!!!!");
        if(other.tag == "Platform")
        {
            Debug.Log("platform!!!!!!!!!!");
        }
    }
}
