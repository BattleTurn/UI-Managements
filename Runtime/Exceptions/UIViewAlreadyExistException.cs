using System;

namespace BattleTurn.UIManagements.Runtime
{
    public sealed class UIViewAlreadyExistException : Exception
    {
        public UIViewAlreadyExistException(string viewName, string existIn) : base($"UIView with name {viewName} already exist in {existIn}.")
        {
        }
    }
}