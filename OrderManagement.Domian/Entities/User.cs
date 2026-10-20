
namespace OrderManagement.Domian.Entities;

public class User
{
    public Guid UserID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
