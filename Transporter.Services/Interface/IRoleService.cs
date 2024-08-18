using Transporter.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transporter.Common.Models;

namespace Transporter.Services.Interface
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface IRoleService
    {
        Task<ResponseMessage> GetAllRole(RequestMessage requestMessage);
        Task<ResponseMessage> SaveRole(RequestMessage requestMessage);
        Task<ResponseMessage> GetRoleById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteRole(RequestMessage requestMessage);
        Task<List<Roles>> GetAllRoles();
    }
}
