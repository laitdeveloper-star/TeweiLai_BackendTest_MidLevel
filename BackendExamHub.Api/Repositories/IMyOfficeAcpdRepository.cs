using BackendExamHub.Api.Models;

namespace BackendExamHub.Api.Repositories;

public interface IMyOfficeAcpdRepository
{
    Task<IReadOnlyList<MyOfficeAcpd>> GetAllAsync(CancellationToken cancellationToken);
    Task<MyOfficeAcpd?> GetByIdAsync(string sid, CancellationToken cancellationToken);
    Task<MyOfficeAcpd> CreateAsync(MyOfficeAcpdUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(string sid, MyOfficeAcpdUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string sid, CancellationToken cancellationToken);
}
