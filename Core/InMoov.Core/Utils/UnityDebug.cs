#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
#define UNITY_ENGINE
#endif

#if !UNITY_ENGINE


namespace InMoov.Core.Utils
{
    public static class Debug
    {
        public static void Log(object log)
        {
            Console.WriteLine(log);
        }

        public static void LogWarning(object log)
        {
            Console.WriteLine(log);
        }

        public static void LogError(object log)
        {
            Console.WriteLine(log);
        }
    }
}

#endif