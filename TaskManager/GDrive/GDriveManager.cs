using System;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Util.Store;

namespace GanttMonoTracker
{
    public class GDriveManager
    {
        public GDriveManager(GDriveCredentials credentials)
        {
            Uploader = new GDriveUploader();
            Downloader = new GDriveDownloader();
            Credentials = credentials;
        }

		/// <summary>
		/// Authorize this instance.
		/// to setup credentials : https://console.developers.google.com/apis/library
		/// </summary>
        public void Authorize()
        {
            //Scopes for use with the Google Drive API
           
            // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            Credentials.Credential =
                        GoogleWebAuthorizationBroker
                                      .AuthorizeAsync(new ClientSecrets
                                      {
                                          ClientId = Credentials.CLIENT_ID,
                                          ClientSecret = Credentials.CLIENT_SECRET
                                      },
                                      Credentials.Scopes,
                                      Environment.UserName, 
                                      CancellationToken.None,
                                      new FileDataStore("gmt")).Result;
        }

        public GDriveCredentials Credentials { get; set; }

        public GDriveUploader Uploader { get; set; }

        public GDriveDownloader Downloader { get; set; }
    }
}
