using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.IO;
using Google.Apis.Drive.v3.Data;
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
                ApplicationName = "Gantt Mono Tracker",
            });

            var l = service.Files.List();
            var list = l.Execute();
            File file1 = null;
			foreach (var f in list.Files)
            {
                if (f.Description == "Gantt Mono Tracker project")
                {
                    file1 = f;
                }
            }

			//https://developers.google.com/drive/v3/web/manage-downloads
            var request = service.Files.Get(file1.Id);
			using (var s = new MemoryStream())
			{
				request.Download(s);
				return s.ToArray();
			}
        }

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
