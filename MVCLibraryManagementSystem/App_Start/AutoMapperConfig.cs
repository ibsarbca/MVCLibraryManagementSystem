using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MVCLibraryManagementSystem.Models;
using MVCLibraryManagementSystem.ViewModels;

namespace MVCLibraryManagementSystem.App_Start
{
    public class AutoMapperConfig
    {
        public static void MapViewModelsToModels()
        {
            Mapper.CreateMap<BookViewModel, Book>()
                .ForMember(dest => dest.Item, opts => opts.MapFrom(src => new Item { Title = src.ItemTitle, Year = src.ItemYear }));
        }
    }
}