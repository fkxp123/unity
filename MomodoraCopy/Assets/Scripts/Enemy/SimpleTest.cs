using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class SimpleTest : MonoBehaviour
    {
        EnemyMovement enemyMovement;
        // Start is called before the first frame update
        void Start()
        {
            enemyMovement = GetComponent<EnemyMovement>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                enemyMovement.direction.x = -1;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                enemyMovement.direction.x = 1;
            }
        }
    }
}
