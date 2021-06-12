using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    public class MainMap : MonoBehaviour
    {
        void Start()
        {
            CameraManager.instance.SetCameraBounds(transform.GetChild(0).GetComponent<Tilemap>());
        }
    }

}