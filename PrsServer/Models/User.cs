using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PrsServer.Models
{
	public class User
	{
		public int Id { get; set; }
		[StringLength (30), Required]
		public string Username { get; set; }
		[StringLength(30), Required]
		public string Password { get; set; }
		[StringLength(30), Required]
		public string Firstname { get; set; }
		[StringLength(30), Required]
		public string Lastname { get; set; }
		[StringLength(12)]
		public string Phone { get; set; }
		[StringLength(255)]
		public string Email { get; set; }
		[Required]
		public Boolean IsReviewer { get; set; }
		[Required]
		public Boolean IsAdmin { get; set; }

	}
}
