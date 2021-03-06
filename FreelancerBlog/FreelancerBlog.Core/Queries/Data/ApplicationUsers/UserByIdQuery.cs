﻿using System.Security.Claims;
using FreelancerBlog.Core.Domain;
using MediatR;

namespace FreelancerBlog.Core.Queries.Data.ApplicationUsers
{
    public class UserByIdQuery : IRequest<ApplicationUser>
    {
        public ClaimsPrincipal User { get; set; }
    }
}