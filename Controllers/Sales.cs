using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using project.Authentication;
using project.Data;
using project.Models;
using project.ViewModel;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Sales : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webhost;
     
        public Sales(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webhost = webHost;
        }
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles =UserRoles.Admin)]
        [HttpGet]
        public IActionResult Get()
        {
            var items = _context.items.ToList();    
            return Ok(items);
        }
        // Insert item and image to database
        [HttpPost]
        public async Task<IActionResult>Insert([FromForm]IFormFile[] ProfileImage, [FromForm] ItemImageVM model)
        {
            //convert items of Text to Json 
            List<items> select = JsonConvert.DeserializeObject<List<items>>(model.items);
            if (select == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Not Found items" });
            }
            else
            {
                foreach (var item in select)
                {
                    //Data injection table item
                    items items = new items
                    {
                        item_Code_No = item.item_Code_No,
                        Name = item.Name,
                        Caliber = item.Caliber,
                        Type = item.Type,
                        price = item.price,
                        Quantity = item.Quantity
                    };
                    //add item to database
                    await _context.items.AddAsync(items);
                    await _context.SaveChangesAsync();

                    if (ProfileImage != null)
                    {
                        foreach (IFormFile img in ProfileImage)
                        {
                            string uniqueFileName = Path.GetFileName(img.FileName);
                            var filePath = Path.Combine(_webhost.WebRootPath, "Images", img.FileName);
                            {
                                var fileSteam = new FileStream(filePath, FileMode.Create);
                                img.CopyTo(fileSteam);
                            }
                            imge e = new imge
                            {
                                ProfilePicture = uniqueFileName,
                                item_Id = items.item_Id
                            };
                            await _context.imges.AddAsync(e);
                            await _context.SaveChangesAsync();
                        }
                    }

                }
            }
            return Ok(new Response { Status = "Success", Message = "Items created successfully!" });
        }

        //Delete item by {Id}
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteItems(int id)
        {
            var items = await _context.items.FirstOrDefaultAsync(x => x.item_Id == id);
            if(items == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Items Not Found" });
            }
           _context.items.Remove(items);
            await _context.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "Items Delete successfully!" });
        }
    }
}
