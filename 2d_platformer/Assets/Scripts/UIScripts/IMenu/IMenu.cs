using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenu
{
    void ShowMenu();
    void SetMenuDisplay();
    void CheckEscapeKey();
    void CheckArrowKey();
    void CheckConfirmKey();
    void CheckCancleKey();
}
