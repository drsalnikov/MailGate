using System.Net;
using MailGate.Core.Models;
using System.Text;
using System.Text.RegularExpressions;
using MailGate.Contracts.Templates;

namespace MailGate.Api.Templates;

public static class EmailTemplateRenderer
{
    private static readonly HashSet<string> BlockTypes = ["heading", "text", "content", "button", "divider", "columns", "image"];
    private static readonly Regex Color = new("^#[0-9a-fA-F]{6}$", RegexOptions.NonBacktracking);

    public static Dictionary<string, string[]> Validate(EmailTemplateDefinition? definition)
    {
        var errors = new Dictionary<string, string[]>();
        void Error(string field, string message) => errors[field] = [message];
        if (definition is null) { Error("Definition", "Укажите шаблон."); return errors; }
        if (string.IsNullOrWhiteSpace(definition.Name) || definition.Name.Length > 200)
            Error("Name", "Название должно содержать от 1 до 200 символов.");
        if (definition.Subject is null || definition.Subject.Length > 500 || definition.Subject.Contains('\r') || definition.Subject.Contains('\n'))
            Error("Subject", "Тема: не более 500 символов, без перевода строки.");
        if (definition.Preheader is null || definition.Preheader.Length > 500 || definition.Footer is null || definition.Footer.Length > 4000)
            Error("Text", "Прехедер: до 500 символов, подпись: до 4000.");
        if (definition.AccentColor is null || !Color.IsMatch(definition.AccentColor) ||
            definition.BackgroundColor is null || !Color.IsMatch(definition.BackgroundColor))
            Error("Colors", "Цвет должен иметь формат #RRGGBB.");
        if (definition.Blocks is null || definition.Blocks.Count is < 1 or > 40)
        { Error("Blocks", "Добавьте от 1 до 40 блоков."); return errors; }
        if (definition.Blocks.Count(b => b?.Type == "content") != 1)
            Error("Blocks", "Шаблон должен содержать ровно один блок основного сообщения.");
        var totalImageBytes = 0;
        for (var i = 0; i < definition.Blocks.Count; i++)
        {
            var block = definition.Blocks[i];
            if (block is null || !BlockTypes.Contains(block.Type ?? ""))
            { Error($"Blocks[{i}]", "Неизвестный тип блока."); continue; }
            if (block.Text is null || block.Text.Length > 10000 || block.SecondaryText is null || block.SecondaryText.Length > 10000 || block.Url is null || (block.Type != "image" && block.Url.Length > 2000))
            { Error($"Blocks[{i}]", "Слишком длинное или отсутствующее содержимое блока."); continue; }
            if (block.Type == "button" && (string.IsNullOrWhiteSpace(block.Text) || !IsHttpUrl(block.Url)))
                Error($"Blocks[{i}]", "Для кнопки нужны текст и абсолютная ссылка http:// или https://.");
            if (block.Type == "image")
            {
                if (EmbeddedEmailImage.TryRead(block.Url, out _, out var bytes))
                    totalImageBytes += bytes.Length;
                else if (block.Url.Length > 2000 || !IsHttpUrl(block.Url) || !block.Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    Error($"Blocks[{i}]", "Загрузите PNG, JPEG или GIF до 128 КБ.");
            }
        }
        if (totalImageBytes > EmbeddedEmailImage.MaxTotalBytes)
            Error("Blocks", "Общий размер изображений: не более 512 КБ.");
        return errors;
    }

    public static bool IsHttpUrl(string value) => Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp) && string.IsNullOrEmpty(uri.UserInfo);

    public static string Encode(string? value) => WebUtility.HtmlEncode(value ?? "");
    private static string Lines(string? value) => Encode(value).Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", "<br />");

    public static EmailPreviewDto Render(EmailTemplateDefinition definition, string body, string? subject = null)
    {
        if (Validate(definition).Count != 0) throw new ArgumentException("Некорректный шаблон.");
        var title = string.IsNullOrWhiteSpace(subject) ? definition.Subject : subject.Trim();
        var html = new StringBuilder();
        html.Append("<!doctype html><html lang=\"ru\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>")
            .Append(Encode(title)).Append("</title><style>@media only screen and (max-width:620px){.email-column{display:block!important;width:100%!important}.email-padding{padding:20px!important}}</style></head>")
            .Append($"<body style=\"margin:0;padding:0;background-color:{definition.BackgroundColor}\">")
            .Append($"<div style=\"display:none;max-height:0;overflow:hidden;opacity:0\">{Encode(definition.Preheader)}</div>")
            .Append($"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"{definition.BackgroundColor}\"><tr><td align=\"center\" style=\"padding:24px 8px\">")
            .Append("<!--[if mso]><table role=\"presentation\" width=\"600\"><tr><td><![endif]-->")
            .Append($"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" style=\"max-width:600px;border-top:4px solid {definition.AccentColor}\"><tr><td class=\"email-padding\" style=\"padding:32px;font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:1.6;color:#202631;word-break:break-word\">");
        foreach (var block in definition.Blocks)
        {
            html.Append(block.Type switch
            {
                "image" => $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"margin-bottom:20px\"><tr><td align=\"center\"><img src=\"{Encode(block.Url)}\" alt=\"{Encode(block.Text)}\" width=\"536\" style=\"display:block;width:100%;max-width:536px;height:auto;border:0\" /></td></tr></table>",
                "heading" => $"<h1 style=\"margin:0 0 20px;font-size:26px;line-height:1.3;color:{definition.AccentColor}\">{Lines(block.Text)}</h1>",
                "text" => $"<p style=\"margin:0 0 20px\">{Lines(block.Text)}</p>",
                "content" => $"<div style=\"margin:0 0 20px\">{Lines(body)}</div>",
                "divider" => "<table role=\"presentation\" width=\"100%\"><tr><td style=\"border-top:1px solid #dfe3e8;height:20px\"></td></tr></table>",
                "button" => $"<table role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" style=\"margin-bottom:20px\"><tr><td bgcolor=\"{definition.AccentColor}\" style=\"padding:12px 22px;border-radius:4px\"><a href=\"{Encode(block.Url)}\" style=\"color:#ffffff;text-decoration:none;font-weight:bold\">{Encode(block.Text)}</a></td></tr></table>",
                "columns" => $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"margin-bottom:20px\"><tr><td class=\"email-column\" width=\"50%\" valign=\"top\" style=\"padding:0 12px 12px 0\">{Lines(block.Text)}</td><td class=\"email-column\" width=\"50%\" valign=\"top\" style=\"padding-bottom:12px\">{Lines(block.SecondaryText)}</td></tr></table>",
                _ => throw new ArgumentException("Неизвестный блок.")
            });
        }
        if (!string.IsNullOrWhiteSpace(definition.Footer))
            html.Append($"<p style=\"margin:24px 0 0;font-size:12px;color:#667085;border-top:1px solid #e5e7eb;padding-top:16px\">{Lines(definition.Footer)}</p>");
        html.Append("</td></tr></table><!--[if mso]></td></tr></table><![endif]--></td></tr></table></body></html>");
        return new EmailPreviewDto(title, html.ToString());
    }
}
