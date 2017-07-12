﻿using Blogifier.Core.Common;
using Blogifier.Core.Data.Interfaces;
using Blogifier.Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Blogifier.Core.Controllers
{
    [Route("category")]
	public class CategoryController : Controller
	{
		IUnitOfWork _db;
        ILogger _logger;
        private readonly string _themePattern = "~/Views/Blogifier/Blog/{0}/Category.cshtml";
        string _theme;

		public CategoryController(IUnitOfWork db, ILogger<CategoryController> logger)
		{
			_db = db;
            _logger = logger;
			_theme = string.Format(_themePattern, ApplicationSettings.BlogTheme);
        }

        [Route("{slug}/{page:int?}")]
        public async Task<IActionResult> PagedCategoryAsync(string slug, int page = 1)
        {
            var pager = new Pager(page);
            var posts = await _db.BlogPosts.ByCategory(slug, pager);

            if (page < 1 || page > pager.LastPage)
                return View(_theme + "Error.cshtml", 404);

            var category = _db.Categories.Single(c => c.Slug == slug);

            var categories = _db.Categories.All().OrderBy(c => c.Title)
                .GroupBy(c => c.Title).Select(group => group.First()).Take(10)
                .Select(c => new SelectListItem { Text = c.Title, Value = c.Slug }).ToList();

            return View(_theme, new BlogCategoryModel { Categories = categories,
                Category = category, Posts = posts, Pager = pager });
        }
    }
}
