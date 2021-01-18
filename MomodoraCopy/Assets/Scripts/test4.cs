using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Check");
        //StartCoroutine("waitFunction1");
        //StartCoroutine("waitFunction2");
    }
    IEnumerator Check()
    {
        Debug.Log("coroutine : " + Time.time);
        yield return new WaitForSeconds(0.01f);
        StartCoroutine("Check");
    }

    IEnumerator waitFunction1()
    {
        Debug.Log("Func1 Before Waiting");
        yield return new WaitForSeconds(3); //Will wait for 3 seconds then run the code below
        Debug.Log("Func1 After waiting for 3 seconds");
    }

    IEnumerator waitFunction2()
    {
        const float waitTime = 3f;
        float counter = 0f;

        Debug.Log("Func2 Before Waiting");
        while (counter < waitTime)
        {
            Debug.Log("Func2 Current WaitTime: " + counter);
            counter += Time.deltaTime;
            yield return null; //Don't freeze Unity
        }
        Debug.Log("Func2 After waiting for 3 seconds");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("update : " + Time.time);
    }
}
