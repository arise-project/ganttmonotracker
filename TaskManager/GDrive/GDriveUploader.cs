using System;
using System.IO;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google;
using File = Google.Apis.Drive.v3.Data.File;

namespace GanttMonoTracker
{
    public class GDriveUploader
    {
        public bool Upload(GDriveCredentials cred, byte[] raw, string fileId)
        {
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred.Credential,
                ApplicationName = "Gantt Mono Tracker",
            });

            var body = new File { Name = fileId, Description = "Gantt Mono Tracker project", MimeType = "text/xml" };

			//https://developers.google.com/drive/v3/web/manage-uploads

			FilesResource.CreateMediaUpload request;
			using (var stream = new MemoryStream(raw))
			{
			    request = service.Files.Create(
			        body, stream, "text/xml");
			    request.Fields = "id";
			    request.Upload();
			}
			var file = request.ResponseBody;
		
            return true;
        }
    }
}
