using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsServer.Data;
using PrsServer.Models;

namespace PrsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public RequestsController(PrsDbContext context)
        {
            _context = context;
        }


		//PUT: api/Requests/Approve/5
		#region
		[HttpPut("Approve/{id}")]
        public async Task<IActionResult> SetOrderStatusToApprove(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            request.Status = "Approve";
            return await PutRequest(request.Id, request);
        }

        #endregion

        //PUT: api/Requests/Review/5
        #region
        [HttpPut("Review/{id}")]
        public async Task<IActionResult> SetOrderStatusToReview(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            request.Status = (request.Total <= 50) ?"Approved" :"Review";
            return await PutRequest(request.Id, request);
        }
        #endregion

        //PUT: api/Requests/Reject/5
        #region
        [HttpPut("Reject/{id}")]
        public async Task<IActionResult> SetOrderStatusToReject(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Rejected";      //{request.RejectionReason}

            return await PutRequest(request.Id, request);

        }

        #endregion

        //GET: api/Requests/GetReviews/5
        #region
        [HttpGet("GetReviews/{id}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetReviews(int id)
        {
            return await _context.Requests.Include(u => u.User)
                        .Where(c => c.Status == "Review" && c.UserId != id)
                        .ToListAsync(); }
        #endregion

        // GET: api/Requests
        [HttpGet]
        #region
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _context.Requests.Include(u => u.User)
                .ToListAsync();
        }
        #endregion
        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var request = await _context.Requests
                .Include(u => u.User)
                .Include(r=> r.RequestLines)
                .ThenInclude(p => p.Product)
                .SingleOrDefaultAsync(r =>r.Id==id);

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                //await CalculateTotal(request.RequestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requests
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Request>> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return request;
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
