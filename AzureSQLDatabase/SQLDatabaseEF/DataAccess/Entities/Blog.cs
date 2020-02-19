using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess.Entities
{
	public class Blog
	{
		[Key]
		public int BlogId { get; set; }
		[Required]
		public string Url { get; set; }
		public string Description { get; set; }
	}

}
