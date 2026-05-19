using System.IO;
using System.Linq;
using System.Xml.Linq;
using DialogEditor.Avalonia.Models;

namespace DialogEditor.Avalonia.Services;

public sealed class DialogXmlSerializer
{
    public void Save(DialogGraph graph, Stream output)
    {
        var root = new XElement("messages", new XAttribute("uid", graph.CurrentUid));

        foreach (var message in graph.Messages)
        {
            var messageNode = new XElement("message", new XAttribute("mid", message.MessageId));
            messageNode.Add(new XText(message.Text));

            var answersNode = new XElement("answers");
            foreach (var answer in message.Answers)
            {
                var answerNode = new XElement(
                    "answer",
                    new XAttribute("auid", answer.AnswerId),
                    new XAttribute("muid", answer.LinkedMessageId),
                    new XAttribute("action", answer.Action));

                answerNode.Add(new XText(answer.Text));
                answersNode.Add(answerNode);
            }

            messageNode.Add(answersNode);
            root.Add(messageNode);
        }

        var document = new XDocument(root);
        document.Save(output);
    }

    public DialogGraph Load(Stream input)
    {
        var document = XDocument.Load(input);
        var root = document.Element("messages") ?? throw new InvalidDataException("Корневой узел <messages> не найден.");

        var graph = new DialogGraph();
        graph.SetCurrentUid(ParseLong(root.Attribute("uid")?.Value, "uid"));

        foreach (var messageNode in root.Elements("message"))
        {
            var message = new DialogMessage
            {
                MessageId = ParseLong(messageNode.Attribute("mid")?.Value, "mid"),
                Text = GetElementText(messageNode),
            };

            var answersNode = messageNode.Element("answers");
            if (answersNode is not null)
            {
                foreach (var answerNode in answersNode.Elements("answer"))
                {
                    var answer = new DialogAnswer
                    {
                        AnswerId = ParseLong(answerNode.Attribute("auid")?.Value, "auid"),
                        LinkedMessageId = ParseLong(answerNode.Attribute("muid")?.Value, "muid"),
                        Action = answerNode.Attribute("action")?.Value ?? DialogActions.None,
                        Text = GetElementText(answerNode),
                    };

                    message.Answers.Add(answer);
                }
            }

            graph.Messages.Add(message);
        }

        var maxExistingId = graph.Messages
            .SelectMany(message => message.Answers.Select(answer => answer.AnswerId).Append(message.MessageId))
            .DefaultIfEmpty(0)
            .Max();

        if (graph.CurrentUid < maxExistingId)
        {
            graph.SetCurrentUid(maxExistingId);
        }

        return graph;
    }

    private static long ParseLong(string? value, string attributeName)
    {
        if (long.TryParse(value, out var result))
        {
            return result;
        }

        throw new InvalidDataException($"Атрибут '{attributeName}' отсутствует или имеет неверный формат.");
    }

    private static string GetElementText(XElement element)
    {
        return string.Concat(element.Nodes().OfType<XText>().Select(textNode => textNode.Value)).Trim();
    }
}
