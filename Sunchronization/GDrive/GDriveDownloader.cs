using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using File = Google.Apis.Drive.v3.Data.File;

namespace GanttMonoTracker
{
	public class GDriveDownloader
	{
		public byte[] Download(GDriveCredentials cred, string fileId)
		{
			var service = new DriveService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = cred.Credential,
				ApplicationName = "GMT",
			});

			var l = service.Files.List();
			var list = l.Execute();
			File file1 = null;
			foreach (var f in list.Files)
			{
				if (fileId.Equals(f.Name, System.StringComparison.OrdinalIgnoreCase))
				{
					file1 = f;
				}
			}

			if (file1 == null)
				return null;

			//https://developers.google.com/drive/v3/web/manage-downloads
			var request = service.Files.Get(file1.Id);
			using (var s = new MemoryStream())
			{
				request.Download(s);
				return s.ToArray();
			}
		}
	}
}
