using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectedMenu : AbstractMenu
{
    public override  void ShowMenu()
    {
        base.ShowMenu();
    }
    public override abstract void SetMenuDisplay();

    public override void CheckEscapeKey()
    {
        base.CheckEscapeKey();
    }
    public override abstract void CheckArrowKey();
    public override abstract void CheckConfirmKey();
    public override void CheckCancleKey()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            menuManager.menuHandler.ChangeMenuType(menuManager.mainMenu);
            menuManager.mainMenuObject.SetActive(true);
            menuManager.selectedMenuObject.SetActive(false);
            menuManager.selectedMenuObjectList[menuManager.selectedCount].SetActive(false);
        }
    }
}
