namespace sample.Entities;

public class Role
{
    public int Id { get; set; }
    public required string Value { get; set; }

    public List<User> Users { get; set; } = new List<User>();
}