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
    public class RequestLinesController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public RequestLinesController(PrsDbContext context)
        {
            _context = context;
        }

        //calculate total
        #region
        private async Task<IActionResult> CalculateTotal(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) { return NotFound(); }
            request.Total = _context.RequestLine
                .Where(rl => rl.RequestId == id)
                .Sum(rl => rl.Quantity * rl.Product.Price);
            var rowsAffected = await _context.SaveChangesAsync();
            if (rowsAffected != 1) { throw new Exception("Failed to change request total"); }
            return Ok();
        }
        #endregion

        // GET: api/RequestLines
        #region
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestLine()
        {
            return await _context.RequestLine.Include (p =>p.Product).ToListAsync();
        }
        #endregion

        // GET: api/RequestLines/5
        #region
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine(int id)
        {
            var requestLine = await _context.RequestLine
                .Include(p => p.Product)
                .SingleOrDefaultAsync(r => r.Id==id);

            if (requestLine == null)
            {
                return NotFound();
            }

            return requestLine;
        }
        #endregion

        // PUT: api/RequestLines/5
        #region
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestLine(int id, RequestLine requestLine)
        {
            if (id != requestLine.Id)
            {
                return BadRequest();
            }

            if(requestLine.Quantity < 1)
			{
                throw new Exception(" not allowed to be less than 0");
			}

            _context.Entry(requestLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await CalculateTotal(requestLine.RequestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestLineExists(id))
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
        #endregion

        // POST: api/RequestLines
        #region
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine(RequestLine requestLine)
        {
            if (requestLine.Quantity < 1)
            {
                throw new Exception(" not allowed to be less than 0");
            }
            _context.RequestLine.Add(requestLine);
            await _context.SaveChangesAsync();
            await CalculateTotal(requestLine.RequestId);

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }
        #endregion

        // DELETE: api/RequestLines/5
        #region
        [HttpDelete("{id}")]
        public async Task<ActionResult<RequestLine>> DeleteRequestLine(int id)
        {
            var requestLine = await _context.RequestLine.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            _context.RequestLine.Remove(requestLine);
            await _context.SaveChangesAsync();
            await CalculateTotal(requestLine.RequestId);

            return requestLine;
        }
		#endregion

		private bool RequestLineExists(int id)
        {
            return _context.RequestLine.Any(e => e.Id == id);
        }
    }
}
