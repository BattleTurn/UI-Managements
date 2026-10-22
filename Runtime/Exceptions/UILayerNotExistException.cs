using System;

namespace BattleTurn.UIManagements.Runtime
{
    internal sealed class UILayerNotExistException : Exception
    {
        public UILayerNotExistException(string layerName, string existIn) : base($"UILayer with name <color=red>{layerName}</color> does NOT exist in {existIn}.")
        {
        }
    }
}