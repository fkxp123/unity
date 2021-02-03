using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class CheckPointManager : Singleton<CheckPointManager>
    {        
        public Dictionary<int, List<GameObject>> checkPointsDict = new Dictionary<int, List<GameObject>>();
        //public List<GameObject> checkPointsList = new List<GameObject>();

        public void AddCheckPoint(int sceneNameHash, GameObject checkPoint)
        {
            List<GameObject> checkPointsList;
            if (checkPointsDict.ContainsKey(sceneNameHash))
            {
                checkPointsList = checkPointsDict[sceneNameHash];
                checkPointsDict.Remove(sceneNameHash);
                checkPointsList.Add(checkPoint);
            }
            else
            {
                checkPointsList = new List<GameObject>()
                {
                    checkPoint
                };
            }
            checkPointsDict.Add(sceneNameHash, checkPointsList);
        }
    }

}