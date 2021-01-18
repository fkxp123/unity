using UnityEngine;

namespace MomodoraCopy
{
    public class PostTest : MonoBehaviour, IListener
    {
        public int _health = 100;
        public int Health
        {
            get { return _health; }
            set
            {
                _health = Mathf.Clamp(value, 0, 100);
                if (_health <= 0)
                {
                    EventManager.instance.PostNotification(EVENT_TYPE.GAME_END);
                }
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            EventManager.instance.PostNotification(EVENT_TYPE.GAME_INIT);
            EventManager.instance.AddListener(EVENT_TYPE.GAME_END, this);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Health -= 20;
            }
        }

        public void OnEvent(EVENT_TYPE Event_Type, IListener Listener)
        {
            Debug.Log("Add " + Event_Type + " Event + in " + Listener);
        }
    }

}