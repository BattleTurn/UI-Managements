namespace BattleTurn.UIManagements.Runtime
{
    public abstract class ScreenBase : UIViewer
    {
        protected override void HideSelf()
        {
            uiManager.HideScreen(GetType());
        }

        protected override void ShowSelf()
        {
            uiManager.ShowScreen(GetType());
        }
    }
}