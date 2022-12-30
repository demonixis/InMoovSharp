#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
#define UNITY_ENGINE
#endif

#if !UNITY_ENGINE
using System.Collections;

namespace UnityEngine
{
    public class MonoBehaviour
    {
        public bool enabled;
        public string name;

        public T GetComponent<T>() where T : MonoBehaviour
        {
            return null;
        }

        public static T FindObjectOfType<T>(bool includeInactive = false) where T : MonoBehaviour
        {
            return null;
        }

        public static T[] FindObjectsOfType<T>(bool includeInactive = false) where T : MonoBehaviour
        {
            return null;
        }

        public T[] GetComponentsInChildren<T>(bool includeInactive = false) where T : MonoBehaviour
        {
            return null;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {

        }

        public void StopAllCoroutines()
        {

        }
    }
}

#endif