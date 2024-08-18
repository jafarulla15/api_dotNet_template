using Newtonsoft.Json;
using Transporter.Common.Constants;
using Transporter.Common.DTO;
using Transporter.Common.Enums;
using Transporter.Common.Models;
using Transporter.Common.VM;
using Transporter.Services;
using Transporter.Services.Interface;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Transporter.API.Auth
{
#pragma warning disable CS8600
#pragma warning disable CS0436
#pragma warning disable CS8604
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class NSVAuthentication
    {
        private readonly RequestDelegate _next;

        public NSVAuthentication(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IAccessTokenService accessTokenService, ISecurityService securityService)
        {

            string url = httpContext.Request.Path;
            if (url?.ToLower() == CommonPath.loginUrl)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                RequestMessage objRequestMessage = new RequestMessage();
                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                {
                    try
                    {
                        var obj = await reader.ReadToEndAsync();
                        objRequestMessage = JsonConvert.DeserializeObject<RequestMessage>(obj);

                        //for getting user info

                        objResponseMessage = await securityService.Login(objRequestMessage);

                        if (objResponseMessage != null && objResponseMessage.ResponseObj != null)
                        {
                            VMLogin objVMLogin = objResponseMessage.ResponseObj as VMLogin;

                            if (objVMLogin != null)
                            {

                                AccessToken objAccessToken = new AccessToken();
                                objAccessToken.SystemUserID = objVMLogin.SystemUserID;
                                objAccessToken.VendorOrEmployeeId = objVMLogin.VendorOrEmployeeId;
                                objAccessToken.RoleId = objVMLogin.RoleID;
                                objAccessToken.ExpiredOn = DateTime.Now.AddDays(1);
                                objAccessToken.IssuedOn = DateTime.Now;
                                objAccessToken.VendorId = objVMLogin.VendorId;
                                objAccessToken.EmployeeId = objVMLogin.EmployeeId;

                                //     if (existingSystemUser.ReferenceTypeID == (int)Enums.UserReferenceType.VendorAdmin
                                //|| existingSystemUser.ReferenceTypeID == (int)Enums.UserReferenceType.VendorEmployee)
                                //     {
                                //         objVMLogin.EmployeeId = 0;
                                //         objVMLogin.VendorId = existingSystemUser.ReferenceID;
                                //     }
                                //     else
                                //     {
                                //         objVMLogin.EmployeeId = existingSystemUser.ReferenceID;
                                //         objVMLogin.VendorId = 0;
                                //     }
                                //     objVMLogin.VendorOrEmployeeId = existingSystemUser.ReferenceID;

                                //for creating sesssion token
                                AccessToken accessToken = await accessTokenService.Create(objAccessToken);
                                objVMLogin.Token = (accessToken != null) ? accessToken.Token : String.Empty;

                                objVMLogin.SystemUserID = 0;
                                objVMLogin.VendorOrEmployeeId = 0;
                                objVMLogin.RoleID = 0;
                                objResponseMessage.ResponseObj = objVMLogin;
                                objResponseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = false
                                };

                                await httpContext.Response.WriteAsJsonAsync(objResponseMessage, options);
                            }
                            else
                            {
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = false
                                };

                                await httpContext.Response.WriteAsJsonAsync(objResponseMessage, options);
                            }
                        }
                        else
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = false
                            };

                            httpContext.Response.ContentType = "application/json";
                            await httpContext.Response.WriteAsJsonAsync(objResponseMessage, options);
                        }
                    }
                    catch
                    {
                        httpContext.Response.ContentType = "application/json";
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await httpContext.Response.WriteAsJsonAsync(MessageConstant.InternalServerError);
                    }
                }
            }
            else
            {
                await _next(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class NSVAuthenticationExtensions
    {
        public static IApplicationBuilder UseNSVAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NSVAuthentication>();
        }
    }
#pragma warning restore CS8600
#pragma warning restore CS0436
#pragma warning restore CS8604
}
