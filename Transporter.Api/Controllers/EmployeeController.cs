using Transporter.Common.DTO;
using Transporter.Services;
using Transporter.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Transporter.API.Controllers
{
    [Route("api/Employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeServices _employeeService;
        public EmployeeController(IEmployeeServices userService)
        {
            this._employeeService = userService;
        }

        [HttpPost("Search")]
        public async Task<ResponseMessage> SearchEmployee(RequestMessage requestMessage)
        {
            return await _employeeService.Search(requestMessage);
        }

        [HttpPost("GetAll")]
        public async Task<ResponseMessage> GetAllEmployee(RequestMessage requestMessage)
        {
            return await _employeeService.GetAll(requestMessage);
        }

        [HttpPost("GetById")]
        public async Task<ResponseMessage> GetEmployeeById(RequestMessage requestMessage)
        {
            return await this._employeeService.GetById(requestMessage);
        }

        [HttpPost("Save")]
        public async Task<ResponseMessage> SaveEmployee(RequestMessage requestMessage)
        {
            return await _employeeService.Save(requestMessage);
        }

        [HttpPost("EmployeeStatusUpdate")]
        public async Task<ResponseMessage> EmployeeStatusUpdate(RequestMessage requestMessage)
        {
            return await _employeeService.EmployeeStatusUpdate(requestMessage);
        }

        [HttpPost("Delete")]
        public async Task<ResponseMessage> DeleteEmployee(RequestMessage requestMessage)
        {
            return await _employeeService.DeleteSoft(requestMessage);
        }
    }
}
