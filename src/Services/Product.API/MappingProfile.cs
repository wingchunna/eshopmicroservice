using AutoMapper;
using Infrastructure.Mappings;
using Product.API.Entities;
using Shared.DTOs.Product;

namespace Product.API
{
    // sử dụng mapper để thay đổi dữ liệu hiển thị. Loại bỏ trường dữ liệu không cần thiết như created_date, updated_date

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CatalogProduct, ProductDto>();
            CreateMap<CreateProductDto, CatalogProduct>();
            CreateMap<UpdateProductDto, CatalogProduct>().IgnoreAllNonExisting(); // thêm IgnoreAll để những trường dữ liệu nào ko update thì sẽ bỏ qua và giữ nguyên giá trị chứ ko bị gán null

        }
    }
}
