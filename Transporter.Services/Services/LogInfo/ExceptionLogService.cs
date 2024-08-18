using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transporter.Common.Helper.AuditLog;

namespace Transporter.Services.Services.Log
{
    public class ExceptionLogService : IExceptionLogService
    {
        //TODO: jafar ulla

        //private readonly ICustomDbContextFactory<LibasLogDBContext> _customDbContextFactory;

        ////private IUnitOfWork<LibasLogDBContext> _unitOfWork;
        //public ExceptionLogService(ICustomDbContextFactory<LibasLogDBContext> unitOfWork)
        //{
        //    _customDbContextFactory = unitOfWork;
        //}
        public async Task SaveExceptionLog(int priority, int moduleID, string exceptionMessege, string exceptionDetail, object objectData, string controllerName, string actionName, int actionType, string managerName)
        {
            try
            {
                // TODO: jafar ulla

                //ExceptionLog exceptionLog = new ExceptionLog();

                //exceptionLog.Priority = priority;
                //exceptionLog.ModuleID = moduleID;
                //exceptionLog.ExceptionMessege = exceptionMessege;
                //exceptionLog.ExceptionDetail = exceptionDetail;
                //exceptionLog.ObjectData = JsonConvert.SerializeObject(objectData).ToString();
                //exceptionLog.ControllerName = controllerName;
                //exceptionLog.ActionName = actionName;
                //exceptionLog.ActionType = actionType;
                //exceptionLog.ManagerName = managerName;
                //exceptionLog.ExceptionTime = DateTime.Now;

                ////await _unitOfWork.Repository<ExceptionLog>().InsertAsync(exceptionLog);
                ////await _unitOfWork.SaveChangesAsync();

                ////using (var dbContext = _customDbContextFactory.CreateDbContext(string.Empty))
                ////{

                ////    await dbContext.ExceptionLog.AddAsync(exceptionLog);

                ////    await dbContext.SaveChangesAsync();
                ////}

            }
            catch (Exception ex)
            {
                //TO DO:Later will write file here
            }


        }
        public async Task SaveExceptionLogs(ExceptionLog exceptionLog)
        {
            try
            {
                //TODO: jafar ulla

                //exceptionLog.ExceptionTime = DateTime.Now;

                //using (var dbContext = _customDbContextFactory.CreateDbContext(string.Empty))
                //{

                //    await dbContext.ExceptionLog.AddAsync(exceptionLog);

                //    await dbContext.SaveChangesAsync();
                //}

                ////await _unitOfWork.Repository<ExceptionLog>().InsertAsync(exceptionLog);
                ////await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                //TO DO:Later will write file here
            }
        }
    }

    public interface IExceptionLogService
    {
        Task SaveExceptionLog(int priority, int moduleID, string exceptionMessege, string exceptionDetail, object objectData, string controllerName, string actionName, int actionType, string managerName);
        Task SaveExceptionLogs(ExceptionLog exceptionLog);
    }
}
