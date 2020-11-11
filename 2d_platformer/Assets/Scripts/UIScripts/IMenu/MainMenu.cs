using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : AbstractMenu
{
    public override void ShowMenu()
    {
        base.ShowMenu();
    }

    public override void SetMenuDisplay()
    {
        SetInvisibleAllSelectedSlot();
        ChangeMenuDescription(menuManager.selectedCount);
        SetVisibleSelectedSlot(menuManager.selectedCount);
    }

    public override void CheckEscapeKey()
    {
        base.CheckEscapeKey();
    }
    public override void CheckArrowKey()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            menuManager.selectedCount += 1;
            if (menuManager.selectedCount >= menuManager.selectedSlot.transform.childCount)
            {
                menuManager.selectedCount = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            menuManager.selectedCount -= 1;
            if (menuManager.selectedCount < 0)
            {
                menuManager.selectedCount = menuManager.selectedSlot.transform.childCount - 1;
            }
        }
    }
    public override void CheckConfirmKey()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            menuManager.menuHandler.ChangeMenuType(menuManager.selectedMenuList[menuManager.selectedCount]);
            menuManager.mainMenuObject.SetActive(false);
            SetActiveFalseAllSelectedMenu();
            menuManager.selectedMenuObject.SetActive(true);
            SetActiveTrueSelectedMenu(menuManager.selectedCount);
        }
    }
    public override void CheckCancleKey()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            menuManager.Resume();
        }
    }

    void SetInvisibleAllSelectedSlot()
    {
        for (int i = 0; i < menuManager.selectedSlot.transform.childCount; i++)
        {
            menuManager.selectedSlots[i] = menuManager.selectedSlot.transform.GetChild(i).gameObject;
            menuManager.img[i] = menuManager.selectedSlots[i].GetComponent<Image>();
            menuManager.img[i].color = new Color(1f, 1f, 1f, 0f); //모든 '선택된 slot 이미지'를 안보이게함
        }
    }
    void SetVisibleSelectedSlot(int selectedCount)
    {
        menuManager.img[selectedCount].color = new Color(1f, 1f, 1f, 1f);
    }
    void ChangeMenuDescription(int selectedCount)
    {
        switch (selectedCount)
        {
            case 0:
                menuManager.descriptionText.text = "장비";
                break;
            case 1:
                menuManager.descriptionText.text = "주요 아이템";
                break;
            case 2:
                menuManager.descriptionText.text = "지도";
                break;
            case 3:
                menuManager.descriptionText.text = "설정";
                break;
            case 4:
                menuManager.descriptionText.text = "시작 화면으로 돌아가기";
                break;
            default:
                break;
        }
    }

    #region SelectedMenu Function
    public void SetActiveFalseAllSelectedMenu()
    {
        menuManager.selectedMenuObject.SetActive(false);
        foreach(GameObject obj in menuManager.selectedMenuObjectList)
        {
            obj.SetActive(false);
        }
    }
    public void SetActiveTrueSelectedMenu(int selectedCount)
    {
        menuManager.selectedMenuObjectList[selectedCount].SetActive(true);
    }
    #endregion
}
