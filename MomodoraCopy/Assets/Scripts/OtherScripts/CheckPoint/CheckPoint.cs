using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MomodoraCopy
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField]
        GameObject bell;

        public string sceneName;
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
        float pendulumSpeed = 6;
        float currentTime;

        Scene scene;

        void Start()
        {
            scene = SceneManager.GetActiveScene();
            sceneName = scene.name;
            CheckPointsData.AddCheckPoint(scene.name, gameObject);
            //Debug.Log(CheckPointsData.checkPointsDict[scene.name.GetHashCode()].Count);
            //SetPlayerPosition();
        }

        void SetPlayerPosition()
        {
            if(GameManager.instance.playerData.playerPosition == null)
            {
                GameManager.instance.playerObject.transform.position = CheckPointsData.checkPointsDict[scene.name.GetHashCode()][0].transform.position;
            }
        }

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


        //float MaxAngleDeflection = 30.0f;
        //float SpeedOfPendulum = 1.0f;
        //float angle = MaxAngleDeflection * Mathf.Sin(Time.time * SpeedOfPendulum);
        //myPivotTransform.localRotation = Quaternion.Euler( 0, 0, angle);

        // to set the initial angle?
        //Instead of Time.time, keep your on notion of float time; and then add to it each frame:
        //That way you can set its initial time to whatever you want to get the desired angle, 
        //and you can use Mathf.Asin() to back-calculate the desired time for an angle.

        void PendulumMovement()
        {
            if (maxAngleDeflection > 0)
            {
                float angle = playerDirection * maxAngleDeflection * Mathf.Sin(currentTime * pendulumSpeed);
                currentTime += 0.01f;
                maxAngleDeflection -= 0.01f * pendulumSpeed;
                bell.transform.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
        public void SetBellAngle(float playerDirection)
        {
            this.playerDirection = playerDirection;
            MaxAngleDeflection = 30;
            GameManager.instance.Save(gameObject);
        }
    }
}
