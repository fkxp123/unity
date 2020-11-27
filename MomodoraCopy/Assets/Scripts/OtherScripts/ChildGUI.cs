using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildGUI : TestGUI
{
    TestGUI tg;
    ChildGUI cg;

    protected override void Awake()
    {
        cg = GetComponent<ChildGUI>();
        tg = GetComponent<TestGUI>();
    }

    protected override void Start()
    {
        Debug.Log("im childGUI");
        //cg.enabled = false;
    }
    protected override void Update()
    {
        TestFunc2();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            tg.enabled = true;
            enabled = false;
        }
    }
    protected override void TestFunc()
    {
        Debug.Log("updating ChildGUI");
    }
}
