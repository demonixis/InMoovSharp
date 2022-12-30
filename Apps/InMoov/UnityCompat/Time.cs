#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
#define UNITY_ENGINE
#endif

#if !UNITY_ENGINE
using System.Collections;

namespace UnityEngine
{ 
    public static class Time
    {
        public static float realtimeSinceStartup;
        public static float timeScale = 1;
    }
}

#endif
