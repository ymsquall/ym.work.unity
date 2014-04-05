#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor
{
    public class EditorBaseWindow<T> : EditorWindow
            where T : EditorWindow, new()
    {
        public static T CreateForm()
        {
            T form = EditorWindowsManager.CreateWindow<T>();
            form.Show();
            return form;
        }
        public static T CreateForm(string title)
        {
            T form = EditorWindowsManager.CreateWindow<T>(title);
            form.Show();
            return form;
        }
        public static T CreateForm(string name, string title)
        {
            T form = EditorWindowsManager.CreateWindow<T>(name, title);
            form.Show();
            return form;
        }
    }
}

#endif