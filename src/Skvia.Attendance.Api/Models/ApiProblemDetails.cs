using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace Skvia.Attendance.Api.Models;

public class ApiProblemDetails : ProblemDetails
{
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }
}
