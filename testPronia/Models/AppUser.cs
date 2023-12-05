using Microsoft.AspNetCore.Identity;

namespace testPronia.Models
{
	public class AppUser:IdentityUser
	{
        public string Name { get; set; }
		public string Surname { get; set; }
        public string Gender { get; set; }

    }
}
