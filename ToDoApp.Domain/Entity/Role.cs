using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Domain.Entity
{
    public class Role : IdentityRole<int>
    {
        public Role() { }
        public Role(string name) : base(name) { }
    }
}