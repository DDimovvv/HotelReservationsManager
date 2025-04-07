using Hotel_Reservations_Manager.Data;
using Hotel_Reservations_Manager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Reservations_Manager.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.EGN == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            var user = new User
            {
                Active = true,
                HireDate = DateOnly.FromDateTime(DateTime.Today)
            };
            return View(user);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EGN,FirstName,MiddleName,LastName,HireDate,Active,Email,PhoneNumber,UserName")] User user, string password)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid().ToString();
                user.Active = true;

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }

        private bool UserExists(string id)
        {
            return _userManager.Users.Any(e => e.EGN == id);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EGN == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EGN,FirstName,MiddleName,LastName,HireDate,Active,ReleaseDate,Email,PhoneNumber,UserName,Id")] User user)
        {
            if (id != user.EGN)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.EGN == id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.FirstName = user.FirstName;
                    existingUser.MiddleName = user.MiddleName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.UserName = user.UserName;
                    existingUser.HireDate = user.HireDate;

                    if (existingUser.Active && !user.Active)
                    {
                        existingUser.Active = false;
                        existingUser.ReleaseDate = user.ReleaseDate ?? DateOnly.FromDateTime(DateTime.Today);
                    }
                    else if (!existingUser.Active && user.Active)
                    {
                        existingUser.Active = true;
                        existingUser.ReleaseDate = null;
                    }
                    else
                    {
                        existingUser.Active = user.Active;
                        existingUser.ReleaseDate = user.ReleaseDate;
                    }

                    var result = await _userManager.UpdateAsync(existingUser);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.EGN))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.EGN == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EGN == id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: User/ChangePassword/5
        public async Task<IActionResult> ChangePassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserId = id;
            ViewBag.UserName = $"{user.FirstName} {user.LastName}";

            return View();
        }

        // POST: User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, string newPassword)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (removePasswordResult.Succeeded)
            {
                var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
                if (addPasswordResult.Succeeded)
                {
                    TempData["SuccessMessage"] = "Password changed successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in addPasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                foreach (var error in removePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.UserId = id;
            ViewBag.UserName = $"{user.FirstName} {user.LastName}";
            return View();
        }
        // GET: User/SearchByEGN
        public IActionResult SearchByEGN()
        {
            return View();
        }

        // POST: User/SearchByEGN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByEGN(string egn)
        {
            if (string.IsNullOrWhiteSpace(egn))
            {
                ModelState.AddModelError("", "Please enter an EGN to search");
                return View();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EGN == egn);

            if (user == null)
            {
                ModelState.AddModelError("", "No user found with the specified EGN");
                return View();
            }

            return RedirectToAction("Details", new { id = user.EGN });
        }
    }
}