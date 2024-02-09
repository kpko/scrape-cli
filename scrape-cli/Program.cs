using System.CommandLine;

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

using scrape_cli;

var querySelectorOption = new Option<string?>(aliases: ["-q", "--query"],
    description: "A css query selector to use against the given html input");
var selectOption = new Option<SelectionMode?>(aliases: ["-s", "--select"],
    description: "Specifies what the query selector selects", getDefaultValue: () => SelectionMode.OuterHtml);
var selectAttrOption = new Option<string?>(aliases: ["-sa", "--select-attr"],
    description: "Selects an attribute of the result object");
var prettyOption = new Option<bool>(aliases: ["-p", "--pretty"],
    description: "Pretty prints the html output");
var trimOption = new Option<bool>(aliases: ["-t", "--trim"],
    description: "Trim html output");

var rootCommand = new RootCommand("Simple html parser, selector and scraper");
rootCommand.AddOption(querySelectorOption);
rootCommand.AddOption(selectOption);
rootCommand.AddOption(selectAttrOption);
rootCommand.AddOption(prettyOption);
rootCommand.AddOption(trimOption);

rootCommand.SetHandler(async (querySelector, select, selectAttr, pretty, trim) =>
    {
        var parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true, SkipPlaintext = false, SkipRawText = false, SkipComments = false });

        var doc = await parser.ParseDocumentAsync(Console.OpenStandardInput());

        var elements = new List<IElement>();
        if (querySelector != null)
        {
            elements.AddRange(doc.QuerySelectorAll(querySelector));
        }
        else
        {
            elements.Add(doc.DocumentElement);
        }

        foreach (var element in elements)
        {
            var output = (select, selectAttr) switch
            {
                (SelectionMode.OuterHtml, null) => pretty ? element.Prettify() : element.OuterHtml,
                (SelectionMode.InnerHtml, null) => pretty ? string.Join(", ", element.Children.Select(c => c.Prettify())) : element.InnerHtml,
                (SelectionMode.InnerText, null) => element.TextContent,
                (SelectionMode.Id, null)        => element.Id,
                (SelectionMode.Class, null)     => element.ClassName,
                (_, string attrName)            => element.GetAttribute(attrName),
                _                               => throw new ArgumentOutOfRangeException()
            };

            Console.WriteLine(trim ? output?.Trim() : output);
        }
    },
    querySelectorOption, selectOption, selectAttrOption, prettyOption, trimOption);

return await rootCommand.InvokeAsync(args);