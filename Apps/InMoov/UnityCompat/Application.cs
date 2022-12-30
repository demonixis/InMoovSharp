#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
#define UNITY_ENGINE
#endif

#if !UNITY_ENGINE

namespace UnityEngine
{
    public enum RuntimePlatform
    {
        Android,
        LinuxEditor,
        LinuxPlayer,
        WindowsEditor,
        WindowsPlayer,
        OSXEditor,
        OSXPlayer
    }

    public static class Application
    {
        public static RuntimePlatform platform
        {
            get
            {
                return RuntimePlatform.WindowsPlayer;
            }
        }

        public static string companyName;
        public static string appVersion;
        public static string productName;
        public static string identifier;
        public static string persistentDataPath;
        public static string streamingAssetsPath { get; set; }
        public static float targetFrameRate { get; set; } = 30;
    }
}

#endif