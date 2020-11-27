using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public abstract class AbstractMenu : MonoBehaviour
    {
        [SerializeField]
        protected GameObject backgroundMenuObject;
        protected BackgroundMenu backgroundMenu;

        protected Vector3 KeyDescriptionBarHighPos = new Vector3(0, -15, 0);
        protected Vector3 KeyDescriptionBarLowPos = new Vector3(0, -240, 0);

        protected virtual void Awake()
        {
            backgroundMenu = backgroundMenuObject.GetComponent<BackgroundMenu>();
            enabled = false;
        }
        protected virtual void OnEnable()
        {
            backgroundMenu.menuBackground.SetActive(true);
            backgroundMenu.keyDescriptionBar.SetActive(true);
        }

        protected virtual void Update()
        {
            CheckArrowKey();
            CheckConfirmKey();
            CheckCancleKey();
            CheckEscapeKey();
        }
        protected virtual void OnDisable()
        {
            if (backgroundMenu == null)
                return;
            backgroundMenu.menuBackground.SetActive(false);
            backgroundMenu.keyDescriptionBar.SetActive(false);
        }

        public virtual void CheckEscapeKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.instance.Resume();
                enabled = false;
            }
        }
        public abstract void CheckArrowKey();
        public abstract void CheckConfirmKey();
        public abstract void CheckCancleKey();
    }

}