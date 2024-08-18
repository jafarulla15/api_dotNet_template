using Microsoft.AspNetCore.Mvc;
using Transporter.Common.DTO;
using Transporter.Services.Interface;
using Transporter.Services;

namespace Transporter.Api.Controllers
{
    [Route("api/Company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyServices _companyService;
        public CompanyController(ICompanyServices userService)
        {
            this._companyService = userService;
        }

        [HttpPost("GetAllData")]
        public async Task<ResponseMessage> GetAllData(RequestMessage requestMessage)
        {
            return await _companyService.GetAllData(requestMessage);
        }

        [HttpPost("GetAll")]
        public async Task<ResponseMessage> GetAllCompany(RequestMessage requestMessage)
        {
            return await _companyService.GetAll(requestMessage);
        }

        [HttpPost("Search")]
        public async Task<ResponseMessage> Search(RequestMessage requestMessage)
        {
            return await _companyService.Search(requestMessage);
        }

        [HttpPost("GetById")]
        public async Task<ResponseMessage> GetCompanyById(RequestMessage requestMessage)
        {
            return await this._companyService.GetById(requestMessage);
        }

        [HttpPost("Save")]
        public async Task<ResponseMessage> Save(RequestMessage requestMessage)
        {
            return await _companyService.Save(requestMessage);
        }

        [HttpPost("Delete")]
        public async Task<ResponseMessage> Delete(RequestMessage requestMessage)
        {
            return await this._companyService.Delete(requestMessage);
        }

        [HttpPost("DeleteSoft")]
        public async Task<ResponseMessage> DeleteSoft(RequestMessage requestMessage)
        {
            return await this._companyService.DeleteSoft(requestMessage);
        }
    }
}
