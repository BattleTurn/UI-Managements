using System;

namespace BattleTurn.UIManagements.Runtime
{
    internal sealed class UIViewNotShowingException : Exception
    {
        public UIViewNotShowingException(string viewName, string collectionName)
            : base($"UIView of type '{viewName}' is not showing in '{collectionName}'.")
        {
        }
    }
}