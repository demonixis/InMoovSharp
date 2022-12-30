#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
#define UNITY_ENGINE
#endif

#if !UNITY_ENGINE
using System.Collections;

namespace UnityEngine
{
    public class SerializeField : Attribute
    {
    }

    public class Header : Attribute
    {
        public Header(string name) : base()
        {
        }
    }

    public class RequireComponent : Attribute
    {
        public RequireComponent(Type type) : base()
        {
        }
    }
}

#endif