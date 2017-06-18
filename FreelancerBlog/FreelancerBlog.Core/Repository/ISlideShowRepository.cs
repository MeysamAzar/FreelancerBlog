﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FreelancerBlog.Core.Domain;

namespace FreelancerBlog.Core.Repository
{
    public interface ISlideShowRepository : IRepository<SlideShow, int>
    {
        Task<int> AddNewSlideShow(SlideShow slideShow);
        Task<int> DeleteSlideShowAsync(SlideShow model);
        Task<List<SlideShow>> GetAllAsyncForHomePage();
        Task<int> UpdateSlideShowAsync(SlideShow slideshow);
        void Detach(SlideShow model);
    }
}