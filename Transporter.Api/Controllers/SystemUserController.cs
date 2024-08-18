using Transporter.Common.DTO;
using Transporter.Services;
using Transporter.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Transporter.API.Controllers
{
    [Route("api/SystemUser")]
    [ApiController]
    public class SystemUserController : ControllerBase
    {
        private readonly ISystemUserService _userService;

        public SystemUserController(ISystemUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost("ApproveSystemUser")]
        public async Task<ResponseMessage> ApproveSystemUser(RequestMessage requestMessage)
        {
            return await this._userService.ApproveSystemUser(requestMessage);
        }

        [HttpPost("RejectSystemUser")]
        public async Task<ResponseMessage> RejectSystemUser(RequestMessage requestMessage)
        {
            return await this._userService.RejectSystemUser(requestMessage);
        }

        [HttpPost("RemoveSystemUser")]
        public async Task<ResponseMessage> RemoveSystemUser(RequestMessage requestMessage)
        {
            return await this._userService.RemoveSystemUser(requestMessage);
        }

        [HttpPost("GetAllSystemUser")]
        public async Task<ResponseMessage> GetAllSystemUsers(RequestMessage requestMessage)
        {
            return await this._userService.GetAllSystemUser(requestMessage);
        }

        [HttpPost("GetAllEmployeeSystemUserIncludingExtendedData")]
        public async Task<ResponseMessage> GetAllEmployeeSystemUserIncludingExtendedData(RequestMessage requestMessage)
        {
            return await this._userService.GetAllEmployeeSystemUserIncludingExtendedData(requestMessage);
        }

        [HttpPost("GetAllSystemUserIncludingExtendedDataWithSearch")]
        public async Task<ResponseMessage> GetAllSystemUserIncludingExtendedDataWithSearch(RequestMessage requestMessage)
        {
            return await this._userService.GetAllSystemUserIncludingExtendedData(requestMessage);
        }

        [HttpPost("GetAllSystemUserIncludingExtendedData")]
        public async Task<ResponseMessage> GetAllSystemUserIncludingExtendedData(RequestMessage requestMessage)
        {
            return await this._userService.GetAllSystemUserIncludingExtendedData(requestMessage);
        }

        [HttpPost("GetSystemUserById")]
        public async Task<ResponseMessage> GetSystemUserById(RequestMessage requestMessage)
        {
            return await this._userService.GetSystemUserById(requestMessage);
        }

        [HttpPost("GetSystemUserByPhoneNo")]
        public async Task<ResponseMessage> GetSystemUserByPhoneNo(RequestMessage requestMessage)
        {
            return await this._userService.GetSystemUserByPhoneNo(requestMessage);
        }

        [HttpPost("GetSystemUsersByRole")]
        public async Task<ResponseMessage> GetSystemUserByRole(RequestMessage requestMessage)
        {
            return await this._userService.GetSystemUsersByRole(requestMessage);
        }

        [HttpPost("registerSystemUser")]
        public async Task<ResponseMessage> registerSystemUser(RequestMessage requestMessage)
        {
            return await this._userService.SaveSystemUser(requestMessage);
        }

        [HttpPost("updateSystemUser")]
        public async Task<ResponseMessage> updateSystemUser(RequestMessage requestMessage)
        {
            return await this._userService.SaveSystemUser(requestMessage);
        }

        [HttpPost("profileUpdate")]
        public async Task<ResponseMessage> UpdateProfile(RequestMessage requestMessage)
        {
            return await this._userService.UpdateProfile(requestMessage);
        }

        [HttpPost("GetSystemUserByEmail")]
        public async Task<ResponseMessage> GetSystemUserByEmail(RequestMessage requestMessage)
        {
            return await this._userService.GetSystemUserByEmail(requestMessage);
        }

        [HttpPost("DeleteSystemUser")]
        public async Task<ResponseMessage> DeleteSystemUser(RequestMessage requestMessage)
        {
            return await this._userService.DeleteSystemUser(requestMessage);
        }

        [HttpPost("ChangePassword")]
        public async Task<ResponseMessage> ChangePassword(RequestMessage requestMessage)
        {
            return await this._userService.ChangePassword(requestMessage);
        }
    }
}
