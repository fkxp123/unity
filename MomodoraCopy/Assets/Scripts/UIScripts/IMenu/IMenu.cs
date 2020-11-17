namespace MomodoraCopy
{
    public interface IMenu
    {
        void ShowMenu();
        void SetMenuDisplay();
        void CheckEscapeKey();
        void CheckArrowKey();
        void CheckConfirmKey();
        void CheckCancleKey();
    }

}