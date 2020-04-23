using System;
using System.IO;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IImageStore
    {
        Task<Uri> SaveRaceImageAsync(User user, Race race, Stream imageStream, string contentType);
        Task<Uri> SaveBillImageAsync(DateTime purchaseDate, Stream imageStream, string contentType);
        Task<Stream> DownloadImageAsync(Uri imageUrl);
    }
}
