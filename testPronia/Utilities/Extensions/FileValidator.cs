namespace testPronia.Utilities.Extensions
{
	public static class FileValidator
	{
		public static bool ValidateType(this IFormFile file, string type = "image/")
		{
			if (file.ContentType.Contains(type))
			{
				return true;
			}

			return false;


		}

	}
}
