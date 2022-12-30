#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
#define UNITY_ENGINE
#endif

#if !UNITY_ENGINE

namespace UnityEngine
{
    public static class Random
    {
        private static readonly System.Random Rnd = new System.Random();

        public static float Range(float min, float max)
        {
            return min;
        }
    }
}

#endif