using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace LR6.Dialogues
{
    public sealed class DialogueXmlLoader
    {
        public DialogueData LoadFromText(string xml)
        {
            var dialogue = new DialogueData();
            var document = XDocument.Parse(xml);
            var root = document.Element("messages");

            if (root == null)
            {
                throw new InvalidDataException("Root node <messages> was not found.");
            }

            dialogue.SetCurrentUid(ParseLong(root.Attribute("uid")?.Value));

            foreach (var messageNode in root.Elements("message"))
            {
                var message = new DialogueMessage
                {
                    MessageId = ParseLong(messageNode.Attribute("mid")?.Value),
                    Text = GetDirectText(messageNode),
                };

                var answersNode = messageNode.Element("answers");
                if (answersNode != null)
                {
                    foreach (var answerNode in answersNode.Elements("answer"))
                    {
                        message.Answers.Add(new DialogueAnswer
                        {
                            AnswerId = ParseLong(answerNode.Attribute("auid")?.Value),
                            LinkedMessageId = ParseLong(answerNode.Attribute("muid")?.Value),
                            Action = CanonicalizeActionString(answerNode.Attribute("action")?.Value),
                            Text = GetDirectText(answerNode),
                        });
                    }
                }

                dialogue.LoadMessage(message);
            }

            var firstMessage = dialogue.Messages.FirstOrDefault();
            if (firstMessage != null)
            {
                dialogue.SelectMessage(firstMessage.MessageId);
            }

            return dialogue;
        }

        public DialogueData LoadFromTextAsset(TextAsset textAsset)
        {
            return LoadFromText(textAsset.text);
        }

        private static string GetDirectText(XElement element)
        {
            return string.Concat(element.Nodes().OfType<XText>().Select(node => node.Value)).Trim();
        }

        private static long ParseLong(string? rawValue)
        {
            if (long.TryParse(rawValue, out var value))
            {
                return value;
            }

            return -1;
        }

        private static string CanonicalizeActionString(string? action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return "none";
            }

            var parts = action.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
            var normalizedParts = new System.Collections.Generic.List<string>();

            foreach (var rawPart in parts)
            {
                var token = rawPart.Trim();
                if (token.Length == 0 || string.Equals(token, "none", System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(token, "open_door", System.StringComparison.OrdinalIgnoreCase))
                {
                    token = "door open";
                }
                else if (string.Equals(token, "end_dialog", System.StringComparison.OrdinalIgnoreCase))
                {
                    token = "dialogue end";
                }

                normalizedParts.Add(token);
            }

            return normalizedParts.Count == 0 ? "none" : string.Join("; ", normalizedParts);
        }
    }
}
