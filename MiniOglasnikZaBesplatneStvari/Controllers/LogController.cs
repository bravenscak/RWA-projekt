﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniOglasnikZaBesplatneStvari.Models;

namespace MiniOglasnikZaBesplatneStvari.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly AdvertisementRwaContext _context;

        public LogController(AdvertisementRwaContext context)
        {
            _context = context;
        }

        // GET: api/logs/get/N 
        [HttpGet("get/{N:int?}")]
        public ActionResult<IEnumerable<Log>> GetLogs(int N = 10)
        {
            try
            {
                var logs = _context.Logs.Take(N).ToList();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/logs/count
        [HttpGet("count")]
        public ActionResult<int> GetLogCount()
        {
            try
            {
                var count = _context.Logs.Count();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}