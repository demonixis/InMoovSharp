using AIMLbot.Utils;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
    /// <summary>
    ///     NOT IMPLEMENTED FOR SECURITY REASONS
    /// </summary>
    public class Javascript : AIMLTagHandler
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public Javascript(AIMLbot.Bot bot,
            User user,
            SubQuery query,
            Request request,
            Result result,
            XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }

        protected override string ProcessChange()
        {
            bot.WriteToLog("The javascript tag is not implemented in this bot");
            return string.Empty;
        }
    }
}