﻿using AutoMapper;
using FreelancerBlog.Areas.Admin.ViewModels.SlideShow;
using FreelancerBlog.Core.Domain;

namespace FreelancerBlog.AutoMapper.Profiles
{
    public class SlideShowProfile: Profile
    {
        public SlideShowProfile()
        {
            CreateMap<SlideShow, SlideShowViewModel>();
            CreateMap<SlideShowViewModel, SlideShow>();
            CreateMap<SlideShow, SlideShowViewModelEdit>();
            CreateMap<SlideShowViewModelEdit, SlideShow>();
        }
    }
}