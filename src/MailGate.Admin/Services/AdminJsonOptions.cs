using System.Text.Json;
using System.Text.Json.Serialization;

namespace MailGate.Admin.Services;

internal static class AdminJsonOptions
{
    public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}
