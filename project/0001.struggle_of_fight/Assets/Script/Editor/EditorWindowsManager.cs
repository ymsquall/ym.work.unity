#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor
{
    class EditorWindowsManager
    {
        public static T CreateWindow<T>()
            where T : EditorWindow, new()
        {
            string name = string.Format("AutoEditor({0})", ++mAutoNamed);
            return CreateWindow<T>(name, name);
        }
        public static T CreateWindow<T>(string name)
            where T : EditorWindow, new()
        {
            return CreateWindow<T>(name, name);
        }
        public static T CreateWindow<T>(string name, string title)
            where T : EditorWindow, new()
        {
            T form = null;
            if (mWindowList.ContainsKey(name))
            {
                form = mWindowList[name] as T;
                form.title = title;
                form.Focus();
                form.Show();
            }
            else
            {
                Debug.Log(string.Format("CreateWindow<{0}>({1}, {2})", typeof(T).ToString(), name, title));
                form = EditorWindow.GetWindow<T>(title, true);
                //form = EditorWindow.CreateInstance<T>();
                form.title = title;
                form.name = name;
                form.Show();
                mWindowList[name] = form;
            }
            return form;
        }

        public static T GetWindow<T>(string name)
            where T : EditorWindow
        {
            T form = null;
            if (mWindowList.ContainsKey(name))
                form = mWindowList[name] as T;
            return form;
        }

        public static EditorWindow ClosedWindow(string name)
        {
            EditorWindow wnd = null;
            if (mWindowList.ContainsKey(name))
            {
                wnd = mWindowList[name];
                mWindowList.Remove(name);
            }
            return wnd;
        }

        static Dictionary<string, EditorWindow> mWindowList = new Dictionary<string,EditorWindow>(0);
        static int mAutoNamed = 0;
    }
}

#endif