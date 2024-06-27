namespace ResultPattern.Models
{
    public class Role
    {
        public Role()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class RoleDTO
    {
        public string Name { get; set; }
    }

    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
