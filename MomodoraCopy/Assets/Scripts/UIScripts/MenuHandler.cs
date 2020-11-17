namespace MomodoraCopy
{
    public class MenuHandler
    {
        public IMenu CurrentMenu { get; private set; }//read only
        public MenuHandler(IMenu menuType)
        {
            CurrentMenu = menuType;
        }
        public void ChangeMenuType(IMenu menuType)
        {
            CurrentMenu = menuType;
        }
        public void ExitMenu()
        {
            CurrentMenu = MenuManager.instance.mainMenu;
        }
    }

}