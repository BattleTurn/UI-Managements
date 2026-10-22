using System;

namespace BattleTurn.UIManagements.Runtime
{
    public sealed class UIViewNotExistException : Exception
    {
        public UIViewNotExistException(string viewName, string notExistIn) : base($"{viewName} does not exist in {notExistIn}.")
        {
        }
    }
}