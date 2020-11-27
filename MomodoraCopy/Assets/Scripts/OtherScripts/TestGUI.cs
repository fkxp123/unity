using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGUI : MonoBehaviour
{
    MonoBehaviour[] comps;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        comps = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in comps)
        {
            component.enabled = false;
        }
        GetComponent<TestGUI>().enabled = true;
        Debug.Log("im testGUI in awake");
    }
    protected virtual void Start()
    {
        Debug.Log("im testGUI");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        TestFunc();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GetComponent<ChildGUI>().enabled = true;
            GetComponent<TestGUI>().enabled = false;
        }
    }

    protected virtual void TestFunc()
    {
        Debug.Log("updating TestGUI");
    }
    protected virtual void TestFunc2()
    {
        Debug.Log("updating my TestGUI");
    }
}
