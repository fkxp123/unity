using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FireFlyController : MonoBehaviour
{
    Light2D light2D;
    float originRadius;
    Vector3 originScale;
    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
        originRadius = light2D.pointLightOuterRadius;
        originScale = light2D.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        light2D.pointLightOuterRadius = originRadius * transform.localScale.x / originScale.x; 
    }
}
