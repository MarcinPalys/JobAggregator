using JobAggregator.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobAggregator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobRepository _repository;

    public JobsController(IJobRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var jobs = await _repository.GetAllAsync(cancellationToken);
        return Ok(jobs);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var job = await _repository.GetByIdAsync(id, cancellationToken);
        if (job is null) return NotFound();
        return Ok(job);
    }
}