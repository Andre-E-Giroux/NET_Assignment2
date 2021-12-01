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
    public class StudentsController : Controller
    {
        private readonly SchoolCommunityContext _context;

        public StudentsController(SchoolCommunityContext context)
        {
            _context = context;
        }

        // GET: Communities
        public async Task<IActionResult> Index(int ID)
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


            foreach (var communityItem in viewModel.Students)
            {
                var membershipsFound = viewModel.CommunityMemberships.Where(x => x.StudentId == communityItem.Id);
                communityItem.Membership = membershipsFound;
            }

            foreach (var studentItem in viewModel.Students)
            {
                var membershipsFound = viewModel.CommunityMemberships.Where(x => x.StudentId == studentItem.Id);
                studentItem.Membership = membershipsFound;
            }

            ViewData["StudentID"] = ID;
           


            return View(viewModel);
        }

        // GET: Communities/Details/5
        public async Task<IActionResult> Details(int id)
        {
            

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
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
        public async Task<IActionResult> Create([Bind("Id,Title,Budget")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: int/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Communities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Budget")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
         

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }








        //added
        // GET: Communities/Edit/5
        public async Task<IActionResult> EditMemberships(int id)
        {
            var viewStudentMembershipModel = new StudentMembershipViewModel();

            viewStudentMembershipModel.Student = await _context.Students.FindAsync(id);

            var commMembership = await _context.Communities
               .Include(i => i.Membership)
               .ToListAsync();


            viewStudentMembershipModel.Memberships = new CommunityMembershipViewModel[] { };

           
            foreach (var item in commMembership)
            {
                var commItem = new CommunityMembershipViewModel();


                commItem.CommunityId = item.Id;
                commItem.Title = item.Title;


                foreach (var itemMembership in item.Membership)
                {
                    if (itemMembership.StudentId == id && itemMembership.CommunityId == item.Id)
                    {
                        commItem.IsMember = true;
                        break;
                    }

                    else
                    {
                        commItem.IsMember = false;
                    }

                }

                viewStudentMembershipModel.Memberships = viewStudentMembershipModel.Memberships.Concat(new CommunityMembershipViewModel[] { commItem });
                
            }


            viewStudentMembershipModel.Memberships = viewStudentMembershipModel.Memberships;



            return View(viewStudentMembershipModel);
        }


        public async Task<IActionResult> AddMembership(int studentId, string communityId)
        {
            if (ModelState.IsValid)
            {
                var membership = new CommunityMembership();

                membership.StudentId = studentId;
                membership.CommunityId = communityId;


                _context.Add(membership);





                var student = await _context.Students.FindAsync(studentId);

                student.Membership.Concat(new CommunityMembership[] { membership });

                _context.Update(student);



                var community = await _context.Communities.FindAsync(communityId);

                community.Membership.Concat(new CommunityMembership[] { membership });

                _context.Update(community);



                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EditMemberships), student);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveMembership(int studentId, string communityId)
        {
            if (ModelState.IsValid)
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


                var student = viewModel.Students.Where(x => x.Id == studentId).Single();

                var community = viewModel.Communities.Where(x => x.Id == communityId).Single();


                var membership = viewModel.CommunityMemberships.Where(x => x.CommunityId == communityId && x.StudentId == studentId).Single();




                _context.Students.Update(student);

                _context.Communities.Update(community);


                _context.CommunityMemberships.Remove(membership);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(EditMemberships), student);
            }
            return RedirectToAction(nameof(Index));


        }


    }
}
