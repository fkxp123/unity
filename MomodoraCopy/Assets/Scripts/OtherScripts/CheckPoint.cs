using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField]
    GameObject bell;

    public float maxAngleDeflection;
    float playerDirection;
    float MaxAngleDeflection
    {
        get { return 0; }
        set
        {
            maxAngleDeflection = value;
            CancelInvoke("PendulumMovement");
            currentTime = 0;
            InvokeRepeating("PendulumMovement", 0, 0.01f);
        }
    }
    float pendulumSpeed = 5.0f;
    float currentTime;

    //void Update()
    //{
    //    if (maxAngleDeflection > 0)
    //    {
    //        float angle = playerDirection * maxAngleDeflection * Mathf.Sin(currentTime * pendulumSpeed);
    //        currentTime += Time.deltaTime;
    //        maxAngleDeflection -= Time.deltaTime * pendulumSpeed;
    //        bell.transform.localRotation = Quaternion.Euler(0, 0, angle);
    //        Debug.Log("Angle : " + angle);
    //        Debug.Log("maxAngleDeflection : " + maxAngleDeflection);
    //    }
    //}

    void PendulumMovement()
    {
        if (maxAngleDeflection > 0)
        {
            float angle = playerDirection * maxAngleDeflection * Mathf.Sin(currentTime * pendulumSpeed);
            currentTime += 0.01f;
            maxAngleDeflection -= 0.01f * pendulumSpeed;
            bell.transform.localRotation = Quaternion.Euler(0, 0, angle);
            Debug.Log("Angle : " + angle);
            Debug.Log("maxAngleDeflection : " + maxAngleDeflection);
        }
    }
    public void SetBellAngle(float playerDirection)
    {
        this.playerDirection = playerDirection;
        MaxAngleDeflection = 30;
    }
}
