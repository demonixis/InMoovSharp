using AIMLbot.Utils;
using System.Text.RegularExpressions;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
    /// <summary>
    ///     The condition element instructs the AIML interpreter to return specified contents depending
    ///     upon the results of matching a predicate against a pattern.
    ///     NB: The condition element has three different types. The three different types specified
    ///     here are distinguished by an xsi:type attribute, which permits a validating XML Schema
    ///     processor to validate them. Two of the types may contain li elements, of which there are
    ///     three different types, whose validity is determined by the type of enclosing condition. In
    ///     practice, an AIML interpreter may allow the omission of the xsi:type attribute and may instead
    ///     heuristically determine which type of condition (and hence li) is in use.
    ///     Block Condition
    ///     ---------------
    ///     The blockCondition type of condition has a required attribute "name", which specifies an AIML
    ///     predicate, and a required attribute "value", which contains a simple pattern expression.
    ///     If the contents of the value attribute match the value of the predicate specified by name, then
    ///     the AIML interpreter should return the contents of the condition. If not, the empty string ""
    ///     should be returned.
    ///     Single-predicate Condition
    ///     --------------------------
    ///     The singlePredicateCondition type of condition has a required attribute "name", which specifies
    ///     an AIML predicate. This form of condition must contain at least one li element. Zero or more of
    ///     these li elements may be of the valueOnlyListItem type. Zero or one of these li elements may be
    ///     of the defaultListItem type.
    ///     The singlePredicateCondition type of condition is processed as follows:
    ///     Reading each contained li in order:
    ///     1. If the li is a valueOnlyListItem type, then compare the contents of the value attribute of
    ///     the li with the value of the predicate specified by the name attribute of the enclosing
    ///     condition.
    ///     a. If they match, then return the contents of the li and stop processing this condition.
    ///     b. If they do not match, continue processing the condition.
    ///     2. If the li is a defaultListItem type, then return the contents of the li and stop processing
    ///     this condition.
    ///     Multi-predicate Condition
    ///     -------------------------
    ///     The multiPredicateCondition type of condition has no attributes. This form of condition must
    ///     contain at least one li element. Zero or more of these li elements may be of the
    ///     nameValueListItem type. Zero or one of these li elements may be of the defaultListItem type.
    ///     The multiPredicateCondition type of condition is processed as follows:
    ///     Reading each contained li in order:
    ///     1. If the li is a nameValueListItem type, then compare the contents of the value attribute of
    ///     the li with the value of the predicate specified by the name attribute of the li.
    ///     a. If they match, then return the contents of the li and stop processing this condition.
    ///     b. If they do not match, continue processing the condition.
    ///     2. If the li is a defaultListItem type, then return the contents of the li and stop processing
    ///     this condition.
    ///     ****************
    ///     Condition List Items
    ///     As described above, two types of condition may contain li elements. There are three types of
    ///     li elements. The type of li element allowed in a given condition depends upon the type of that
    ///     condition, as described above.
    ///     Default List Items
    ///     ------------------
    ///     An li element of the type defaultListItem has no attributes. It may contain any AIML template
    ///     elements.
    ///     Value-only List Items
    ///     ---------------------
    ///     An li element of the type valueOnlyListItem has a required attribute value, which must contain
    ///     a simple pattern expression. The element may contain any AIML template elements.
    ///     Name and Value List Items
    ///     -------------------------
    ///     An li element of the type nameValueListItem has a required attribute name, which specifies an
    ///     AIML predicate, and a required attribute value, which contains a simple pattern expression. The
    ///     element may contain any AIML template elements.
    /// </summary>
    public class Condition : AIMLTagHandler
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
        public Condition(AIMLbot.Bot bot,
            User user,
            SubQuery query,
            Request request,
            Result result,
            XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
            isRecursive = false;
        }

        protected override string ProcessChange()
        {
            if (templateNode.Name.ToLower() == "condition")
            {
                // heuristically work out the type of condition being processed

                if (templateNode.Attributes.Count == 2) // block
                {
                    var name = "";
                    var value = "";

                    if (templateNode.Attributes[0].Name == "name")
                        name = templateNode.Attributes[0].Value;
                    else if (templateNode.Attributes[0].Name == "value") value = templateNode.Attributes[0].Value;

                    if (templateNode.Attributes[1].Name == "name")
                        name = templateNode.Attributes[1].Value;
                    else if (templateNode.Attributes[1].Name == "value") value = templateNode.Attributes[1].Value;

                    if ((name.Length > 0) & (value.Length > 0))
                    {
                        var actualValue = user.Predicates.GrabSetting(name);
                        var matcher = new Regex(value.Replace(" ", "\\s").Replace("*", "[\\sA-Z0-9]+"),
                            RegexOptions.IgnoreCase);
                        if (matcher.IsMatch(actualValue)) return templateNode.InnerXml;
                    }
                }
                else if (templateNode.Attributes.Count == 1) // single predicate
                {
                    if (templateNode.Attributes[0].Name == "name")
                    {
                        var name = templateNode.Attributes[0].Value;
                        foreach (XmlNode childLiNode in templateNode.ChildNodes)
                            if (childLiNode.Name.ToLower() == "li")
                            {
                                if (childLiNode.Attributes.Count == 1)
                                {
                                    if (childLiNode.Attributes[0].Name.ToLower() == "value")
                                    {
                                        var actualValue = user.Predicates.GrabSetting(name);
                                        var matcher =
                                            new Regex(
                                                childLiNode.Attributes[0].Value.Replace(" ", "\\s")
                                                    .Replace("*", "[\\sA-Z0-9]+"), RegexOptions.IgnoreCase);
                                        if (matcher.IsMatch(actualValue)) return childLiNode.InnerXml;
                                    }
                                }
                                else if (childLiNode.Attributes.Count == 0)
                                {
                                    return childLiNode.InnerXml;
                                }
                            }
                    }
                }
                else if (templateNode.Attributes.Count == 0) // multi-predicate
                {
                    foreach (XmlNode childLiNode in templateNode.ChildNodes)
                        if (childLiNode.Name.ToLower() == "li")
                        {
                            if (childLiNode.Attributes.Count == 2)
                            {
                                var name = "";
                                var value = "";
                                if (childLiNode.Attributes[0].Name == "name")
                                    name = childLiNode.Attributes[0].Value;
                                else if (childLiNode.Attributes[0].Name == "value")
                                    value = childLiNode.Attributes[0].Value;

                                if (childLiNode.Attributes[1].Name == "name")
                                    name = childLiNode.Attributes[1].Value;
                                else if (childLiNode.Attributes[1].Name == "value")
                                    value = childLiNode.Attributes[1].Value;

                                if ((name.Length > 0) & (value.Length > 0))
                                {
                                    var actualValue = user.Predicates.GrabSetting(name);
                                    var matcher = new Regex(value.Replace(" ", "\\s").Replace("*", "[\\sA-Z0-9]+"),
                                        RegexOptions.IgnoreCase);
                                    if (matcher.IsMatch(actualValue)) return childLiNode.InnerXml;
                                }
                            }
                            else if (childLiNode.Attributes.Count == 0)
                            {
                                return childLiNode.InnerXml;
                            }
                        }
                }
            }

            return string.Empty;
        }
    }
}