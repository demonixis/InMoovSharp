#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
#define UNITY_ENGINE
#endif

#if !UNITY_ENGINE

namespace UnityEngine
{
    public static class PlayerPrefs
    {
        public static bool HasKey(string key)
        {
            return false;
        }

        public static void DeleteKey(string key)
        {
        }

        public static string GetString(string key, string defaultValue)
        {
            return defaultValue;
        }

        public static void SetString(string key, string value)
        {
        }

        public static void Save()
        {
        }
    }
}

#endif