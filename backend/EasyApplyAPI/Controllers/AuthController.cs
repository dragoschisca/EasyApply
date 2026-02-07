using EasyApplyAPI.Data;

namespace EasyApplyAPI.Controllers;

public class AuthController
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }
}