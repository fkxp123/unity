using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    ObjectPooler objectPooler;
    public GameObject arrowPrefab;
    float arrowRotateY;
    GameObject target;
    Player player;
    float objectActivatedTime = 1.5f;
    PoolingObjectInfo info = new PoolingObjectInfo();
    float originPosX;
    float originPosY;
    float originPosZ;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("player");
        player = target.GetComponent<Player>();
        objectPooler = ObjectPooler.instance;
        SetPoolingObjectInfo();
        objectPooler.CreatePoolingObjectQueue(info, 10);
    }
    void Update() //delete Update and use bool type property ex)player.isPlayerShootArrow ?
    {
        if (Input.GetKeyUp(KeyCode.D))
        {
            transform.position = new Vector3(player.transform.position.x,
                                             player.transform.position.y + 0.65f,
                                             player.transform.position.z);
        }
        if (player.stateMachine.CurState != player.idle && //if use property, delete this
           player.stateMachine.CurState != player.jump &&
           player.stateMachine.CurState != player.fall &&
           player.stateMachine.CurState != player.run &&
           player.stateMachine.CurState != player.crouch)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (player.stateMachine.CurState == player.crouch)
            {
                transform.position = new Vector3(player.transform.position.x,
                                                 player.transform.position.y - 0.1f, 
                                                 player.transform.position.z);
            }
            else if (player.stateMachine.CurState == player.jump || player.stateMachine.CurState == player.fall)
            {
                transform.position = new Vector3(player.transform.position.x,
                                                 player.transform.position.y + 0.75f,
                                                 player.transform.position.z);
            }
            GameObject recycleObject;
            SetArrowDirection();
            SetPoolingObjectInfo();
            recycleObject = objectPooler.GetPoolingObject(info);
            StartCoroutine(CheckPoolingObjectActivateTime(arrowPrefab, recycleObject));
        }
    }
    void SetPoolingObjectInfo()
    {
        info.prefab = arrowPrefab;
        info.spawner = gameObject;
        info.objectRotateY = arrowRotateY;
    }
    void SetArrowDirection()
    {
        if (target.GetComponent<PlayerMovement>().spriteRenderer.flipX)
        {
            arrowRotateY = 180.0f;
            return;
        }
        arrowRotateY = 0.0f;
    }
    IEnumerator CheckPoolingObjectActivateTime(GameObject prefab, GameObject clone)
    {
        yield return new WaitForSeconds(objectActivatedTime);
        RecyclePoolingObject(prefab, clone);
    }
    void RecyclePoolingObject(GameObject prefab, GameObject clone)
    {
        objectPooler.RecyclePoolingObject(prefab, clone);
    }
}
