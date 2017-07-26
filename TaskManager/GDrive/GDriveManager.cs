using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GanttMonoTracker
{
    public class GDriveManager
    {
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";

        public GDriveManager(GDriveCredentials credentials)
        {
            Uploader = new GDriveUploader();
            Downloader = new GDriveDownloader();
            Credentials = credentials;
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
        public async void Authorize()
        {

            var credFile = "TaskManager\\GDrive\\client_secret_167315580398-e93kt4cfp2qnthgmpgf1hdn5p1u91e9a.apps.googleusercontent.com.json";
            var resultFile = "TaskManager\\GDrive\\connect.json";
            string credPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string resultPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            credPath = Path.Combine(credPath, credFile);
            resultPath = Path.Combine(resultPath, resultFile);
            using (var stream =
                new FileStream(credPath, FileMode.Open, FileAccess.Read))
            {
                //Scopes for use with the Google Drive API

                var secrets = GoogleClientSecrets.Load(stream).Secrets;

				var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
				secrets,
				new[] { Uri.EscapeUriString(DriveService.Scope.DriveReadonly ) },
				"user",
					CancellationToken.None);

				var t = credential.Token;
			/*
				var calendarService = new CalendarService(new BaseClientService.Initializer
				{
					HttpClientInitializer = credential,
					ApplicationName = "Windows 8.1 Calendar sample"
				});
				var calendarListResource = await calendarService.CalendarList.List().ExecuteAsync();
				*/

				/*
                IAuthorizationCodeFlow flow =
        new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = secrets,
            Scopes = new[] { DriveService.Scope.Drive },
            //DataStore = new FileDataStore("Drive.Api.Auth.Store")
            //DataStore = new GDriveMemoryDataStore(commonUser, refreshToken)
            DataStore = new GDriveMemoryDataStore()
        });
        */


                //// here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
                /*var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                  secrets,
                  Scopes,
                  "user",
                  CancellationToken.None,
                  new FileDataStore(resultPath, true)).Result;
                  */
            }

        }


        internal class GDriveMemoryDataStore : IDataStore
        {
            private Dictionary<string, TokenResponse> _store;
            private Dictionary<string, string> _stringStore;

            //private key password: notasecret

            public GDriveMemoryDataStore()
            {
                _store = new Dictionary<string, TokenResponse>();
                _stringStore = new Dictionary<string, string>();
            }

            public GDriveMemoryDataStore(string key, string refreshToken)
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");
                if (string.IsNullOrEmpty(refreshToken))
                    throw new ArgumentNullException("refreshToken");

                _store = new Dictionary<string, TokenResponse>();

                // add new entry
                StoreAsync<TokenResponse>(key,
                    new TokenResponse() { RefreshToken = refreshToken, TokenType = "Bearer" }).Wait();
            }

            /// <summary>
            /// Remove all items
            /// </summary>
            /// <returns></returns>
            public async Task ClearAsync()
            {
                await Task.Run(() =>
                {
                    _store.Clear();
                    _stringStore.Clear();
                });
            }

            /// <summary>
            /// Remove single entry
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public async Task DeleteAsync<T>(string key)
            {
                await Task.Run(() =>
                {
                    // check type
                    AssertCorrectType<T>();

                    if (typeof(T) == typeof(string))
                    {
                        if (_stringStore.ContainsKey(key))
                            _stringStore.Remove(key);
                    }
                    else if (_store.ContainsKey(key))
                    {
                        _store.Remove(key);
                    }
                });
            }

            /// <summary>
            /// Obtain object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public async Task<T> GetAsync<T>(string key)
            {
                // check type
                AssertCorrectType<T>();

                if (typeof(T) == typeof(string))
                {
                    if (_stringStore.ContainsKey(key))
                        return await Task.Run(() => { return (T)(object)_stringStore[key]; });
                }
                else if (_store.ContainsKey(key))
                {
                    return await Task.Run(() => { return (T)(object)_store[key]; });
                }
                // key not found
                return default(T);
            }

            /// <summary>
            /// Add/update value for key/value
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public Task StoreAsync<T>(string key, T value)
            {
                return Task.Run(() =>
                {
                    if (typeof(T) == typeof(string))
                    {
                        if (_stringStore.ContainsKey(key))
                            _stringStore[key] = (string)(object)value;
                        else
                            _stringStore.Add(key, (string)(object)value);
                    }
                    else
                    {
                        if (_store.ContainsKey(key))
                            _store[key] = (TokenResponse)(object)value;
                        else
                            _store.Add(key, (TokenResponse)(object)value);
                    }
                });
            }

            /// <summary>
            /// Validate we can store this type
            /// </summary>
            /// <typeparam name="T"></typeparam>
            private void AssertCorrectType<T>()
            {
                if (typeof(T) != typeof(TokenResponse) && typeof(T) != typeof(string))
                    throw new NotImplementedException(typeof(T).ToString());
            }
        }

        public GDriveCredentials Credentials { get; set; }

        public GDriveUploader Uploader { get; set; }

        public GDriveDownloader Downloader { get; set; }
    }
}
