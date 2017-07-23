using System;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;

namespace GanttMonoTracker
{
    public class GDriveCredentials
    {
        public GDriveCredentials(string CLIENT_ID, string CLIENT_SECRET)
        {
            this.CLIENT_ID = CLIENT_ID;
            this.CLIENT_SECRET = CLIENT_SECRET;
            Scopes = new string[] { DriveService.Scope.Drive,
                                 DriveService.Scope.DriveFile};
        }

        public string CLIENT_ID { get; set; }

        public string CLIENT_SECRET { get; set; }

        public UserCredential Credential { get; set; }

        public string[] Scopes { get; set; }
    }
}
