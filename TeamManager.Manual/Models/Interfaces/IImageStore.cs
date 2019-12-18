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
        Task<Uri> SaveRaceImageAsync(User user, Race race, Stream imageStream, string contentType);
        Task<Uri> SaveBillImageAsync(DateTime purchaseDate, Stream imageStream, string contentType);
        Task<Stream> DownloadImageAsync(Uri imageUrl);
    }
}
