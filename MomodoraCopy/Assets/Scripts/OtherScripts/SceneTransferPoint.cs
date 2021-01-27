using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransferPoint : MonoBehaviour
{
    public string transferSceneName;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.transform.tag == "Player")
        {
            SceneManager.LoadScene(transferSceneName);
        }
    }
}
