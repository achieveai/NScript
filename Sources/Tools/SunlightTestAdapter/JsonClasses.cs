namespace SunlightTestAdapter;

using Newtonsoft.Json;

public class Assertions
{
    [JsonProperty("passed")]
    public bool Passed { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("todo")]
    public bool Todo { get; set; }
}

public class Error
{
    [JsonProperty("passed")]
    public bool Passed { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("stack")]
    public string Stack { get; set; }

    [JsonProperty("todo")]
    public bool Todo { get; set; }
}

public class RootObject
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("suiteName")]
    public string SuiteName { get; set; }

    [JsonProperty("fullName")]
    public string[] FullName { get; set; }

    [JsonProperty("runtime")]
    public int Runtime { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("errors")]
    public Error[] Errors { get; set; }

    [JsonProperty("assertions")]
    public Error[] Assertions { get; set; }
}