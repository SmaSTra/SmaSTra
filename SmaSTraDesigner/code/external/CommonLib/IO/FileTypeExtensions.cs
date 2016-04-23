namespace Common.IO
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	// TODO: (PS) Comment this.
	public static class FileTypeExtensions
	{
		#region constants

		/// <summary>
		/// List of audio file extensions.
		/// </summary>
		public static readonly string[] Audio = 
			{
				".mp3",
				".ogg",
				".wav",
				".ra"
			};

		/// <summary>
		/// List of graphics file extensions.
		/// </summary>
		public static readonly string[] Graphics = 
			{
				".bmp",
				".jpg",
				".jpeg",
				".png",
				".svg",
				".tga",
				".tif",
				".tiff",
				".pcx",
				".ico"
			};

		/// <summary>
		/// List of video file extensions.
		/// </summary>
		public static readonly string[] Video = 
			{
				".mkv",
				".avi",
				".mp4",
				".mpg",
				".mpe",
				".mpeg",
				".flv",
				".divx",
				".3gp",
				".asf",
				".m4v",
				".mov",
				".wmv",
				".rm"
			};

		#endregion constants

		#region static methods

		/// <summary>
		/// Determins whether the provided file-name, -path or -extension (including the dot ".") belongs to a audio file.
		/// </summary>
		/// <param name="file">file-name, -path or -extension (including the dot ".")</param>
		/// <returns></returns>
		public static bool IsAudioFile(string file)
		{
			return IsFileType(file, (IEnumerable<string>)Audio);
		}

		/// <summary>
		/// Determins whether the provided file-name, -path or -extension (including the dot ".") belongs to a file of a type specified by the given list of estensions.
		/// </summary>
		/// <param name="file">file-name, -path or -extension (including the dot ".")</param>
		/// <param name="fileExtensions">List of file extensions.</param>
		/// <returns></returns>
		public static bool IsFileType(string file, IEnumerable<string> fileExtensions)
		{
			return fileExtensions.Any(ext => file.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		/// Determins whether the provided file-name, -path or -extension (including the dot ".") belongs to a file of a type specified by the given list of estensions.
		/// </summary>
		/// <param name="file">file-name, -path or -extension (including the dot ".")</param>
		/// <param name="fileExtensions">List of file extensions.</param>
		/// <returns></returns>
		public static bool IsFileType(string file, params string[] fileExtensions)
		{
			return IsFileType(file, (IEnumerable<string>)fileExtensions);
		}

		/// <summary>
		/// Determins whether the provided file-name, -path or -extension (including the dot ".") belongs to a graphics file.
		/// </summary>
		/// <param name="file">file-name, -path or -extension (including the dot ".")</param>
		/// <returns></returns>
		public static bool IsGraphicsFile(string file)
		{
			return IsFileType(file, (IEnumerable<string>)Graphics);
		}

		/// <summary>
		/// Determins whether the provided file-name, -path or -extension (including the dot ".") belongs to a video file.
		/// </summary>
		/// <param name="file">file-name, -path or -extension (including the dot ".")</param>
		/// <returns></returns>
		public static bool IsVideoFile(string file)
		{
			return IsFileType(file, (IEnumerable<string>)Video);
		}

		#endregion static methods
	}
}