﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelancerBlog.Core.Domain;
using FreelancerBlog.Core.Enums;
using FreelancerBlog.Core.Repository;
using FreelancerBlog.Core.Services.ArticleServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FreelancerBlog.Infrastructure.Services.ArticleServices
{
    public class ArticleServices : IArticleServices
    {
        private readonly IUnitOfWork _uw;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArticleServices(IUnitOfWork uw, IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager)
        {
            _uw = uw;
            this._contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task<List<ArticleStatus>> CreateNewArticleAsync(Article article, string articleTags)
        {
            var articleStatusList = new List<ArticleStatus>();
            int addTagsResult = 0;
            int addTagsToArticleResult = 0;

            article.ArticleDateCreated = DateTime.Now;
            article.ArticleViewCount = 1;

            article.UserIDfk = _userManager.GetUserId(_contextAccessor.HttpContext.User);

            _uw.ArticleRepository.Add(article);

            int addArticleResult = await _uw.SaveAsync();

            if (!string.IsNullOrEmpty(articleTags))
            {
                var viewModelTags = articleTags.Split(',');

                addTagsResult = await _uw.ArticleRepository.AddRangeOfTags(viewModelTags);

                var tagsToAddToArticle = await _uw.ArticleRepository.FindByTagsName(viewModelTags);

                addTagsToArticleResult = await _uw.ArticleRepository.AddTagRangeToArticle(tagsToAddToArticle, article);

            }

            if (addArticleResult > 0)
            {
                articleStatusList.Add(ArticleStatus.ArticleCreateSucess);
            }

            if (addTagsResult > 0)
            {
                articleStatusList.Add(ArticleStatus.ArticleTagCreateSucess);
            }

            if (addTagsToArticleResult > 0)
            {
                articleStatusList.Add(ArticleStatus.ArticleArticleTagsCreateSucess);
            }

            return articleStatusList;
        }

        public async Task<List<ArticleStatus>> EditArticleAsync(Article article, string articleTags)
        {
            article.ArticleDateModified = DateTime.Now;
            var articleStatusList = new List<ArticleStatus>();
            int addTagsResult = 0;
            int removeTagsFromArticleResult = 0;
            int addTagsToArticleResult = 0;
            int tagsRemovalResult = 0;

            int updateArticleResult = await _uw.ArticleRepository.UpdateArticleAsync(article);

            var currentArticleTags = await _uw.ArticleRepository.GetCurrentArticleTagsAsync(article.ArticleId);

            if (!string.IsNullOrEmpty(articleTags))
            {
                var viewModelTags = articleTags.Split(',');

                var exceptRemovedTags = currentArticleTags.Select(c => c.ArticleTagName).Except(viewModelTags);

                var exceptAddedTags = viewModelTags.Except(currentArticleTags.Select(c => c.ArticleTagName));

                var tagsToRemoveFromArticle = await _uw.ArticleRepository.FindByTagsName(exceptRemovedTags);

                addTagsResult = await _uw.ArticleRepository.AddRangeOfTags(exceptAddedTags);

                var tagsToAddToArticle = await _uw.ArticleRepository.FindByTagsName(exceptAddedTags);

                removeTagsFromArticleResult = await _uw.ArticleRepository.RemoveTagRangeFromArticle(tagsToRemoveFromArticle, article.ArticleId);

                addTagsToArticleResult = await _uw.ArticleRepository.AddTagRangeToArticle(tagsToAddToArticle, article);
            }
            else
            {
                tagsRemovalResult = await _uw.ArticleRepository.RemoveTagRangeFromArticle(currentArticleTags, article.ArticleId);
            }

            if (updateArticleResult > 0)
            {
                articleStatusList.Add(ArticleStatus.ArticleEditSucess);
            }

            if (addTagsResult > 0)
            {
                articleStatusList.Add(ArticleStatus.ArticleTagCreateSucess);
            }

            if (removeTagsFromArticleResult > 0 || tagsRemovalResult > 0)
            {
                articleStatusList.Add(ArticleStatus.ArticleRemoveTagsFromArticleSucess);
            }

            if (addTagsToArticleResult > 0)
            {
                articleStatusList.Add(ArticleStatus.ArticleArticleTagsCreateSucess);
            }

            return articleStatusList;
        }
    }
}
