using EasyApply.Infrastructure.Data;

namespace EasyApplyAPI.Controllers;

public class JobController
{
    private readonly ApplicationDbContext _context;

    public JobController(ApplicationDbContext context)
    {
        _context = context;
    }

}