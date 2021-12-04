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
using Assignment2.Models;

namespace Assignment2.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly SchoolCommunityContext _context;

        public AdvertisementsController(SchoolCommunityContext context)
        {
            _context = context;
        }

        // GET: Advertisements
        public async Task<IActionResult> Index(string id)
        {

            var viewModel = new AdsViewModel();

            var hold = await _context.Communities
                           .Include(i => i.Advertisements)
                           .ToListAsync();

           
            
            //Console.WriteLine("");

            foreach (var communityItem in hold)
            {
               if(communityItem.Id.Equals(id))
                {
                    viewModel.Community = communityItem;
                    break;
                }
            }

            viewModel.Advertisements = await _context.Advertisements.Where(x => x.CommunityAdvertisment.CommunityId == id).ToListAsync();

           


            //ViewData["CommunityID"] = "A1";

           
           

            return View(viewModel);
        }

        // remove
        // GET: Advertisements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.AdvertisementId == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // GET: Advertisements/Create
        public IActionResult Create(string id)
        {


            ViewData["HoldCommunityId"] = id;

            return View();
        }

        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdvertisementId,FileName,Url")] Advertisement advertisement, string communityID)
        {
            var comAdd = new CommunityAdvertisement();

            comAdd.CommunityId = communityID;

            Console.WriteLine(comAdd.CommunityId);
            //comAdd.AdvertismentID = advertisement.AdvertisementId;

            advertisement.CommunityAdvertisment = comAdd;

            // advertisement advertisement are null
            var communities = await _context.Communities.Include(k => k.Advertisements).ToListAsync();//.FindAsync(comAdd.CommunityId);

            Community holdComm = new Community();

            foreach(var item in communities)
            {
                if (item.Id == communityID)
                    holdComm = item;
            }


            holdComm.Advertisements.Concat(new[] { comAdd });

            Console.WriteLine(holdComm.Advertisements);

            if (ModelState.IsValid)
            {
                Console.WriteLine(comAdd.AdvertismentID + " - " + comAdd.CommunityId);
                _context.Advertisements.Add(advertisement);
                _context.CommunityAdvertisements.Add(comAdd);

                _context.Communities.Update(holdComm);

                await _context.SaveChangesAsync();








                //////////

                var viewModel = new AdsViewModel();

                var hold = await _context.Communities
                               .Include(i => i.Advertisements)
                               .ToListAsync();



                //Console.WriteLine("");

                foreach (var communityItem in hold)
                {
                    if (communityItem.Id.Equals(communityID))
                    {
                        viewModel.Community = communityItem;
                        break;
                    }
                }

                viewModel.Advertisements = await _context.Advertisements.Where(x => x.CommunityAdvertisment.CommunityId == communityID).ToListAsync();




                //ViewData["CommunityID"] = "A1";




                return View("Index",viewModel);

            }

            




            return View();
        }

        // GET: Advertisements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdvertisementId,FileName,Url")] Advertisement advertisement)
        {
            if (id != advertisement.AdvertisementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advertisement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvertisementExists(advertisement.AdvertisementId))
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
            return View(advertisement);
        }

        // GET: Advertisements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.AdvertisementId == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);
            _context.Advertisements.Remove(advertisement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvertisementExists(int id)
        {
            return _context.Advertisements.Any(e => e.AdvertisementId == id);
        }
    }
}
