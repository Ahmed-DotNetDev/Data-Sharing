using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;

namespace FileSharingAPP.Helpers.Mail
{
	public class MailHelper : ImailHelper
	{
		private readonly IConfiguration _config;

		public MailHelper(IConfiguration config)
		{
			this._config = config;
		}
		public void SendMail(InputEmailMessage model)
		{
			using (SmtpClient client = new SmtpClient(_config.GetValue<string>("Mail:Host"), _config.GetValue<int>("Mail:Port")))
			{
				var msg = new MailMessage();
				msg.To.Add(model.Email);
				msg.Subject = model.Subject;
				msg.Body = model.Body;
				msg.IsBodyHtml = true;
				client.Credentials = new System.Net.NetworkCredential(_config.GetValue<string>("Mail:From"), _config.GetValue<string>("Mail:PWD"));
				msg.From = new MailAddress(_config.GetValue<string>("Mail:From"), _config.GetValue<string>("Mail:Sender"), System.Text.Encoding.UTF8);
				try
				{
					client.Send(msg);
				}
				catch (Exception ex)
				{
					// Log or handle the exception here
					// For example:
					Console.WriteLine("Error sending email: " + ex.Message);
				}
			}
		}
	}
}

