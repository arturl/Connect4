namespace BlazorApp
{
	using System.ComponentModel.DataAnnotations;

	public class StartGameForm
	{
		[Required]
		[Range(0, 20, ErrorMessage = "Level invalid (0-20).")]
		public int Level { get; set; }
	}
}
