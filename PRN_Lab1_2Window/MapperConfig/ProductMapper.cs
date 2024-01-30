using AutoMapper;
using PRN_Lab1_2Window.DTOClasses;
using PRN_Lab1_2Window.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN_Lab1_2Window.MapperConfig
{
    internal class ProductMapper : Profile
    {
        public ProductMapper()
        {

        }
        public static Mapper InitMapper()
        {
            var config = new MapperConfiguration(conf =>
            {
                conf.CreateMap<Product, ProductDTO>()
                .ForPath(
                    dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier.CompanyName)
                )
                .ForPath(
                    dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.CategoryName)
                    )
                .ForMember(
                    //dest => dest.OrderDetails.GroupBy(x => x.ProductId).Sum(y => y.Sum(x => x.Quantity)),
                    dest => dest.SoldNumber,
                    opt => opt.MapFrom((x, dest) => x.OrderDetails.Sum(s => s.Quantity))
                ).ReverseMap();

            });
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
