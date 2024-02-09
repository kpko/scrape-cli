# scrape-cli
A small .NET cli to scrape web contents using AngleSharp

## Build
The project is AOT-enabled to enable a fast startup time. Just build the project as release and let AOT do its thing:

```sh
dotnet publish -c Release
```

I'd recommend to alias the cli to something meaningful while we don't have an integrated build in this repository.

```sh
alias scrape="$(pwd)/scrape-cli/bin/Release/net8.0/osx-arm64/publish/scrape-cli"
```

## Usage
```
Options:
  -q, --query <query>                                              A css query selector to use against the given html input
  -s, --select <Attribute|Class|Id|InnerHtml|InnerText|OuterHtml>  Specifies what the query selector selects [default: OuterHtml]
  -sa, --select-attr <select-attr>                                 Selects an attribute of the result object
  -p, --pretty                                                     Pretty prints the html output
  -t, --trim                                                       Trim html output
  --version                                                        Show version information
  -?, -h, --help                                                   Show help and usage information
```

## Examples
Scraping the commit message from whatthecommit.com:

```sh
curl -sL whatthecommit.com | scrape -q '#content > p:first-child' -s outerhtml
```

Scrape a news article and generate a pdf out of it using wkhtmltopdf:

```sh
curl -sL https://www.nytimes.com/2024/02/09/opinion/eat-just-upside-foods-cultivated-meat.html | scrape -q 'article#story' | wkhtmltopdf --encoding utf8 - test.pdf

# another one:
curl -sL https://www.presseportal.de/pm/43315/5710973 | scrape -q 'article.story p' | wkhtmltopdf --encoding utf8 - test.pdf
```

