using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;


namespace Lab4.Controllers
{
    public class CommunitiesController : Controller
    {
        private readonly SchoolCommunityContext _context;

        public CommunitiesController(SchoolCommunityContext context)
        {
            _context = context;
        }

        // GET: Communities
        public async Task<IActionResult> Index(string ID)
        {
            var viewModel = new CommunityViewModel();


            viewModel.Communities = await _context.Communities
               .Include(i => i.Membership)
               .ToListAsync();

            viewModel.Students = await _context.Students
               .Include(k => k.Membership)
               .ToListAsync();

            viewModel.CommunityMemberships = await _context.CommunityMemberships
               .ToListAsync();


            foreach(var communityItem in viewModel.Communities)
            {
                var membershipsFound = viewModel.CommunityMemberships.Where(x => x.CommunityId == communityItem.Id);
                communityItem.Membership = membershipsFound;   
            }

            foreach (var studentItem in viewModel.Students)
            {
                var membershipsFound = viewModel.CommunityMemberships.Where(x => x.StudentId == studentItem.Id);
                studentItem.Membership = membershipsFound;
            }


            if (ID != null)
            {
                ViewData["CommunityID"] = ID;
               
            }


            return View(viewModel);
        }

        // GET: Communities/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var community = await _context.Communities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (community == null)
            {
                return NotFound();
            }

            return View(community);
        }

        // GET: Communities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Communities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Budget")] Community community)
        {
            if (ModelState.IsValid)
            {
                _context.Add(community);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(community);
        }

        // GET: Communities/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var community = await _context.Communities.FindAsync(id);
            if (community == null)
            {
                return NotFound();
            }
            return View(community);
        }

        // POST: Communities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Budget")] Community community)
        {
            if (id != community.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(community);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommunityExists(community.Id))
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
            return View(community);
        }

        // GET: Communities/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var community = await _context.Communities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (community == null)
            {
                return NotFound();
            }

            return View(community);
        }

        // POST: Communities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //var community = await _context.Communities.FindAsync(id);

            var community = await _context.Communities.Include(x => x.Advertisements).FirstOrDefaultAsync(i => i.Id == id);
            
            var advertisements = _context.Advertisements.Include(x => x.CommunityAdvertisment).Where(x => x.CommunityAdvertisment.CommunityId == id);


            
            // if there is an advertisement, do not pass. Works, no indication of success
            if (community.Advertisements.Any())
            {
                ModelState.AddModelError(nameof(community.Title), "Community has " + advertisements.Count() + " advertisements, please delete them before deleting the community");
                return View(community);
            }



          
            _context.Communities.Remove(community);


            var memberships = _context.CommunityMemberships.Where(x => x.CommunityId.Equals(id));
            foreach (var item in memberships)
            {
                _context.CommunityMemberships.Remove(item);
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommunityExists(string id)
        {
            return _context.Communities.Any(e => e.Id == id);
        }
    }
}
