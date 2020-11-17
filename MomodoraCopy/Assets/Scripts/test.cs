using UnityEngine;

namespace MomodoraCopy
{
    public class test : MonoBehaviour
    {
        public MonoBehaviour[] Handler;
        // Start is called before the first frame update
        void Start()
        {
            foreach (MonoBehaviour h in Handler)
                h.Invoke("OnSave", 0.0f);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}