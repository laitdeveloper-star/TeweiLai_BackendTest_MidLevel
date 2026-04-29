namespace BackendExamHub.Api.Models;

public sealed class MyOfficeAcpd
{
    public required string AcpdSid { get; init; }
    public string? AcpdCname { get; init; }
    public string? AcpdEname { get; init; }
    public string? AcpdSname { get; init; }
    public string? AcpdEmail { get; init; }
    public byte? AcpdStatus { get; init; }
    public bool? AcpdStop { get; init; }
    public string? AcpdStopMemo { get; init; }
    public string? AcpdLoginId { get; init; }
    public string? AcpdLoginPwd { get; init; }
    public string? AcpdMemo { get; init; }
    public DateTime? AcpdNowDateTime { get; init; }
    public string? AcpdNowId { get; init; }
    public DateTime? AcpdUpdDateTime { get; init; }
    public string? AcpdUpdId { get; init; }
}
