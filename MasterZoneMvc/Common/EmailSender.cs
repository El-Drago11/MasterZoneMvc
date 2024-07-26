using MasterZoneMvc.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using static MasterZoneMvc.ViewModels.CourseViewModel;

namespace MasterZoneMvc.Common
{
    public class EmailSender
    {
        string _AppName = ConfigurationManager.AppSettings["AppName"];
        string _SiteURL = ConfigurationManager.AppSettings["SiteURL"];
        string _SMTPEmail = ConfigurationManager.AppSettings["SMTPEmail"];
        string _SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
        bool _TestMode = Convert.ToBoolean(ConfigurationManager.AppSettings["TestMode"]);
        bool _TurnOffEmailSendFromLiveServer = Convert.ToBoolean(ConfigurationManager.AppSettings["TurnOffEmailSendFromLiveServer"]);

        private async Task<string> SendEmailFromExternalEndpointForTesting(string subject, string ToEmail, string msg)
        {
            
            using (var httpClient = new HttpClient())
            {
                
                var formData = new MultipartFormDataContent();
                HttpContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token",  "1ai9CKYK6itsziEARaKRYwndw7K6FLMOw"),
                    new KeyValuePair<string, string>("email_to", ToEmail),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("message", msg)
                });

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var url = "http://phpstack-102119-702427.cloudwaysapps.com/emailapi/";

                using (var response = await httpClient.PostAsync(url, content))
                {
                    string httpResponse = await response.Content.ReadAsStringAsync();

                    if (String.IsNullOrEmpty(httpResponse))
                    {
                        //return View(objLoginModel);
                        return string.Empty;
                    }
                    
                    return httpResponse;
                }

            }

            return string.Empty;
        }

        public void Send(string receiverName, string subject, string ToEmail, string msg, string AttachFileName)
        {
            string testModeMSG = "";

            if (_TestMode == false && _TurnOffEmailSendFromLiveServer == true)
            {
                return;
            }

            if(_TestMode)
            {
                testModeMSG = "<div style='padding:10px 20px; background: #ffefc4; width: 100%; text-align:center;'>This is email received from <b>TestMode</b>.<br/> Actual email will be sent to: <b>" + ToEmail + "</b></div>";
                ToEmail = _SMTPEmail;
            }

            if (AttachFileName == "")
            {
                #region Send withiout Attachment

                
                string bodys = @"<html><body><div>" + testModeMSG + "<p>Hi " + receiverName + @"</p>
                <p style='text-align:justify;'>" + msg + @"
                </p><br>
                <div>
                <span style='border-bottom:1px solid #0070AC; width:100%;Float: left;'><h3 style='border-bottom:2px solid #0070AC; Float:left; margin-bottom:0;'>" + _AppName + @"</h3></span>
                <div style='Float: left !important; margin-right: 10px;background-color:black;width:100%;text-align:center;'>
                </div>
                </div>
                </div></body></html>";

                var response = Task.Run<string>(async () => await SendEmailFromExternalEndpointForTesting(subject, ToEmail, bodys));
                return;

                MailMessage message2 = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                // Enter Email Address here
                MailAddress fromAddress = new MailAddress(_SMTPEmail, _AppName);
                message2.From = fromAddress;
                message2.To.Add(ToEmail);
                message2.Subject = subject;
                message2.IsBodyHtml = true;
                message2.Body = bodys;
                message2.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                message2.Priority = MailPriority.High;
                AlternateView htmlMail = AlternateView.CreateAlternateViewFromString(bodys, null, "text/html");

                //LinkedResource myimage = new LinkedResource(System.Web.HttpContext.Current.Server.MapPath("/Content/images/logo1.png"));
                //myimage.ContentId = "companylogo";
                //htmlMail.LinkedResources.Add(myimage);

                //message2.AlternateViews.Add(htmlMail);
                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment(filePath);
                //message2.Attachments.Add(attachment);

                //smtpClient.Host = "relay-hosting.secureserver.net";   //-- Donot change.
                smtpClient.Host = "smtp.gmail.com";   //-- Donot change.

                smtpClient.Port = 587; //--- Donot change
                smtpClient.EnableSsl = true;//--- Donot change
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(_SMTPEmail, _SMTPPassword);
                smtpClient.Send(message2);
                //return 1;
                #endregion

            }
        }

        public void SendWithTemplate1(string receiverName, string subject, string ToEmail, string msg, string AttachFileName)
        {
            if (AttachFileName == "")
            {
                #region Send withiout Attachment

                string bodys = @"<html>
    <body style='margin:0;'>
        <div style='font-family:sohne,Helvetica Neue,Helvetica,Arial,sans-serif;color:rgba(25,25,25,1);background-color:rgba(242,242,242,1)!important'>
            <table role='presentation' cellpadding='0' cellspacing='0'
                style='margin-left:auto;margin-right:auto;width:100%;max-width:680px;' width='100%'>
                <tbody>
                    <tr>
                        <td style='width:100%;min-width:100%;text-align: center;' width='100%'>
                            <div style='margin: 20px 0;'>
                                <div style='background-color:rgba(255,255,255,1); height: auto; min-height:200px; border-radius:20px; padding: 20px; border: 1px solid #e1c6e1;'>
                                    <div style='border-bottom:1px solid #cecece;'>
                                        <h2 style='margin-top:0px;'><a href='" + _SiteURL + "' style='text-decoration: none; color: purple;'>" + _AppName + @"</a></h2>
                                    </div>
                                    " + msg + @"
                                </div>
                                <div style='text-align: center;color: rgb(146, 143, 143);'>
                                    <p>powered by <a href='https://www.protolabzit.com' style='color: purple;'>Protolabz eServices</a></p>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </body>
</html>
";

                MailMessage message2 = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                // Enter Email Address here
                MailAddress fromAddress = new MailAddress("angulardeveloper.protolabz@gmail.com", _AppName);
                message2.From = fromAddress;
                message2.To.Add(ToEmail);
                message2.Subject = subject;
                message2.IsBodyHtml = true;
                message2.Body = bodys;
                message2.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                message2.Priority = MailPriority.High;
                AlternateView htmlMail = AlternateView.CreateAlternateViewFromString(bodys, null, "text/html");

                //LinkedResource myimage = new LinkedResource(System.Web.HttpContext.Current.Server.MapPath("/Content/images/logo1.png"));
                //myimage.ContentId = "companylogo";
                //htmlMail.LinkedResources.Add(myimage);

                //message2.AlternateViews.Add(htmlMail);
                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment(filePath);
                //message2.Attachments.Add(attachment);

                //smtpClient.Host = "relay-hosting.secureserver.net";   //-- Donot change.
                smtpClient.Host = "smtp.gmail.com";   //-- Donot change.

                smtpClient.Port = 587; //--- Donot change
                smtpClient.EnableSsl = true;//--- Donot change
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("angulardeveloper.protolabz@gmail.com", "zpsidnsjrihznvfd"); // angular, zpsidnsjrihznvfd
                smtpClient.Send(message2);
                //return 1;
                #endregion

            }
        }

        public void SendFromInstitute(string receiverName, string subject, string ToEmail, string msg, string AttachFileName, string InstituteName)
        {
            if (AttachFileName == "")
            {
                #region Send withiout Attachment

                string bodys = @"<html><body><div><p>Hi " + receiverName + @"</p>
                <p style='text-align:justify;'>" + msg + @"
                </p><br>
                <div>
                <span style='border-bottom:1px solid #0070AC; width:100%;Float: left;'><h3 style='border-bottom:2px solid #0070AC; Float:left; margin-bottom:0;'>" + InstituteName + @"</h3></span>
                <div style='Float: left !important; margin-right: 10px;background-color:black;width:100%;text-align:center;'>
                </div>
                </div>
                </div></body></html>";

                MailMessage message2 = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                // Enter Email Address here
                MailAddress fromAddress = new MailAddress("angulardeveloper.protolabz@gmail.com", InstituteName);
                message2.From = fromAddress;
                message2.To.Add(ToEmail);
                message2.Subject = subject;
                message2.IsBodyHtml = true;
                message2.Body = bodys;
                message2.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                message2.Priority = MailPriority.High;
                AlternateView htmlMail = AlternateView.CreateAlternateViewFromString(bodys, null, "text/html");

                //LinkedResource myimage = new LinkedResource(System.Web.HttpContext.Current.Server.MapPath("/Content/images/logo1.png"));
                //myimage.ContentId = "companylogo";
                //htmlMail.LinkedResources.Add(myimage);

                //message2.AlternateViews.Add(htmlMail);
                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment(filePath);
                //message2.Attachments.Add(attachment);

                //smtpClient.Host = "relay-hosting.secureserver.net";   //-- Donot change.
                smtpClient.Host = "smtp.gmail.com";   //-- Donot change.

                smtpClient.Port = 587; //--- Donot change
                smtpClient.EnableSsl = true;//--- Donot change
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("angulardeveloper.protolabz@gmail.com", "zpsidnsjrihznvfd"); // angular, zpsidnsjrihznvfd
                smtpClient.Send(message2);
                //return 1;
                #endregion

            }
        }

        /// <summary>
        /// Generate Event Booked Email-Message Body
        /// </summary>
        /// <param name="_Event">Event Data</param>
        /// <param name="QRCodeImage">QR Code Image Name</param>
        /// <returns>Email Message Body</returns>
        public string EventBookedMailMessage(EventViewModel _Event, string QRCodeImage)
        {

            var _QRTicket = @"<tr>
                <td style='text-align:center;'>
                    <img src='" + _SiteURL + StaticResources.FileUploadPath_QRCodeTicketImage + @"" + QRCodeImage + "' style='height:100px;width:100px'>" +
                @"</td>
            </tr>";
            
            string Message = @"<div><table><tr>
                      <td>
                        <label>Event Title:" + _Event.Title + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>Description:" + _Event.ShortDescription + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>StartDate:" + _Event.StartDate + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>Ticket:" + _QRTicket + @"</label>
                      </td>
                    </tr>
                  </table></div>";

            return Message;
        }

        /// <summary>
        /// Generate Class Booked Email-Message Body
        /// </summary>
        /// <param name="_ClassDetail">Class Data</param>
        /// <param name="ORCodeImage">QR Code Image Name</param>
        /// <returns>Email Message Body</returns>
        public string ClassBookedMailMessage(ClassViewModel _ClassDetail, string ORCodeImage)
        {
            var ModeDetails = "";
            if (_ClassDetail.ClassMode == "Online")
            {
                ModeDetails = @"<tr>
                          <td style='text-align:center;'>
                            <a href='" + _ClassDetail.OnlineClassLink + "' style='padding:10px;background-color:green;color:white;text-decoration:none;'>Click Here</a>" +
                           @"</td>
                        </tr>";
            }
            else if (_ClassDetail.ClassMode == "Offline")
            {
                ModeDetails = @"<tr>
                <td style='text-align:center;'>
                    <img src='" + _SiteURL + StaticResources.FileUploadPath_QRCodeTicketImage + ORCodeImage + "' style='height:100px;width:100px'>" +
                @"</td>
            </tr>";
            }

            string Message = @"<div><table><tr>
                      <td>
                        <label>Name:" + _ClassDetail.Name + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>Description:" + _ClassDetail.Description + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>Class Mode:" + _ClassDetail.ClassMode + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>Class:" + ModeDetails + @"</label>
                      </td>
                    </tr>
                  </table></div>";

            return Message;
        }

        /// <summary>
        /// Generate course Booked Email-Message Body
        /// </summary>
        /// <param name="_ClassDetail">Class Data</param>
        /// <returns>Email Message Body</returns>
        public string CourseBookedMailMessage(CourseDetail_VM _CourseDetail)
        {
            var ModeDetails = "";
            if (_CourseDetail.CourseMode == "Online")
            {
                ModeDetails = @"<tr>
                          <td style='text-align:center;'>
                            <a href='" + _CourseDetail.OnlineCourseLink + "' style='padding:10px;background-color:green;color:white;text-decoration:none;'>Click Here</a>" +
                           @"</td>
                        </tr>";
            }
            //else if (_CourseDetail.CourseMode == "Offline")
            //{
            //    ModeDetails = @"<tr>
            //    <td style='text-align:center;'>
            //        <img src='" + _SiteURL + StaticResources.FileUploadPath_QRCodeTicketImage + ORCodeImage + "' style='height:100px;width:100px'>" +
            //    @"</td>
            //</tr>";
            //}

            string Message = @"<div><table><tr>
                      <td>
                        <label>Name:" + _CourseDetail.Name + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>Description:" + _CourseDetail.Description + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>Class Mode:" + _CourseDetail.CourseMode + @"</label>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <label>Class:" + ModeDetails + @"</label>
                      </td>
                    </tr>
                  </table></div>";

            return Message;
        }
    }
}