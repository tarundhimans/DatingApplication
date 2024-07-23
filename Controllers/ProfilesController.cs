﻿using DatingApplication.Data;
using DatingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DatingApplication.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(profile);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Bio,Preferences,Day,Month,Year,Gender,InterestedIn")] Profile profile, IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                ModelState.AddModelError("ProfilePicture", "Profile Picture is required.");
            }

           
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                profile.UserId = userId;

                using (var memoryStream = new MemoryStream())
                {
                    await profilePicture.CopyToAsync(memoryStream);
                    profile.ProfilePicture = memoryStream.ToArray();
                }

                _context.Add(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
     
        }

        // GET: Profiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            // Retain the UserId for the view
            ViewData["UserId"] = profile.UserId;
            return View(profile);
        }

        // POST: Profiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Profile profile, IFormFile profilePicture)
        {
            if (id != profile.Id)
            {
                return NotFound();
            }

            try
            {
                var existingProfile = await _context.Profiles.FindAsync(id);
                if (existingProfile == null)
                {
                    return NotFound();
                }

                // Update existing profile details
                existingProfile.Bio = profile.Bio;
                existingProfile.Preferences = profile.Preferences;
                existingProfile.Day = profile.Day;
                existingProfile.Month = profile.Month;
                existingProfile.Year = profile.Year;
                existingProfile.Gender = profile.Gender;
                existingProfile.InterestedIn = profile.InterestedIn;

                if (profilePicture != null && profilePicture.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await profilePicture.CopyToAsync(memoryStream);
                        existingProfile.ProfilePicture = memoryStream.ToArray();
                    }
                }

                // Update the profile in the database
                _context.Update(existingProfile);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(profile.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}
