﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class TagController : Controller
	{
		private readonly AppDbContext _context;

		public TagController(AppDbContext context)
        {
			_context = context;
        }
        public async Task<IActionResult> Index()
		{
			List<Tag> tags = await _context.Tags.Include(t => t.ProductTags).ToListAsync();
			return View(tags);
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Tag tag)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			
			bool result = _context.Tags.Any(c => c.Name.ToLower().Trim() == tag.Name.ToLower().Trim());
			if (result)
			{
				ModelState.AddModelError("Name", "Tag already exists");
				return View();
			}

			await _context.Tags.AddAsync(tag);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) return BadRequest();
			Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
			return View(tag);
		}

		[HttpPost]
		public async Task<IActionResult> Update(int id, Tag tag)
		{
			if (!ModelState.IsValid) { return View(); }

			Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

			if (existed == null) return NotFound();

			bool result = _context.Tags.Any(t => t.Name == tag.Name && t.Id == id);
			if (result) { ModelState.AddModelError("Name", "Tag already exists"); return View(); }

			existed.Name = tag.Name;

			_context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		}

	}
}
