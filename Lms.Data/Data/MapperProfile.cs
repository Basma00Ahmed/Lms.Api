using AutoMapper;
using Lms.Core.Dtos;
using Lms.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lms.Data.Data
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {

                CreateMap<Course, CourseDto>()
                 .ForMember(
                    des => des.EndDate,
                    from => from.MapFrom(s => s.StartDate.AddMonths(3)))
                 .ReverseMap();


                CreateMap<Module, ModuleDto>()
                                  .ForMember(
                        des => des.EndDate,
                        from => from.MapFrom(s => s.StartDate.AddMonths(1)))
                                  .ReverseMap();
            
        }
    }
}
