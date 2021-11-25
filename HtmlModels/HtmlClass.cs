using System.Text.RegularExpressions;

namespace HtmlModels;

public class HtmlClass
{
  public HtmlClass(Stream stream)
  {

  }

  public HtmlClass(string htmlString)
  {

  }

  public static void ParseValues(string htmlString, (string, string)[] taglist)
  {
    var (tag, className) = taglist[0];

    var regex = new Regex(HtmlTagExpression(tag, className));

    var match = regex.Match(htmlString);
    foreach (var item in match.Groups)
    {
    }


  }

  private static string HtmlTagExpression(string tag, string className)
    => $"<{tag}(.*?)class=\"{className}\"(.*?)>(.*?)</{tag}>";
}
