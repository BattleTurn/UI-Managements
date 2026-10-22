using System;
using System.Collections.Generic;
namespace BattleTurn.UIManagements.Runtime
{
    [Serializable]
    internal sealed class ViewPool<T> where T : UIViewer
    {
        public Dictionary<int, T> views = new();

        public int Count => views.Count;

        public T Get(int key)
        {
            T view;
            if (!views.TryGetValue(key, out view))
            {
                return null;
            }

            views.Remove(key);
            return view;
        }

        public T GetFirst()
        {
            T view;
            foreach (KeyValuePair<int, T> value in views)
            {
                if (value.Value)
                {
                    view = value.Value;
                    views.Remove(value.Key);
                    return view;
                }
            }

            return null;
        }

        public void Push(int key, T view)
        {
            if (views.ContainsKey(key))
            {
                return;
            }

            views.Add(key, view);
        }

        public bool Has(int key)
        {
            return views.ContainsKey(key);
        }

        public bool TryGet(int key, out T view)
        {
            if (views.ContainsKey(key))
            {
                view = views[key];
                return true;
            }

            view = null;
            return false;
        }
    }
}