using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;

namespace GanttMonoTracker
{
    public class GDriveManager
    {
		static string[] Scopes = { DriveService.Scope.DriveFile, DriveService.Scope.Drive, DriveService.Scope.DriveAppdata, DriveService.Scope.DriveReadonly };

        public GDriveManager()
        {
            Uploader = new GDriveUploader();
            Downloader = new GDriveDownloader();
        }

        /// <summary>
        /// Authorize this instance.
        /// to setup credentials : https://console.developers.google.com/apis/library
        /// https://console.developers.google.com/home/dashboard?project=gantt-mono-tracker
        /// A problem with W10
        /// https://github.com/google/google-api-dotnet-client/issues/557
        /// Use access_type offline with the Google C# SDK to prevent the need to reauthenticate every hour 
        /// https://gist.github.com/SNiels/d2d39276bdeaeaa4d6b6148a0ab02a48
        /// </summary>
        public async Task AuthorizeAsync()
        {
			//TODO: exceptions from async methods hangs app.

			var credFile = "TaskManager\\GDrive\\client_id.json";
            string credPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string resultPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            credPath = Path.Combine(credPath, credFile);
            using (var stream =
                new FileStream(credPath, FileMode.Open, FileAccess.Read))
            {
                //Scopes for use with the Google Drive API

                var secrets = GoogleClientSecrets.Load(stream).Secrets;
                Credentials = new GDriveCredentials(secrets.ClientId, secrets.ClientSecret);
                Credentials.Credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new[] { Uri.EscapeUriString(DriveService.Scope.DriveReadonly) },
                Environment.UserName,
                    CancellationToken.None);
            };

        }

        public GDriveCredentials Credentials { get; set; }

        public GDriveUploader Uploader { get; set; }

        public GDriveDownloader Downloader { get; set; }

        public async Task Revoke()
        {
            var cancelationSource = new CancellationTokenSource();
            await Credentials.Credential.RevokeTokenAsync(cancelationSource.Token);
        }
    }
}
