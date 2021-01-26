using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class CheckPointsData
    {
        public static Dictionary<int, List<GameObject>> checkPointsDict = new Dictionary<int, List<GameObject>>();
        public static List<GameObject> checkPointsList = new List<GameObject>();

        public static void AddCheckPoint(string sceneName, GameObject checkPoint)
        {
            int hash = sceneName.GetHashCode();
            checkPointsList.Add(checkPoint);
            if (checkPointsDict.ContainsKey(hash))
            {
                checkPointsDict.Remove(hash);
            }
            checkPointsDict.Add(hash, checkPointsList);
        }
    }

}