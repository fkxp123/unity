using UnityEngine;

namespace MomodoraCopy
{
    public abstract class SelectedMenu : MonoBehaviour, IMenu
    {
        Vector3 position = new Vector3(0, -240, 0);
        public virtual void OnStartMenu()
        {
            //menuManager.mainMenuObjectStatic.SetActive(false);
            //menuManager.mainMenuObjectDynamic.SetActive(false);
            //menuManager.selectedMenuObject.SetActive(true);
            //SetBackgroundAlpha(0.5f);
            //SetKeyDescriptionBarPosition(position);
        }
        public virtual void UpdateMenu()
        {

        }

        public virtual void CheckEscapeKey()
        {

        }
        public abstract void CheckArrowKey();
        public abstract void CheckConfirmKey();

        public virtual void CheckCancleKey()
        {
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    menuManager.menuHandler.ChangeMenuType(menuManager.mainMenu);
            //    menuManager.selectedMenuObject.SetActive(false);
            //    menuManager.selectedMenuObjectList[menuManager.mainMenuSlotCount].SetActive(false);
            //}
        }
    }

}