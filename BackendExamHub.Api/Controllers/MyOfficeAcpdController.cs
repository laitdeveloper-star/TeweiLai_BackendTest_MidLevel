using BackendExamHub.Api.Models;
using BackendExamHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BackendExamHub.Api.Controllers;

[ApiController]
[Route("api/myofficeacpd")]
public sealed class MyOfficeAcpdController(IMyOfficeAcpdRepository repository) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MyOfficeAcpd>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MyOfficeAcpd>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{sid}")]
    [ProducesResponseType(typeof(MyOfficeAcpd), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MyOfficeAcpd>> GetById(string sid, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(sid, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MyOfficeAcpd), StatusCodes.Status201Created)]
    public async Task<ActionResult<MyOfficeAcpd>> Create(
        [FromBody] MyOfficeAcpdUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var item = await repository.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { sid = item.AcpdSid }, item);
    }

    [HttpPut("{sid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        string sid,
        [FromBody] MyOfficeAcpdUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await repository.UpdateAsync(sid, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{sid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string sid, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(sid, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
