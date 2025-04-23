namespace Demo.PL.Helper
{
    public static class DocumentSettings
    {
        public static string UploadFile (IFormFile file, string folderName)
        {
            // get located folder 
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", folderName);
            // get file name and make it unique 
            var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(file.FileName)}";
            // get file path 
            var filePath = Path.Combine(folderPath, fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);
            return fileName;
            
        }
        public static void DeleteFile (string fileName, string folderName)
        {
            // get folder path 
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", folderName);
            // get file path 
            var filePath = Path.Combine(folderPath, fileName);
            if(File.Exists(filePath))
            {
                File.Delete(filePath);  
            }
        }
    }
}
