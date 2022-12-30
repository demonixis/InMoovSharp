#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
#define UNITY_ENGINE
#endif

#if !UNITY_ENGINE
using System.Collections;

namespace UnityEngine
{
    public class UnityException : Exception
    {
        public UnityException(string s) : base(s) { }
    }
}

#endif