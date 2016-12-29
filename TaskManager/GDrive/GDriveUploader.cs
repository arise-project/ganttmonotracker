using System;

using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google;

namespace GanttMonoTracker
{
	public class GDriveUploader
	{
		public bool Upload(GDriveCredentials cred, byte [] raw, string fileId)
		{
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred.Credential,
                ApplicationName = "Gantt Mono Tracker",
            });

            var body = new File { Title = fileId, Description = "Gantt Mono Tracker project", MimeType = "text/xml" };

		    using (var stream = new System.IO.MemoryStream(raw))
		    {
				var l = service.Files.List ();
				var list = l.Execute ();
				File file = null;
				foreach (var f in list.Items) {
					if (f.Description == "Gantt Mono Tracker project") {
						file = f;
					}
				}

				if (file == null || string.IsNullOrEmpty(file.DownloadUrl))
		        {
                    FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, "text/plain");
                    request.Upload();
		        }
		        else
		        {
                    var request = service.Files.Update(body, fileId, stream, "text/plain");
                    request.Upload();
		        }
		    }

		    return true;
		}
	}
}
