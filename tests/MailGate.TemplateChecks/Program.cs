using System.Text.Json;
using MailGate.Api.Templates;
using MailGate.Contracts.Templates;

var checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception(name);
    checks++;
}
EmailTemplateDefinition Example() => new()
{
    Name = "Тестовый макет", Subject = "Тема по умолчанию", Preheader = "Краткое описание",
    Footer = "Команда поддержки", Blocks = [new() { Type = "heading", Text = "Здравствуйте" }, new() { Type = "content" }]
};
var definition = Example();
Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Valid definition");
var body = "Первая строка\r\n<script>alert('x')</script> & {{content}}";
var rendered = EmailTemplateRenderer.Render(definition, body);
Check(rendered.Subject == definition.Subject, "Subject fallback");
Check(rendered.Html.Contains("Первая строка<br />&lt;script&gt;"), "Body encoding and line breaks");
Check(!rendered.Html.Contains("<script>"), "No executable markup from text");
Check(rendered.Html.Contains("{{content}}"), "User text is not recursively interpolated");
Check(EmailTemplateRenderer.Render(definition, body, "  Другая тема  ").Subject == "Другая тема", "Subject override");
Check(rendered.Html.Contains("<!--[if mso]>") && rendered.Html.Contains("role=\"presentation\""), "Email table layout");
var snapshot = JsonSerializer.Deserialize<EmailTemplateDefinition>(JsonSerializer.Serialize(definition))!;
Check(EmailTemplateRenderer.Render(snapshot, body).Html == rendered.Html, "JSON persistence preserves rendering");
definition.Blocks.Add(new() { Type = "content" });
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks"), "Reject duplicate content slots");
definition.Blocks.RemoveAll(b => b.Type == "content");
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks"), "Require content slot");
definition = Example(); definition.AccentColor = "red; background:url(x)";
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Colors"), "Reject CSS injection");
definition = Example(); definition.Blocks.Add(new() { Type = "button", Text = "Click", Url = "javascript:alert(1)" });
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject executable URLs");
definition.Blocks[2].Url = "https://example.com/path?a=1&b=2";
Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Accept HTTPS button");
Check(EmailTemplateRenderer.Render(definition, body).Html.Contains("a=1&amp;b=2"), "Encode URL attributes");
definition.Blocks.Add(new() { Type = "columns", Text = "<left>", SecondaryText = "<right>" });
Check(EmailTemplateRenderer.Render(definition, body).Html.Contains("&lt;right&gt;"), "Encode column text");
definition.Subject = "Hello\r\nBcc:someone@example.com";
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Subject"), "Reject multiline subject");
definition = Example(); definition.Blocks.Add(null!);
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject null blocks without crashing");
definition = Example(); definition.Blocks = Enumerable.Repeat(new EmailTemplateBlock { Type = "content" }, 41).ToList();
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks"), "Limit block count");
Check(!EmailTemplateRenderer.IsHttpUrl("https://user:secret@example.com"), "Reject credentials in URLs");
definition = Example();
definition.Blocks.Insert(0, new() { Type = "image", Url = "https://example.com/banner.png?a=1&b=2", Text = "<Banner>" });
Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Accept HTTPS image");
var imageHtml = EmailTemplateRenderer.Render(definition, body).Html;
Check(imageHtml.Contains("alt=\"&lt;Banner&gt;\"") && imageHtml.Contains("banner.png?a=1&amp;b=2"), "Encode image attributes");
Check(imageHtml.IndexOf("<img ") < imageHtml.IndexOf("<h1 "), "Preserve image block order");
foreach (var url in new[] { "http://example.com/a.png", "javascript:alert(1)", "data:image/svg+xml,test", "https://user:secret@example.com/a.png", "/image.png" })
{
    definition.Blocks[0].Url = url;
    Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[0]"), "Reject unsafe image URL: " + url);
}
var png = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aNecAAAAASUVORK5CYII=";
definition = Example(); definition.Blocks.Add(new() { Type = "image", Url = png });
Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Accept uploaded PNG");
definition.Blocks[2].Url = png.Insert(30, "\n");
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject whitespace in base64 before SMTP conversion");
definition.Blocks[2].Url = png;
Check(EmailTemplateRenderer.Render(definition, body).Html.Contains(png), "Preserve embedded image in HTML snapshot");
var storedImage = JsonSerializer.Deserialize<EmailTemplateDefinition>(JsonSerializer.Serialize(definition))!;
Check(storedImage.Blocks[2].Url == png, "Preserve uploaded bytes in template JSON");
foreach (var invalid in new[] { "data:image/svg+xml;base64,PHN2Zz4=", "data:image/png;base64,aGVsbG8gd29ybGQ=", "data:image/png;base64,!!" })
{
    definition.Blocks[2].Url = invalid;
    Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject invalid uploaded image");
}
var big = new byte[128 * 1024]; Convert.FromBase64String(png.Split(',')[1])[..8].CopyTo(big, 0);
definition.Blocks[2].Url = "data:image/png;base64," + Convert.ToBase64String(big);
Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Accept image at size limit");
for (var i = 0; i < 4; i++) definition.Blocks.Add(new() { Type = "image", Url = definition.Blocks[2].Url });
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks"), "Limit total uploaded image bytes");
definition = Example(); definition.Blocks.Add(new() { Type = "image", Url = "data:image/png;base64," + Convert.ToBase64String(new byte[128 * 1024 + 1]) });
Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject oversized upload");
Console.WriteLine($"Passed {checks} email template checks.");
