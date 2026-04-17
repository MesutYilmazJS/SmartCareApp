using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SmartCareApp.TagHelpers;

[HtmlTargetElement("risk-badge")]
public class RiskBadgeTagHelper : TagHelper
{
    [HtmlAttributeName("score")]
    public int Score { get; set; }

    [HtmlAttributeName("label")]
    public string? Label { get; set; }

    [HtmlAttributeName("show-score")]
    public bool ShowScore { get; set; } = true;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", $"risk-chip {GetToneClass(Score)}");

        var text = Label ?? GetLabel(Score);
        output.Content.SetContent(ShowScore ? $"{text} {Score}" : text);
    }

    private static string GetLabel(int score) =>
        score switch
        {
            >= 80 => "Kritik",
            >= 65 => "Öncelikli",
            >= 45 => "İzlem",
            _ => "Rutin"
        };

    private static string GetToneClass(int score) =>
        score switch
        {
            >= 80 => "risk-chip-critical",
            >= 65 => "risk-chip-priority",
            >= 45 => "risk-chip-monitor",
            _ => "risk-chip-routine"
        };
}
