using Newtonsoft.Json;
using Transporter.Common.Constants;
using Transporter.Common.DTO;
using Transporter.Common.Models;
using Transporter.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace Transporter.API.Auth
{
#pragma warning disable CS8604
#pragma warning disable CS8600
#pragma warning disable CS8602
    public class NSVAuthorization
    {
        private readonly RequestDelegate _next;

        public NSVAuthorization(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IUserSessionService userSessionService, ISecurityService securityService)
        {
            string url = httpContext.Request.Path;

            if (url?.ToLower() == CommonPath.registerUrl.ToLower()
                || url?.ToLower() == CommonPath.registerUrl_Vendor.ToLower()
                 || url?.ToLower() == CommonPath.testUrl.ToLower()
                ) // True here to bypass login //registerUrl_Vendor  // Test/get
            {
                await _next(httpContext);
            }
            else
            {
                var handler = new JwtSecurityTokenHandler();
                var headerToken = SubstringToken(httpContext.Request.Headers[HttpHeaders.Token]);
                var token = handler.ReadToken(headerToken) as JwtSecurityToken;
                if (token != null)
                {
                    RequestMessage objRequestMessage = new RequestMessage();
                    ResponseMessage objResponseMessage = new ResponseMessage();

                    objRequestMessage.RequestObj = GetSystemUserId(token);
                    objResponseMessage = await userSessionService.GetUserSessionBySystemUserId(objRequestMessage);
                    try
                    {

                        UserSession objUserSession = objResponseMessage?.ResponseObj as UserSession;

                        if (objUserSession != null)
                        {
                            TimeSpan ts = DateTime.Now - objUserSession.SessionEnd.Value;
                            int min = ts.Minutes;
                            if (min <= CommonConstant.SessionExpired)
                            {
                                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                                {
                                    var obj = await reader.ReadToEndAsync();

                                    objRequestMessage = JsonConvert.DeserializeObject<RequestMessage>(obj);

                                    objRequestMessage.UserID = GetSystemUserId(token);
                                    try {
                                        objRequestMessage.VendorId = GetVendorId(token);
                                    }
                                    catch (Exception ex)
                                    { 
                                    // handle here
                                    }

                                    try
                                    {
                                        objRequestMessage.EmployeeId = GetEmployeeId(token);
                                    }
                                    catch (Exception ex)
                                    {
                                        // handle here
                                    }

                                    try
                                    {
                                        objRequestMessage.VendorOrEmployeeId = GetVendorOrEmployeeId(token);
                                    }
                                    catch (Exception ex)
                                    {
                                        // handle here
                                    }
                                }

                                // for set user id in request boby.                            
                                var requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objRequestMessage));
                                httpContext.Request.Body = new MemoryStream(requestData);
                                httpContext.Request.ContentLength = httpContext.Request.Body.Length;


                                if (await securityService.CheckPermission(url))
                                {

                                    //update session
                                    RequestMessage objRequestMessageNew = new RequestMessage();

                                    DateTime dateTime = DateTime.Now.AddMinutes(CommonConstant.SessionExpired);
                                    objUserSession.SessionEnd = dateTime;
                                    objRequestMessageNew.RequestObj = JsonConvert.SerializeObject(objUserSession);

                                    await userSessionService.SaveUserSession(objRequestMessageNew);
                                    await _next(httpContext);
                                }
                                else
                                {
                                    httpContext.Response.ContentType = "application/json";
                                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    await httpContext.Response.WriteAsJsonAsync("You have no permission.");
                                }

                            }
                            else
                            {
                                httpContext.Response.ContentType = "application/json";
                                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                await httpContext.Response.WriteAsJsonAsync("Session expired.");

                            }
                        }
                        else
                        {
                            httpContext.Response.ContentType = "application/json";
                            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            await httpContext.Response.WriteAsJsonAsync("Any active session not found.");
                        }
                    }
                    catch
                    {
                        httpContext.Response.ContentType = "application/json";
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await httpContext.Response.WriteAsJsonAsync("Internal server error.");
                    }
                }
                else
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync("Unauthorize");
                }
            }

        }

        private string SubstringToken(string fullToken)
        {
            return fullToken.Replace("Bearer ", "");
        }
        /// <summary>
        /// get user id from token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private int GetSystemUserId(JwtSecurityToken token)
        {
            return Convert.ToInt32(token.Claims.First(claim => claim.Type == JwtClaim.UserId).Value);
        }

        private int GetVendorId(JwtSecurityToken token)
        {
            return Convert.ToInt32(token.Claims.First(claim => claim.Type == JwtClaim.VendorId).Value);
        }

        private int GetEmployeeId(JwtSecurityToken token)
        {
            return Convert.ToInt32(token.Claims.First(claim => claim.Type == JwtClaim.EmployeeId).Value);
        }
        private int GetVendorOrEmployeeId(JwtSecurityToken token)
        {
            return Convert.ToInt32(token.Claims.First(claim => claim.Type == JwtClaim.VendorOrEmployeeId).Value);
        }
    } 

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class NSVAuthorizationExtensions
    {
        public static IApplicationBuilder UseNSVAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NSVAuthorization>();
        }
    }
#pragma warning restore CS8604
#pragma warning restore CS8600
#pragma warning restore CS8602
}
