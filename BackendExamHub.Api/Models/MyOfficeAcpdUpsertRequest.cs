using System.ComponentModel.DataAnnotations;

namespace BackendExamHub.Api.Models;

public sealed class MyOfficeAcpdUpsertRequest
{
    [MaxLength(60)]
    public string? AcpdCname { get; init; }

    [MaxLength(40)]
    public string? AcpdEname { get; init; }

    [MaxLength(40)]
    public string? AcpdSname { get; init; }

    [EmailAddress]
    [MaxLength(60)]
    public string? AcpdEmail { get; init; }

    public byte? AcpdStatus { get; init; }

    public bool? AcpdStop { get; init; }

    [MaxLength(60)]
    public string? AcpdStopMemo { get; init; }

    [MaxLength(30)]
    public string? AcpdLoginId { get; init; }

    [MaxLength(60)]
    public string? AcpdLoginPwd { get; init; }

    [MaxLength(600)]
    public string? AcpdMemo { get; init; }

    [MaxLength(20)]
    public string? OperatorId { get; init; }
}
