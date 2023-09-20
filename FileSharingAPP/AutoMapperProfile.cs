using AutoMapper;

namespace FileSharingAPP
{
    public class UploadProfile : Profile
    {
        public UploadProfile()
        {
            CreateMap<Models.InputUpload, Data.Uploads>();
            CreateMap<Data.Uploads, Data.Uploads>();
        }
    }
}
