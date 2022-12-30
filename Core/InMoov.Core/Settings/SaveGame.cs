#define USE_MYGAMES_FOLDER

using InMoov.Core;
using InMoov.Core.Utils;
using Newtonsoft.Json;

namespace Demonixis.InMoov.Settings
{
    public enum GameSaveStorageMode
    {
        External = 0,
        Internal,
        Auto
    }

    [Serializable]
    public abstract class SaveGame
    {
        public static string GetAndroidPath(string? additionalPath = null)
        {
            var path = $"/storage/emulated/0/Android/data/{Application.identifier}/files";

            if (!string.IsNullOrEmpty(additionalPath))
                path = $"{path}/{additionalPath}";

            return path;
        }

        public static string GetStandalonePath(string? additionalPath = null)
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(root, Application.companyName, Application.productName);

#if USE_MYGAMES_FOLDER
            path = Path.Combine(root, "My Games", Application.productName);
#endif

            if (!string.IsNullOrEmpty(additionalPath))
                path = Path.Combine(path, additionalPath);

            return path;
        }

        private static string GetFallbackPath(string? additionalPath = null)
        {
            return Application.persistentDataPath;
        }

        public static string GetSavePath(string? additionalPath = null)
        {
            var path = GetFallbackPath(additionalPath);

            if (Application.isMobile)
                path = GetAndroidPath(additionalPath);
            else
                path = GetStandalonePath(additionalPath);

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return path;
        }

        public static string SaveRawData(object data, string filename, string? additionalPath = null)
        {
            var json = data is string ? (string)data : JsonConvert.SerializeObject(data, Formatting.Indented);

            var savePath = Path.Combine(GetSavePath(additionalPath), filename);
            File.WriteAllText(savePath, json);

            return json;
        }

        public static T? LoadRawData<T>(string filename, string? additionalPath = null)
        {
            var json = string.Empty;
            var filepath = Path.Combine(GetSavePath(additionalPath), filename);

            if (File.Exists(filepath))
                json = File.ReadAllText(filepath);

            if (!string.IsNullOrEmpty(json) && json != "{}")
                return JsonConvert.DeserializeObject<T>(json);

            return default;
        }

        public static void ClearData(string filename)
        {
            var filepath = Path.Combine(GetSavePath(), filename);

            if (File.Exists(filepath))
                File.Delete(filepath);
        }
    }
}