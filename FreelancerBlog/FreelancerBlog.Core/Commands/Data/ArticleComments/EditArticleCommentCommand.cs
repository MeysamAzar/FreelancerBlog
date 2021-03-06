﻿using FreelancerBlog.Core.Domain;
using MediatR;

namespace FreelancerBlog.Core.Commands.Data.ArticleComments
{
    public class EditArticleCommentCommand : IRequest
    {
        public ArticleComment ArticleComment { get; set; }
        public string NewCommentBody { get; set; }
    }
}