﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FreelancerBlog.Areas.Admin.ViewModels.Article;
using FreelancerBlog.AutoMapper;
using FreelancerBlog.Core.Commands.Articles;
using FreelancerBlog.Core.Commands.ArticleTags;
using FreelancerBlog.Core.Domain;
using FreelancerBlog.Core.Enums;
using FreelancerBlog.Core.Queries.Article;
using FreelancerBlog.Core.Queries.Articles;
using FreelancerBlog.Core.Queries.ArticleTags;
using FreelancerBlog.Core.Repository;
using FreelancerBlog.Core.Services.ArticleServices;
using FreelancerBlog.Core.Services.Shared;
using FreelancerBlog.ViewModels.Article;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ArticleController : Controller
    {
        private readonly IUnitOfWork _uw;
        private readonly ICkEditorFileUploder _ckEditorFileUploader;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private IMediator _mediator;
        private IArticleServices _articleServices;
        private readonly IFileManager _fileManager;

        public ArticleController(IUnitOfWork uw, ICkEditorFileUploder ckEditorFileUploader, IArticleServices articleServices, IFileManager fileManager, IMapper mapper, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _uw = uw;
            _ckEditorFileUploader = ckEditorFileUploader;
            _articleServices = articleServices;
            _fileManager = fileManager;
            _mapper = mapper;
            _userManager = userManager;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> ManageArticle()
        {
            var articles = await _mediator.Send(new GetAriclesQuery());

            var articlesViewModel = articles.ProjectTo<ArticleViewModel>().ToList();

            return View(articlesViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageArticleComment()
        {
            var comments = await _uw.ArticleRepository.GetAllCommentAsync();

            var commentsViewModel = _mapper.Map<List<ArticleComment>, List<ArticleCommentViewModel>>(comments);

            return View(commentsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageArticleTag()
        {
            var tags = await _uw.ArticleRepository.GetAllArticleTagsAsync();

            var tagsViewModel = _mapper.Map<List<ArticleTag>, List<ArticleTagViewModel>>(tags);

            return View(tagsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteArticleComment(int id)
        {
            if (id == default(int))
            {
                return Json(new { Status = "IdCannotBeNull" });
            }

            var model = await _uw.ArticleRepository.FindCommentByIdAsync(id);

            if (model == null)
            {
                return Json(new { Status = "ArticleCommentNotFound" });
            }

            int deleteArticleResult = await _uw.ArticleRepository.DeleteArticleCommentAsync(model);

            if (deleteArticleResult > 0)
            {
                return Json(new { Status = "Deleted" });
            }

            return Json(new { Status = "NotDeletedSomeProblem" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteArticleTag(int id)
        {
            if (id == default(int))
            {
                return Json(new { Status = "IdCannotBeNull" });
            }

            var model = await _uw.ArticleRepository.FindArticleTagByIdAsync(id);

            if (model == null)
            {
                return Json(new { Status = "ArticleCommentNotFound" });
            }

            int deleteArticleTagResult = await _uw.ArticleRepository.DeleteArticleTagAsync(model);

            if (deleteArticleTagResult > 0)
            {
                return Json(new { Status = "Deleted" });
            }

            return Json(new { Status = "NotDeletedSomeProblem" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangeArticleCommentApprovalStatus(int commentId)
        {
            if (commentId == default(int))
            {
                return Json(new { Status = "IdCannotBeNull" });
            }

            var model = await _uw.ArticleRepository.FindCommentByIdAsync(commentId);

            if (model == null)
            {
                return Json(new { Status = "ArticleCommentNotFound" });
            }

            int toggleArticleCommentApprovalResult = await _uw.ArticleRepository.ToggleArticleCommentApproval(model);

            if (toggleArticleCommentApprovalResult > 0)
            {
                return Json(new { Status = "Success" });
            }

            return Json(new { Status = "NotDeletedSomeProblem" });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var model = _mapper.Map<ArticleViewModel, Article>(viewModel);

            List<ArticleStatus> result = await _articleServices.CreateNewArticleAsync(model, viewModel.ArticleTags);

            if (!result.Any(r => r == ArticleStatus.ArticleCreateSucess))
            {
                TempData["ViewMessage"] = "مشکلی در ثبت مقاله پیش آمده، مقاله با موفقیت ثبت نشد.";

                return RedirectToAction("ManageArticle", "Article");
            }

            TempData["ViewMessage"] = "مقاله با موفقیت ثبت شد.";

            if (result.Any(r => r == ArticleStatus.ArticleTagCreateSucess))
            {
                TempData["ArticleTagCreateMessage"] = "تگ های جدید با موفقیت ثبت شدند.";
            }

            if (result.Any(r => r == ArticleStatus.ArticleArticleTagsCreateSucess))
            {
                TempData["ArticleArticleTagCreateMessage"] = "تگ ها با موفقیت به این مقاله اضافه شدند.";
            }

            return RedirectToAction("ManageArticle", "Article");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var article = await _uw.ArticleRepository.FindByIdAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            var articleViewModel = _mapper.Map<Article, ArticleViewModel>(article);
            articleViewModel.ArticleTags = await _uw.ArticleRepository.GetTagsByArticleIdAsync(article.ArticleId);
            articleViewModel.ArticleTagsList = await _uw.ArticleRepository.GetCurrentArticleTagsAsync(article.ArticleId);
            articleViewModel.SumOfRating = articleViewModel.ArticleRatings.Sum(a => a.ArticleRatingScore) / articleViewModel.ArticleRatings.Count;
            articleViewModel.CurrentUserRating = articleViewModel.ArticleRatings.SingleOrDefault(a => a.UserIDfk == _userManager.GetUserId(User));

            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var article = _mapper.Map<ArticleViewModel, Article>(viewModel);

            List<ArticleStatus> result = await _articleServices.EditArticleAsync(article, viewModel.ArticleTags);

            if (!result.Any(r => r == ArticleStatus.ArticleEditSucess))
            {
                TempData["ViewMessage"] = "مشکلی در ویرایش مقاله پیش آمده، مقاله با موفقیت ثبت نشد.";

                return RedirectToAction("ManageArticle", "Article");
            }

            TempData["ViewMessage"] = "مقاله با موفقیت ویرایش شد.";

            if (result.Any(r => r == ArticleStatus.ArticleTagCreateSucess))
            {
                TempData["ArticleTagCreateMessage"] = "تگ های جدید با موفقیت ثبت شدند.";
            }

            if (result.Any(r => r == ArticleStatus.ArticleArticleTagsCreateSucess))
            {
                TempData["ArticleArticleTagCreateMessage"] = "تگ ها با موفقیت به این مقاله اضافه شدند.";
            }

            if (result.Any(r => r == ArticleStatus.ArticleRemoveTagsFromArticleSucess))
            {
                TempData["ArticleArticleTagRemoveFromArticle"] = "تگ ها با موفقیت از این مقاله حذف شدند.";
            }

            return RedirectToAction("ManageArticle", "Article");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditArticleComment(int commentId, string newCommentBody)
        {
            if (commentId == default(int))
            {
                return Json(new { Status = "IdCannotBeNull" });
            }

            var model = await _uw.ArticleRepository.FindCommentByIdAsync(commentId);

            if (model == null)
            {
                return Json(new { Status = "ArticleCommentNotFound" });
            }

            int editCommentResult = await _uw.ArticleRepository.EditArticleCommentAsync(model, newCommentBody);

            if (editCommentResult > 0)
            {
                return Json(new { Status = "Success" });
            }

            return Json(new { Status = "NotDeletedSomeProblem" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditArticleTag(int tagId, string newTagName)
        {
            if (tagId == default(int))
            {
                return Json(new { Status = "IdCannotBeNull" });
            }

            var model = await _mediator.Send(new FindArticleTagByIdQuery { ArticleTagId = tagId });

            if (model == null)
            {
                return Json(new { Status = "ArticleTagNotFound" });
            }

            await _mediator.Send(new EditArticleTagCommand { ArticleTag = model, NewTagName = newTagName });

            return Json(new { Status = "Success" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            if (id == default(int))
            {
                return Json(new { Status = "IdCannotBeNull" });
            }
            var model = await _mediator.Send(new ArticleByArticleIdQuery { ArticleId = id });

            if (model == null)
            {
                return Json(new { Status = "ArticleNotFound" });
            }

            _fileManager.DeleteEditorImages(model.ArticleBody, new List<string> { "Files", "ArticleUploads" });

            await _mediator.Send(new DeleteArticleCommand { Article = model });

            return Json(new { Status = "Deleted" });
        }

        public async Task<IActionResult> TagLookup()
        {
            var model = await _mediator.Send(new GetAllTagNamesQuery());

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> CkEditorFileUploder(IFormFile file, string ckEditorFuncNum)
        {
            string htmlResult = await _ckEditorFileUploader.UploadFromCkEditorAsync(file, new List<string> { "images", "blog" }, ckEditorFuncNum);

            return Content(htmlResult, "text/html");
        }
    }
}
