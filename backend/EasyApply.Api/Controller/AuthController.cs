using EasyApply.DataAccess.Data;

namespace EasyApply.Api.Controller;

public class AuthController
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }
}