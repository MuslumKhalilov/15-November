﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using testPronia.DAL;

namespace testPronia.ViewComponents
{
    public class NavbarViewComponents: ViewComponent
    {
        private readonly AppDbContext _context;
        public NavbarViewComponents(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return View(settings);
        }
    }
}
