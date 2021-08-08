using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace MomodoraCopy
{
    public class CinemachineShake : MonoBehaviour
    {
        CinemachineVirtualCamera cinemachineVirtualCamera;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

        float totalShakeTime;
        float originIntensity;

        GameObject mainCamera;

        void Start()
        {
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
            cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            //cinemachineVirtualCamera.Follow = GameManager.instance.playerPhysics.transform;
            mainCamera = transform.parent.gameObject;
        }

        public IEnumerator Shake(float intensity, float time)
        {
            float currentTime = time;
            totalShakeTime = time;

            originIntensity = intensity;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            cinemachineBasicMultiChannelPerlin.m_FrequencyGain = intensity;

            while (currentTime > 0)
            {
                //cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(originIntensity, 0f, currentTime / totalShakeTime);

                currentTime -= Time.deltaTime;
                yield return null;
            }
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0;
            mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}