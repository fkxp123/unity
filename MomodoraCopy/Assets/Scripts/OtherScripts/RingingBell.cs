using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingingBell : MonoBehaviour
{
    [SerializeField]
    GameObject checkArea;
    [SerializeField]
    float checkRadius;
    float playerDirection = 1;
    bool isSaved;
    float angle;
    float num = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
        if (angle < 15 * num)
        {
            angle += 1 * num;
            if(angle == 15)
            {
                num *= -1;
            }
        }
        
        if (isSaved)
        {
            transform.rotation = Quaternion.Euler(0, 0, 30 * playerDirection - Time.deltaTime);
        }
    }

    public void SaveCheckPoint()
    {
        isSaved = true;
    }

    void CheckPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkArea.transform.position, checkRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Player")
            {

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkArea.transform.position, checkRadius);
    }
}
