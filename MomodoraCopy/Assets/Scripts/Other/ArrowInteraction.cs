using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace MomodoraCopy
{
    public class ArrowInteraction : MonoBehaviour
    {
        public GameObject[] reactionObjects;

        Coroutine glowRoutine;

        Light2D light2D;

        void OnEnable()
        {
            glowRoutine = StartCoroutine(GlowOrb());
        }
        void OnDisable()
        {
            StopCoroutine(glowRoutine);
        }

        public void Interact()
        {
            StopCoroutine(glowRoutine);
            light2D.intensity = 0;
            for(int i = 0; i < reactionObjects.Length; i++)
            {
                reactionObjects[i].SetActive(true);
            }
        }

        void Awake()
        {
            light2D = GetComponent<Light2D>();
            glowRoutine = StartCoroutine(GlowOrb());
        }

        IEnumerator GlowOrb()
        {
            int i = 1;
            while (true)
            {
                if(i > 0)
                {
                    light2D.intensity -= 0.1f;
                    yield return null;
                    if(light2D.intensity <= 0)
                    {
                        i *= -1;
                    }
                }
                if(i < 0)
                {
                    light2D.intensity += 0.1f;
                    yield return null;
                    if (light2D.intensity >= 5)
                    {
                        i *= -1;
                    }
                }
            }
        }
    }

}