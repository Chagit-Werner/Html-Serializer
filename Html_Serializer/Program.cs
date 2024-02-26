using Html_Serializer;
using System.Text.RegularExpressions;

string site = "https://codetime.co.il/category/dotnet/";
var html = await Load(site);
static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
html = new Regex("[\\r\\n\\t]").Replace(new Regex("\\s{2,}").Replace(html, ""), "");
//הפונקציה Split תדע לפצל את ה-html לרשימה 
var htmlLines = new Regex("<(.*?)>").Split(html).Where(s => s.Length > 0).ToList();
HtmlElement root = BuildElement(htmlLines[1].Split(' ')[0], null, htmlLines[1]);
BuildTree(htmlLines.Skip(2).ToList(), root);


PrintTree(root, "");
Console.ReadLine();


static HtmlElement BuildTree(List<string> htmlStrings, HtmlElement root)
{
    HtmlElement currentElement = root;

    foreach (string htmlString in htmlStrings)
    {
        if (htmlString.StartsWith("/html"))
            //We have finished with the html page!! let's break out..
            break;

        if (htmlString.StartsWith("/"))
        {
            currentElement = currentElement.Parent;
            continue;
        }
        //מחלק את המילה לפי הרווחים.
       //מכניס ל-firstWord את המילה הראשונה אחרי החלוקה.
        string firstWord = htmlString.Split(' ')[0];
       //אם זה לא כמו  תגית H1 אלא כמו IMG
       //אז זה סימן שאין לתגית הזאת ילדים.
        if (!HtmlHelper.Instance.AllTags.Contains(firstWord))
        {
            currentElement.InnerHtml += htmlString;
            continue;
        }
        HtmlElement newElement = BuildElement(firstWord, currentElement, htmlString);
        currentElement.Children.Add(newElement);
            //אם זה תגית כמו H1
            //כלומר: יש סיכוי שיהיה לתגית  הזאת ילדים:
            if (!HtmlHelper.Instance.NoClosingTags.Contains(firstWord) && !htmlString.EndsWith("/"))        
                currentElement = newElement;
    }
    return root;
}
static HtmlElement BuildElement(string tagName, HtmlElement currentParent, string line)
{
    HtmlElement child = new HtmlElement { Name = tagName, Parent = currentParent };
    //ביטוי רגולרי שמחפש לפי '='
    var attributeMatches = Regex.Matches(line, "([^\\s]*?)=\"(.*?)\"");
    foreach (Match match in attributeMatches)
    {
        string attributeName = match.Groups[1].Value;
        string attributeValue = match.Groups[2].Value;

        if (attributeName.ToLower() == "class")
            child.Classes.AddRange(attributeValue.Split(' '));
        else if (attributeName.ToLower() == "id")
            child.Id = attributeValue;
        else
            child.Attributes.Add(attributeName, attributeValue);
    }

    return child;
}

static void PrintTree(HtmlElement element, string indentation)
{
    Console.WriteLine($"{indentation}{element}");
    foreach (var child in element.Children)
        PrintTree(child, indentation + "  ");
}


    