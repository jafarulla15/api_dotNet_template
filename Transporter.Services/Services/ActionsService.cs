using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Transporter.Common.Constants;
using Transporter.Common.DTO;
using Transporter.Common.Enums;
using Transporter.Common.Helper;
using Transporter.Common.Models;
using Transporter.Common.VM;
using Transporter.DataAccess;
using Transporter.Repository;
using Transporter.Services.Interface;
using Transporter.Services.Utils;

namespace Transporter.Services
{
#pragma warning disable CS8600
    public class ActionsService : IActionsService
    {
        private readonly SbDbContext _crmDbContext;
        private readonly IUnitOfWork<SbDbContext> _unitOfWork;

        public ActionsService(IUnitOfWork<SbDbContext> unitOfWork, SbDbContext ctx)
        {
            this._crmDbContext = ctx;
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all action
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllActionsWithPageNameByRoleID(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                string sql = @"select cus.*, mapping.RoleID from
                            (select p.PageID, p.PageName, p.DisplayName, p.Sequence
                            , a.ActionID, a.ActionName, a.ActionNameDisplay, a.Sequence as SequenceAction
                             from Page as p
                            left join Actions as a on p.PageID = a.PageID
                            ) as cus 
                            left join 
                            (
                            select * from RoleActionMapping as rm where rm.RoleID = 4
                            ) mapping on cus.ActionID = mapping.ActionID
                            order by cus.Sequence asc, cus.SequenceAction asc";

                sql = sql.Replace('\\', '%');

                List<VMPageActionWithRole> lstVMPageActionWithRole = _unitOfWork.RawSqlQuery<VMPageActionWithRole>(sql);

                //if (lstCompany != null)
                //{
                //    return new ResponseMessage
                //    {
                //        ResponseCode = (int)Enums.ResponseCode.Success,
                //        Message = "Success",
                //        ResponseObj = lstCompany
                //    };
                //}
                //else
                //{
                //    return new ResponseMessage
                //    {
                //        ResponseCode = (int)Enums.ResponseCode.Success,
                //        Message = "Success",
                //        ResponseObj = null
                //    };
                //}





                //List<Actions> lstActions = new List<Actions>();
                //int totalSkip = 0;
                //totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                //lstActions = await _crmDbContext.Actions.OrderBy(x => x.ActionID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                //responseMessage.ResponseObj = lstActions;
                //responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllActions");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllActions");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }




        public async Task<ResponseMessage> GetAllActions(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Actions> lstActions = new List<Actions>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstActions = await _crmDbContext.Actions.OrderBy(x => x.ActionID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.ResponseObj = lstActions;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllActions");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllActions");
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
        public async Task<ResponseMessage> GetActionsById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Actions objActions = new Actions();
                int ActionsID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objActions = await _crmDbContext.Actions.FirstOrDefaultAsync(x => x.ActionID == ActionsID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objActions;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetActionsById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetActionsById");
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
        public async Task<ResponseMessage> SaveActions(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Actions objActions = JsonConvert.DeserializeObject<Actions>(requestMessage?.RequestObj.ToString());


                if (objActions != null)
                {
                    if (CheckedValidation(objActions, responseMessage))
                    {
                        if (objActions.ActionID > 0)
                        {
                            Actions existingActions = await this._crmDbContext.Actions.AsNoTracking().FirstOrDefaultAsync(x => x.ActionID == objActions.ActionID && x.Status == (int)Enums.Status.Active);
                            if (existingActions != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objActions.CreatedDate = existingActions.CreatedDate;
                                objActions.CreatedBy = existingActions.CreatedBy;
                                objActions.UpdatedDate = DateTime.Now;
                                objActions.UpdatedBy = requestMessage.UserID;
                                _crmDbContext.Actions.Update(objActions);
                            }
                        }
                        else
                        {
                            objActions.Status = (int)Enums.Status.Active;
                            objActions.CreatedDate = DateTime.Now;
                            objActions.CreatedBy = requestMessage.UserID;
                            await _crmDbContext.Actions.AddAsync(objActions);
                        }

                        await _crmDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objActions;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveActions");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveActions");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }


            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objActions"></param>
        /// <returns></returns>
        private bool CheckedValidation(Actions objActions, ResponseMessage responseMessage)
        {
            if (string.IsNullOrEmpty(objActions.ActionName))
            {
                responseMessage.Message = MessageConstant.ActionName;
                return false;
            }

            return true;
        }


#pragma warning restore CS8600


    }

}
