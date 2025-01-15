using System.Reflection;
using UnityEngine;

namespace OMEGA
{
    public static class Log
    {
        public static void Info(string what = "")
        {
            Write(what, "[INFO]");
        }

        public static void Warn(string msg = "")
        {
            Write(msg, "[WARN]");
        }

        public static void Error(string msg = "")
        {
            Write(msg, "[ERROR]");
        }

        static void Write(string what, string category)
        {
            if (what.Length == 0)
            {
                Debug.Log("");
                return;
            }

            var trace = new System.Diagnostics.StackTrace();
            MethodBase method = null;

            try
            {
                for (byte i = 1; i < 255 && (method == null || method.ReflectedType.Name.Length == 0 || method.ReflectedType.Name.StartsWith("<>") || method.ReflectedType.Name.Equals("Log")); ++i)
                {
                    method = trace.GetFrame(i).GetMethod();
                }
            }
            catch { }

            Debug.Log(string.Format("[{0}.{1}] {2} {3}", method != null ? method.ReflectedType.Name : "Unknown", method != null ? method.Name : "Unknown", category, what));
        }
    }
}