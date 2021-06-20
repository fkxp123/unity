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
            if (Input.GetKeyDown(KeyCode.A))
            {
                OperateMenuConfirm();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                OperateMenuCancle();
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.MenuKey))
            {
                OperateMenuEscape();
            }
            else
            {
                CheckArrowKey();
            }
        }
        protected virtual void OnDisable()
        {
            if (backgroundMenu == null)
                return;
            backgroundMenu.menuBackground.SetActive(false);
            backgroundMenu.keyDescriptionBar.SetActive(false);
        }

        protected virtual void OperateMenuConfirm()
        {  
            enabled = false;
            MenuManager.instance.menuMemento.SetMenuMemento(gameObject);
        }
        protected virtual void OperateMenuCancle()
        {
            enabled = false;
            MenuManager.instance.isDisableByEscapeKey = false;
            MenuManager.instance.menuMemento.componentsStack.Pop().enabled = true;
        }
        protected virtual void OperateMenuEscape()
        {
            enabled = false;
            MenuManager.instance.isDisableByEscapeKey = true;
            MenuManager.instance.menuMemento.componentsStack.Clear();
        }

        public abstract void CheckArrowKey();

        #region ChangeSlotImageFunctions
        protected void ChangeSelectedSlotMenu(Image[] slotImage, int selectedCount)
        {
            SetInvisibleAllSelectedSlots(slotImage);
            SetVisibleSelectedSlot(slotImage, selectedCount);
        }
        protected void SetInvisibleAllSelectedSlots(Image[] slotImage)
        {
            Color temp;
            for (int i = 0; i < slotImage.Length; i++)
            {
                temp = slotImage[i].color;
                temp.a = 0.0f;
                slotImage[i].color = temp;
            }
        }
        protected void SetVisibleSelectedSlot(Image[] slotImage, int selectedCount)
        {
            Color temp = slotImage[selectedCount].color;
            temp.a = 1.0f;
            slotImage[selectedCount].color = temp;
        }
        #endregion
    }

}