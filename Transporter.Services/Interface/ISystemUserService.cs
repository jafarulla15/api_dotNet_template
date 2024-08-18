using Transporter.Common.DTO;
using Transporter.Common.Models;

namespace Transporter.Services.Interface
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface ISystemUserService
    {
        Task<ResponseMessage> GetAllSystemUserIncludingExtendedData(RequestMessage requestMessage);
        Task<ResponseMessage> GetAllSystemUser(RequestMessage requestMessage);
        Task<ResponseMessage> GetSystemUserById(RequestMessage requestMessage);
        Task<ResponseMessage> GetSystemUserByPhoneNo(RequestMessage requestMessage);
        Task<ResponseMessage> GetSystemUserByEmail(RequestMessage requestMessage);
        Task<ResponseMessage> SaveSystemUser(RequestMessage requestMessage);
        Task<ResponseMessage> GetSystemUsersByRole(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteSystemUser(RequestMessage requestMessage);
        Task<ResponseMessage> UpdateProfile(RequestMessage requestMessage);
        Task<ResponseMessage> ApproveSystemUser(RequestMessage requestMessage);
        Task<ResponseMessage> RemoveSystemUser(RequestMessage requestMessage);
        Task<ResponseMessage> ChangePassword(RequestMessage requestMessage);
        Task<SystemUser> GetSystemUserByVendorUserId(int vendorUserID);
        Task<SystemUser> GetSystemUserByEmployeeId(int employeeID);

        Task<bool> CreateUpdateSystemUserForEmployee(EmployeeInformation empObj, int userID, bool isUpdate);
        Task<SystemUser> GetSystemUserByEmail(string email);
        Task<SystemUser> GetSystemUserByVendorUserID(int vendorUserID);
        Task<ResponseMessage> GetAllEmployeeSystemUserIncludingExtendedData(RequestMessage requestMessage);
        Task<ResponseMessage> RejectSystemUser(RequestMessage requestMessage);

    }
}
