namespace SunlightTestAdapter;

using System.Text.Json;
using System.Text.Json.Serialization;

public class Assertion
{
    [JsonPropertyName("passed")]
    public bool Passed { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("todo")]
    public bool Todo { get; set; }

    [JsonPropertyName("actual")]
    public JsonElement Actual { get; set; }

    [JsonPropertyName("expected")]
    public JsonElement Expected { get; set; }
}

public class QUnitError
{
    [JsonPropertyName("passed")]
    public bool Passed { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("stack")]
    public string? Stack { get; set; }

    [JsonPropertyName("todo")]
    public bool Todo { get; set; }
}

public class RootObject
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("suiteName")]
    public string? SuiteName { get; set; }

    [JsonPropertyName("fullName")]
    public string[]? FullName { get; set; }

    [JsonPropertyName("runtime")]
    public double Runtime { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("errors")]
    public QUnitError[]? Errors { get; set; }

    [JsonPropertyName("assertions")]
    public Assertion[]? Assertions { get; set; }
}
