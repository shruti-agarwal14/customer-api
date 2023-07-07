using CustomerCore.Contract;
using CustomerCore.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerWeb.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _repository;

        public CustomerController(ICustomerRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _repository.GetAllAsync();
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            //int maxthread,completion;
            //ThreadPool.GetMaxThreads(out maxthread,out completion);
            //var c = maxthread;
            var customer=  await _repository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();
            return customer;
        }
        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer(Customer customer)
        {
            var newCustomer=await _repository.AddAsync(customer);
            return newCustomer;
           
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(int id,Customer customer)
        {
            if (id != customer.Id)
                return BadRequest();
          return await  _repository.UpdateAsync(customer);
            
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> DeleteCustomer(int id)
        {
            var customer =await _repository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();
            return await _repository.DeleteAsync(customer);
            //return NoContent();
        }
    }
}
