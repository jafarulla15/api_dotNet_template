using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Transporter.Common.Constants;
using Transporter.Common.DTO;
using Transporter.Common.Enums;
using Transporter.Common.Helper;
using Transporter.Common.Models;
using Transporter.Common.VM;
using Transporter.DataAccess;
using Transporter.Services.Interface;
using Transporter.Services.Utils;

namespace Transporter.Services
{
    public class RoleService : IRoleService
    {
        private readonly SbDbContext _sbDbContext;

        public RoleService(SbDbContext ctx)
        {
            this._sbDbContext = ctx;
        }

        /// <summary>
        /// Get all System User
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllRole(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Roles> lstRole = new List<Roles>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstRole = await _sbDbContext.Role.Where(x => x.Status == (int)Enums.Status.Active).OrderBy(x => x.RoleName).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.ResponseObj = lstRole;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllRole");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllRole");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetRoleById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Roles objRole = new Roles();
                int RoleID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objRole = await _sbDbContext.Role.FirstOrDefaultAsync(x => x.RoleID == RoleID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objRole;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetRoleById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetRoleById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update System user
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveRole(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Roles objRole = JsonConvert.DeserializeObject<Roles>(requestMessage?.RequestObj.ToString());



                if (objRole != null)
                {
                    if (CheckedValidation(objRole, responseMessage))
                    {
                        if (objRole.RoleID > 0)
                        {
                            Roles existingRole = await this._sbDbContext.Role.AsNoTracking().FirstOrDefaultAsync(x => x.RoleID == objRole.RoleID && x.Status == (int)Enums.Status.Active);
                            if (existingRole != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objRole.CreatedDate = existingRole.CreatedDate;
                                objRole.CreatedBy = existingRole.CreatedBy;
                                objRole.UpdatedDate = DateTime.Now;
                                objRole.UpdatedBy = requestMessage.UserID;
                                _sbDbContext.Role.Update(objRole);
                            }
                        }
                        else
                        {
                            objRole.Status = (int)Enums.Status.Active;
                            objRole.CreatedDate = DateTime.Now;
                            objRole.CreatedBy = requestMessage.UserID;
                            await _sbDbContext.Role.AddAsync(objRole);

                        }

                        await _sbDbContext.SaveChangesAsync();
                        responseMessage.ResponseObj = objRole;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveRole");

                    }
                    else
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    }
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.SaveFailed;
                }

            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveRole");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update System user
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> DeleteRole(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Delete;
            try
            {

                VMChangeUserRole objVMChangeUserRole = JsonConvert.DeserializeObject<VMChangeUserRole>(requestMessage?.RequestObj.ToString());

                if (objVMChangeUserRole.OldRoleID > 0)
                {
                    if (objVMChangeUserRole.NewRoleID > 0 && objVMChangeUserRole.lstSystemUser.Count > 0)
                    {
                        foreach (SystemUser objSystemUser in objVMChangeUserRole.lstSystemUser)
                        {
                            objSystemUser.Role = objVMChangeUserRole.NewRoleID;
                            _sbDbContext.SystemUser.Update(objSystemUser);
                        }
                    }
                    Roles objRole = await _sbDbContext.Role.Where(x => x.RoleID == objVMChangeUserRole.OldRoleID).FirstOrDefaultAsync();
                    if (objRole != null)
                    {
                        objRole.Status = (int)Enums.Status.Delete;
                        _sbDbContext.Role.Update(objRole);
                    }

                    await _sbDbContext.SaveChangesAsync();

                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.DeleteFailed;
                }

            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteRole");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objRole"></param>
        /// <returns></returns>
        private bool CheckedValidation(Roles objRole, ResponseMessage responseMessage)
        {
            if (string.IsNullOrEmpty(objRole.RoleName))
            {
                responseMessage.Message = MessageConstant.RoleName;
                return false;
            }

            Roles existingRole = _sbDbContext.Role.Where(x => x.RoleName == objRole.RoleName
                                    && x.RoleID != objRole.RoleID
                                    && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingRole != null)
            {
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                responseMessage.Message = MessageConstant.DuplicateRoleName;
                return false;
            }

            return true;
        }


        public async Task<List<Roles>> GetAllRoles() {
            List<Roles> lstRole  = await _sbDbContext.Role.Where(x => x.Status == (int)Enums.Status.Active).OrderBy(x => x.RoleName).ToListAsync();
            return lstRole;
        }



    }
}
