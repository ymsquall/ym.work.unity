using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assets.Script.Tools
{
    public class FileUtils
    {
        static bool EnumFilesByPath(string path, bool subdir, ref List<string> files)
        {
            string[] fns = System.IO.Directory.GetFiles(path);
            foreach (string f in fns)
            {
                if (f.Substring(f.Length - 5) == ".meta")
                    continue;
                files.Add(f);
            }
            if (subdir)
            {
                string[] paths = System.IO.Directory.GetDirectories(path);
                foreach (string p in paths)
                {
                    if (!EnumFilesByPath(p, subdir, ref files))
                        return false;
                }
            }
            return true;
        }
        public static string[] EnumAllFilesByPath(string path, bool subdir)
        {
            List<string> files = new List<string>(0);
            string[] fns = System.IO.Directory.GetFiles(path);
            foreach(string f in fns)
            {
                if (f.Substring(f.Length - 5) == ".meta")
                    continue;
                files.Add(f);
            }
            if(subdir)
            {
                string[] paths = System.IO.Directory.GetDirectories(path);
                foreach (string p in paths)
                {
                    if (!EnumFilesByPath(p, subdir, ref files))
                        return null;
                }
            }
            return files.ToArray();
        }
    }
}
