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
                ApplicationName = "GMT",
            });

            var body = new File { Name = fileId, Description = "Gantt Mono Tracker project", MimeType = "text/xml" };

			//https://developers.google.com/drive/v3/web/manage-uploads

			FilesResource.CreateMediaUpload request;
			using (var stream = new MemoryStream(raw))
			{
			    request = service.Files.Create(
			        body, stream, "text/xml");
			    request.Fields = "id";

				/*{
				 "error": {
				  "errors": [
				   {
				    "domain": "global",
				    "reason": "insufficientPermissions",
				    "message": "Insufficient Permission"
				   }
				  ],
				  "code": 403,
				  "message": "Insufficient Permission"	
				 }
				}*/

			    request.Upload();
			}
			var file = request.ResponseBody;
		
            return true;
        }
    }
}
