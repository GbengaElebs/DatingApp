using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingapp.api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace datingapp.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;

        }
        [Authorize (Roles= "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values=await _context.Values.ToListAsync();

            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet ("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value=await _context.Values.FirstOrDefaultAsync(x =>x.Id ==id);

            return Ok(value);
        }
    }
}