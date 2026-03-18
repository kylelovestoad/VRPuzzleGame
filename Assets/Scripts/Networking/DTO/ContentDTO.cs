using System;

namespace Networking.DTO
{
    
    [Serializable]
    public class ContentDTO
    {
        public string id;
        public string filename;
        public string contentType;
        public long fileSize;
        public string downloadUrl;
    }
}