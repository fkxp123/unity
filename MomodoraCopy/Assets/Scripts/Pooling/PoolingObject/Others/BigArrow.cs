using UnityEngine;

namespace MomodoraCopy
{
    public class BigArrow : MonoBehaviour
    {
        public float arrowSpeed = 10;

        void Update()
        {
            transform.Translate(Vector2.right * arrowSpeed * Time.deltaTime);
        }
    }

}