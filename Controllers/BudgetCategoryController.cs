using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPISecurity.Models;

namespace WebAPISecurity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetCategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDataProtector dataProtector;
        private List<BudgetCategory> _list = new List<BudgetCategory>
        {
            new BudgetCategory{ Name = "Budget", Amount = 5000, Id = 1}
        };


        public BudgetCategoryController(IMapper mapper, IDataProtectionProvider dataProtectionProvider)
        {
            var constants = new StringConstants();
            dataProtector = dataProtectionProvider.CreateProtector(constants.IdQryStr);
            _mapper = mapper;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            var result = _mapper.Map<List<BudgetCategoryDto>>(_list);
            return Ok(result);
        }

        [HttpGet("{Id}")]
        public IActionResult Get(string Id)
        {
            var descryptId = int.Parse(dataProtector.Unprotect(Id));
            var item = _list.FirstOrDefault(x => x.Id == descryptId);
            if(item == null) return NotFound();

            var result = _mapper.Map<BudgetCategoryDto>(item);
            return Ok(result);
        }
    }
}
