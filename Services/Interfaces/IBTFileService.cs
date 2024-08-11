namespace TheBugTracker.Services.Interfaces
{
    public interface IBTFileService
    {
        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);
        public string ConvertByteArrayToFlie(byte[] fileData, string extension);
        //this represent the icon of the fie type chosen by the user.
        public string GetFileIcon(string file);
        //is a way of showing aditional infor about the file.
        //will be used in the ticket details.
        //it returns the suffix of the file("GB" / "MB" / "KB" etc)
        public string FormatFileSize(long bytes);
    }
}
