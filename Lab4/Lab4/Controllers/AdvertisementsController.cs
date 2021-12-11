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
using Azure.Storage.Blobs;
using Azure;
using System.IO;
using Microsoft.AspNetCore.Http;
using Assignment2.Models.ViewModels;

namespace Assignment2.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly SchoolCommunityContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public AdvertisementsController(SchoolCommunityContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
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
                if (communityItem.Id.Equals(id))
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
        /// file input viewmodel only 
        public async Task<IActionResult> Create(IFormFile file, string communityId)
        {
            //if(file == null || )


            FileInputViewModel fileViewModel = new FileInputViewModel();
          

            var communities = await _context.Communities.Include(k => k.Advertisements).ToListAsync();//.FindAsync(comAdd.CommunityId);

            

            foreach (var item in communities)
            {
                if (item.Id == communityId)
                    fileViewModel.CommunityTitle = item.Title;
            }

            fileViewModel.File = file;
            fileViewModel.CommunityId = communityId;
           



            BlobContainerClient containerClient;


            string container = "advertisement";
          

            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(container);
                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(container);
            }


            try
            {
                string filePath = Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(filePath);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                // FILE IS NULL!!!!!

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    // navigate back to the beginning of the memory stream
                    stream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(stream);
                    stream.Close();
                }

                Console.WriteLine(blockBlob.Uri.AbsoluteUri);

                // add the photo to the database if it uploaded successfully
                var advertisement = new Advertisement
                {
                    //AdvertisementId = advertisement.AdvertisementId,
                    Url = blockBlob.Uri.AbsoluteUri,
                    FileName = filePath
                };





                //



               // _context.Advertisements.Add(advertisement);
               // await _context.SaveChangesAsync();









                // _context.Advertisements.Add(image);
                // await _context.SaveChangesAsync();






                //////////////////////////////////



                var comAdvertisement = new CommunityAdvertisement();



                comAdvertisement.CommunityId = fileViewModel.CommunityId;


                //comAdd.AdvertismentID = advertisement.AdvertisementId;

                advertisement.CommunityAdvertisment = comAdvertisement;

                // advertisement advertisement are null

                Community community = new Community();

                foreach (var item in communities)
                {
                    if (item.Id == fileViewModel.CommunityId)
                        community = item;
                }


                community.Advertisements.Concat(new[] { comAdvertisement });

                Console.WriteLine(community.Advertisements);

                if (ModelState.IsValid)
                {
                    Console.WriteLine(comAdvertisement.AdvertismentID + " - " + comAdvertisement.CommunityId);
                    _context.Advertisements.Add(advertisement);
                    _context.CommunityAdvertisements.Add(comAdvertisement);

                    _context.Communities.Update(community);

                    await _context.SaveChangesAsync();


                    var adsViewModel = new AdsViewModel();

                    var hold = await _context.Communities
                                   .Include(i => i.Advertisements)
                                   .ToListAsync();



                    //Console.WriteLine("");

                    foreach (var communityItem in hold)
                    {
                        if (communityItem.Id.Equals(community.Id))
                        {
                            adsViewModel.Community = communityItem;
                            break;
                        }
                    }

                    adsViewModel.Advertisements = await _context.Advertisements.Where(x => x.CommunityAdvertisment.CommunityId == community.Id).ToListAsync();




                    //ViewData["CommunityID"] = "A1";




                    return View("Index", adsViewModel);
                }







                }
            catch (RequestFailedException)
            {
                return RedirectToPage("Error");
            }

            return RedirectToPage("./Index");



            /////////////////////////











            /*




            var comAdvertisement = new CommunityAdvertisement();



            comAdvertisement.CommunityId = communityID;

           
            //comAdd.AdvertismentID = advertisement.AdvertisementId;

            advertisement.CommunityAdvertisment = comAdvertisement;

            // advertisement advertisement are null
            var communities = await _context.Communities.Include(k => k.Advertisements).ToListAsync();//.FindAsync(comAdd.CommunityId);

            Community community = new Community();

            foreach(var item in communities)
            {
                if (item.Id == communityID)
                    community = item;
            }


            community.Advertisements.Concat(new[] { comAdvertisement });

            Console.WriteLine(community.Advertisements);

            if (ModelState.IsValid)
            {
                Console.WriteLine(comAdvertisement.AdvertismentID + " - " + comAdvertisement.CommunityId);
                _context.Advertisements.Add(advertisement);
                _context.CommunityAdvertisements.Add(comAdvertisement);

                _context.Communities.Update(community);

                await _context.SaveChangesAsync();


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
            */
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
