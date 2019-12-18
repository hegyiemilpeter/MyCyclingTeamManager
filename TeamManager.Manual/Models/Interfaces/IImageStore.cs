using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IImageStore
    {
        Task<Uri> SaveRaceImageAsync(User user, Stream imageStream, string fileName, Race race);

        Task<Stream> DownloadImageAsync(Uri imageUrl);
    }
}
