using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Transporter.Common.Constants;
using Transporter.Common.DTO;
using Transporter.Common.Enums;
using Transporter.Common.Helper;
using Transporter.Common.Models;
using Transporter.DataAccess;
using Transporter.Services.Interface;
using Transporter.Services.Utils;

namespace Transporter.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly SbDbContext _crmDbContext;

        public PermissionService(SbDbContext ctx)
        {
            this._crmDbContext = ctx;
        }

        /// <summary>
        /// Get all Permission
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllPermission(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Page> lstPermission = new List<Page>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstPermission = await _crmDbContext.Page.Where(x => x.Sequence != 0 && x.Status != (int)Enums.Status.Delete).OrderBy(x => x.Sequence)
                        .Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();

                responseMessage.TotalCount = lstPermission.Count;
                responseMessage.ResponseObj = lstPermission;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllPermission");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllPermission");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// get permission by id
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetPermissionById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Page objPermission = new Page();
                int PermissionID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objPermission = await _crmDbContext.Page.FirstOrDefaultAsync(x => x.PageID == PermissionID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objPermission;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetPermissionById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetPermissionById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// </summary>
        /// get permission by role id
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetAllPermissionRelateWithRole(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                //List<Permission> lstPermission = new List<Permission>();
                dynamic roleObj = JsonConvert.DeserializeObject<dynamic>(requestMessage?.RequestObj.ToString());
                int roleID = roleObj.roleID;

                var result = (from permission in _crmDbContext.Page.Where(p => p.Status == (int)Enums.Status.Active)
                              join mapping in _crmDbContext.RolePermissionMapping.Where(rm => rm.RoleID == roleID)
                              on permission.PageID equals mapping.PermissionID
                              into Details
                              from m in Details.DefaultIfEmpty()
                              where m.RoleID == roleID || m == null
                              select new
                              {
                                  PageID = permission.PageID,
                                  PermissionName = permission.PageName,
                                  Sequence = permission.Sequence,
                                  DisplayName = permission.DisplayName,
                                  RoleID = m == null ? 0 : m.RoleID,
                              }).ToList();



                //objPermission = await _crmDbContext.Permission.FirstOrDefaultAsync(x => x.role == PermissionID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = result;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetPermissionByRoleId");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetPermissionByRoleId");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        public async Task<ResponseMessage> UpdateRolePermissionMapping(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                dynamic roleObj = JsonConvert.DeserializeObject<dynamic>(requestMessage?.RequestObj.ToString());
                int roleID = roleObj.roleID;
                int permissionID = roleObj.permissionID;

                List<RolePermissionMapping> lstRolePermission = new List<RolePermissionMapping>();
                lstRolePermission = await _crmDbContext.RolePermissionMapping.Where(x => x.PermissionID == permissionID && x.RoleID == roleID).ToListAsync();
                if (lstRolePermission != null && lstRolePermission.Count > 0)
                {
                    // exist, Need to remove mapping
                    _crmDbContext.RolePermissionMapping.RemoveRange(lstRolePermission);
                }
                else
                {
                    // Not Exist, Need to Insert mappint
                    RolePermissionMapping rpMapping = new RolePermissionMapping();
                    rpMapping.RoleID = roleID;
                    rpMapping.PermissionID = permissionID;
                    _crmDbContext.RolePermissionMapping.Add(rpMapping);
                }
                await _crmDbContext.SaveChangesAsync();
                responseMessage.ResponseObj = roleObj;
                responseMessage.Message = MessageConstant.SavedSuccessfully;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Insert, requestMessage.UserID, "RolePermission_AddEdit");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "RolePermission_AddEdit");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        public async Task<ResponseMessage> GetPermissionByRoleId(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                //List<Permission> lstPermission = new List<Permission>();
                int roleID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                var lstPermissionByRole = _crmDbContext.Page
                                        .Join(_crmDbContext.RolePermissionMapping,
                                        p => p.PageID,
                                        m => m.PermissionID,
                                        (p, m) => new
                                        {
                                            PermissionID = p.PageID,
                                            PermissionName = p.PageName,
                                            DisplayName = p.DisplayName,
                                            Sequence = p.Sequence,
                                            RoleID = m.RoleID,
                                            Status = p.Status
                                        }).
                                        Where(a => a.RoleID == roleID && a.Status == (int)Enums.Status.Active);

                //objPermission = await _crmDbContext.Permission.FirstOrDefaultAsync(x => x.role == PermissionID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = lstPermissionByRole;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetPermissionByRoleId");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetPermissionByRoleId");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Permission
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SavePermission(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Page objPermission = JsonConvert.DeserializeObject<Page>(requestMessage?.RequestObj.ToString());

                if (objPermission != null)
                {
                    if (CheckedValidation(objPermission, responseMessage))
                    {
                        if (objPermission.PageID > 0)
                        {
                            Page existingPermission = await this._crmDbContext.Page.AsNoTracking().FirstOrDefaultAsync(x => x.PageID == objPermission.PageID && x.Status != (int)Enums.Status.Delete);
                            if (existingPermission != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objPermission.CreatedDate = existingPermission.CreatedDate;
                                objPermission.CreatedBy = existingPermission.CreatedBy;
                                objPermission.UpdatedDate = DateTime.Now;
                                objPermission.UpdatedBy = requestMessage.UserID;
                                _crmDbContext.Page.Update(objPermission);
                            }
                        }
                        else
                        {
                            List<Page> lstPermission = await _crmDbContext.Page.Where(x => x.Status == (int)Enums.Status.Active).OrderByDescending(x => x.PageID).ToListAsync();
                            //int maxSequence = await _crmDbContext.Permission.MaxAsync(x => x.Sequence);

                            if (lstPermission.Count > 0)
                            {
                                objPermission.Sequence = lstPermission[0].Sequence + 1;
                            }
                            else
                            {
                                objPermission.Sequence = 1;
                            }
                            //objPermission.Sequence = maxSequence + 1;

                            objPermission.Status = (int)Enums.Status.Active;
                            objPermission.CreatedDate = DateTime.Now;
                            objPermission.CreatedBy = requestMessage.UserID;
                            await _crmDbContext.Page.AddAsync(objPermission);

                        }
                        await _crmDbContext.SaveChangesAsync();
                        responseMessage.ResponseObj = objPermission;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SavePermission");

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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SavePermission");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }
            return responseMessage;
        }


        /// <summary>
        /// re sequence the permissions
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SequencePermissions(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;
            try
            {
                List<Page> lstPermission = JsonConvert.DeserializeObject<List<Page>>(requestMessage?.RequestObj.ToString());
                List<Page> lstSequencialPermission = new List<Page>();

                if (lstPermission.Count > 0)
                {
                    foreach (Page objPermission in lstPermission)
                    {
                        Page existingPermission = await _crmDbContext.Page.AsNoTracking().
                            FirstOrDefaultAsync(x => x.PageID == objPermission.PageID && x.Status != (int)Enums.Status.Delete);

                        if (existingPermission != null)
                        {
                            actionType = (int)Enums.ActionType.Update;
                            objPermission.CreatedDate = existingPermission.CreatedDate;
                            objPermission.CreatedBy = existingPermission.CreatedBy;
                            objPermission.UpdatedDate = DateTime.Now;
                            objPermission.UpdatedBy = requestMessage.UserID;
                            _crmDbContext.Page.Update(objPermission);
                        }
                    }
                    await _crmDbContext.SaveChangesAsync();

                    lstSequencialPermission = await _crmDbContext.Page.Where(x => x.Sequence != 0 && x.Status != (int)Enums.Status.Delete).OrderBy(x => x.Sequence).ToListAsync();

                    responseMessage.ResponseObj = lstSequencialPermission;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.SavedSuccessfully;
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SavePermission");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objPermission"></param>
        /// <returns></returns>
        private bool CheckedValidation(Page objPermission, ResponseMessage responseMessage)
        {
            if (string.IsNullOrEmpty(objPermission.PageName))
            {
                responseMessage.Message = MessageConstant.PermissionName;
                return false;
            }

            Page existingPermission = _crmDbContext.Page.Where(x => x.PageName == objPermission.PageName && x.PageID != objPermission.PageID
         && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingPermission != null)
            {
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                responseMessage.Message = MessageConstant.DuplicatePermissionName;
                return false;
            }

            return true;
        }
    }
}
