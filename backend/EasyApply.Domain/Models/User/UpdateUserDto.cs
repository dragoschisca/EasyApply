namespace EasyApply.Domain.Models.User;

public class UpdateUserDto
{
    public bool? IsActive { get; set; }
    public bool? EmailVerified { get; set; }
}
