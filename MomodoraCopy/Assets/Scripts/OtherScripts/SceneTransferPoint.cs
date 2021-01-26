using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransferPoint : MonoBehaviour
{
    public string transferSceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("hi");

        if(coll.transform.tag == "Player")
        {
            Debug.Log("hi");
            SceneManager.LoadScene(transferSceneName);
        }
    }
}
