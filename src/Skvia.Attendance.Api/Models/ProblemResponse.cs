using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;

namespace Skvia.Attendance.Api.Models;

public class ProblemResponse : ProblemDetails
{
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }
}
