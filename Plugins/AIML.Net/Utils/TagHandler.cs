using System.Reflection;

namespace AIMLbot.Utils
{
    /// <summary>
    ///     Encapsulates information about a custom tag class
    /// </summary>
    public class TagHandler
    {
        /// <summary>
        ///     The assembly this class is found in
        /// </summary>
        public string AssemblyName;

        /// <summary>
        ///     The class name for the assembly
        /// </summary>
        public string ClassName;

        /// <summary>
        ///     The name of the tag this class will deal with
        /// </summary>
        public string TagName;

        /// <summary>
        ///     Provides an instantiation of the class represented by this tag-handler
        /// </summary>
        /// <param name="assemblies">All the assemblies the bot knows about</param>
        /// <returns>The instantiated class</returns>
        public AIMLTagHandler Instantiate(Dictionary<string, Assembly> assemblies)
        {
            if (!assemblies.ContainsKey(AssemblyName)) return null;
            var tagDLL = assemblies[AssemblyName];
            var tagDLLTypes = tagDLL.GetTypes();
            return (AIMLTagHandler)tagDLL.CreateInstance(ClassName);
        }
    }
}