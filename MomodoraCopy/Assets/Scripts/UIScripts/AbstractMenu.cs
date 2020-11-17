using UnityEngine;

namespace MomodoraCopy
{
    public abstract class AbstractMenu : IMenu
    {
        protected MenuManager menuManager = MenuManager.instance;

        public virtual void ShowMenu()
        {
            SetMenuDisplay();
            CheckArrowKey();
            CheckConfirmKey();
            CheckCancleKey();
            CheckEscapeKey();
        }
        public abstract void SetMenuDisplay();

        public virtual void CheckEscapeKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!menuManager.isGamePaused)
                {
                    menuManager.Pause();
                    return;
                }
                menuManager.Resume();
                menuManager.menuHandler.ExitMenu();
            }
        }
        public abstract void CheckArrowKey();
        public abstract void CheckConfirmKey();
        public abstract void CheckCancleKey();
    }

}