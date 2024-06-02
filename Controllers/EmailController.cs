using BlossomServer.Datas.Email;
using BlossomServer.Entities;
using BlossomServer.Services.EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmailController : ControllerBase
	{
		private readonly IEmailService _emailService;

		public EmailController(IEmailService emailService)
		{
			_emailService = emailService;
		}

		[HttpPost]
		public async Task<IActionResult> SendMail()
		{
			var message = new Message(
			new string[] { "ajs71151@vogco.com" },
			"Reset Password",
			$"<html><body>" +
				$"<div style=\"margin:0;padding:0;\">\r\n" +
				$"<div>\r\n" +
				$"<table cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: collapse; table-layout: fixed; border-spacing: 0px; vertical-align: top; min-width: 320px; margin: 0px auto; width: 100%;\">\r\n" +
				$"<tbody>\r\n" +
				$"<tr style=\"vertical-align:top\">\r\n" +
				$"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\">\r\n" +
				$"<div style=\"background-color:transparent\">\r\n" +
				$"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:transparent;word-break:break-word\">\r\n" +
				$"<div style=\"display:table;border-collapse:collapse;width:100%;background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
				$"<tbody><tr style=\"background-color:transparent\">\r\n" +
				$"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:5px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
				$"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
				$"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;min-width:100%\" width=\"100%\">\r\n" +
				$"<tbody>\r\n" +
				$"<tr style=\"vertical-align:top\">\r\n" +
				$"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;padding-right:10px;padding-left:10px;padding-top:10px;padding-bottom:10px;min-width:100%\">\r\n" +
				$"<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"0px\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;border-top:1px solid transparent\" width=\"100%\">\r\n" +
				$"<tbody>\r\n" +
				$"<tr style=\"vertical-align:top\">\r\n" +
				$"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;font-size:0px;line-height:0px\">\r\n\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody>\r\n" +
				$"</table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody>\r\n" +
				$"</table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"<div style=\"background-color:transparent\">\r\n" +
				$"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:transparent;word-break:break-word\">\r\n" +
				$"<div style=\"display:table;border-collapse:collapse;width:100%;background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
				$"<tbody>" +
				$"<tr style=\"background-color:transparent\">\r\n" +
				$"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:0px;padding-bottom:0px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
				$"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
				$"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
				$"<div style=\"padding-top:0px;padding-right:0px;padding-left:0px;padding-bottom:0px\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;min-width:100%\" width=\"100%\">\r\n" +
				$"<tbody>\r\n" +
				$"<tr style=\"vertical-align:top\">\r\n" +
				$"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;padding-right:0px;padding-left:0px;padding-top:0px;padding-bottom:0px;min-width:100%\">\r\n" +
				$"<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"0px\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;border-top:3px solid #a2acb7\" width=\"100%\">\r\n" +
				$"<tbody>\r\n" +
				$"<tr style=\"vertical-align:top\">\r\n" +
				$"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;font-size:0px;line-height:0px\">\r\n" +
				$"\r\n\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody>\r\n" +
				$"</table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody>\r\n" +
				$"</table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n " +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"<div style=\"background-color:transparent\">\r\n" +
				$"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:#ffffff;word-break:break-word\">\r\n" +
				$"<div style=\"display: table; border-collapse: collapse; width: 100%;\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:5px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
				$"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
				$"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
				$"<div align=\"center\" style=\"padding-left:0px;padding-right:0px; padding-top: 20px;\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr style=\"line-height:0px;line-height:0px\">\r\n" +
				$"<td align=\"center\" style=\"padding-right:0px;padding-left:0px\">\r\n" +
				$"<div style=\"font-size:1px;line-height:10px\">\r\n" +
				$"</div>\r\n" +
				$"<img align=\"center\" alt=\"Image\" border=\"0\" src=\"https://blossom-nails-client.vercel.app/logo.png\" style=\"outline: none; clear: both; border: 0px; height: auto; float: none; width: 100%; max-width: 80px; display: block !important;\" title=\"Image\" width=\"80\">\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"<div>\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td style=\"padding-right:10px;padding-left:10px;padding-top:10px;padding-bottom:10px\">\r\n" +
				$"<div style=\"color:#555555;line-height:150%;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n" +
				$"<div style=\"font-size: 12px; line-height: 18px;\">\r\n" +
				$"<p style=\"margin:0;font-size:20px;line-height:21px;text-align:center\">\r\n" +
				$"Blossom Nails\r\n" +
				$"</p>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" + 
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"<div style=\"background-color:transparent\">\r\n" +
				$"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:#ffffff;word-break:break-word\">\r\n" +
				$"<div style=\"display: table; border-collapse: collapse; width: 100%;\">\r\n               " +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n                " +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n                   " +
				$"<tbody><tr>\r\n                    " +
				$"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:0px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
				$"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
				$"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
				$"<div style=\"padding-top:0px;padding-right:0px;padding-left:0px;padding-bottom:5px\">\r\n" +
				$"<div>\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td style=\"padding-right:10px;padding-left:10px;padding-top:10px;padding-bottom:20px\">\r\n" +
				$"<div style=\"color:#0d0d0d;line-height:120%;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n" +
				$"<div style=\"font-size: 12px; line-height: 14px;\">\r\n<p style=\"margin:0;font-size:14px;line-height:17px;text-align:center\">\r\n" +
				$"<span style=\"font-size:36px;line-height:55px\">\r\n" +
				$"<span style=\"color:#a2acb7\">\r\n" +
				$"<strong>\r\n" +
				$"BookingID #28\r\n" +
				$"</strong>\r\n" +
				$"</span>\r\n" +
				$"</span>\r\n" +
				$"</p>\r\n" +
				$"<p style=\"margin:0;font-size:14px;line-height:17px;text-align:center\">\r\n" +
				$"<span style=\"font-size:28px;line-height:33px\">\r\n" +
				$"Thank you for your booking!\r\n" +
				$"<br>\r\n" +
				$"</span>\r\n" +
				$"</p>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"<div align=\"center\" style=\"padding-left:0px;padding-right:0px\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr style=\"line-height:0px;line-height:0px\">\r\n" +
				$"<td align=\"center\" style=\"padding-right:0px;padding-left:0px\">\r\n" +
				$"<img align=\"center\" alt=\"Image\" border=\"0\" src=\"https://ci6.googleusercontent.com/proxy/C84yaqYHHeoHcq2KCRRU3NsMGJwpx8hNhn6BHto3yo04Xpo1IOJIw4y1rIjEhyHX53emw6aqeh-PNk6Kb4SXvsl_Dqrgn2vimQIHkj1On4kTnPa8s-7eTbM=s0-d-e1-ft#https://d1oco4z2z1fhwp.cloudfront.net/templates/default/90/divider.png\" style=\"outline: none; clear: both; border: 0px; height: auto; float: none; width: 100%; max-width: 316px; display: block !important;\" title=\"Image\" width=\"316\">\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"<div>\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td style=\"padding-right:20px;padding-left:20px;padding-top:10px;padding-bottom:10px\">\r\n" +
				$"<div style=\"color:#555555;line-height:150%;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n" +
				$"<div style=\"font-size: 12px; line-height: 18px;\">\r\n" +
				$"<p style=\"margin:0;font-size:14px;line-height:21px;text-align:center\">\r\n" +
				$"Dear [Customer's Name],\r\n" +
				$"</p>\r\n" +
				$"<p style=\"margin:0;font-size:14px;line-height:21px;text-align:center\">\r\n" +
				$"Your appointment is set for [date and time]. Our team is eager to give you a fantastic experience, whether you're after a classic manicure, trendy nail art, or a relaxing pedicure. We're here to make sure you leave feeling pampered and pleased with your nails.\r\n" +
				$"</p>\r\n" +
				$"<p style=\"margin:0;font-size:14px;line-height:21px;text-align:center\">\r\n" +
				$"Thank you once again for choosing Blossom Nails. We can’t wait to welcome you and provide you with an exceptional nail care experience.\r\n" +
				$"</p>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"<div align=\"center\" style=\"padding-left:0px;padding-right:0px; padding-bottom: 20px;\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr style=\"line-height:0px;line-height:0px\">\r\n" +
				$"<td align=\"center\" style=\"padding-right:0px;padding-left:0px\">\r\n" +
				$"<img align=\"center\" alt=\"Image\" border=\"0\" src=\"https://ci6.googleusercontent.com/proxy/C84yaqYHHeoHcq2KCRRU3NsMGJwpx8hNhn6BHto3yo04Xpo1IOJIw4y1rIjEhyHX53emw6aqeh-PNk6Kb4SXvsl_Dqrgn2vimQIHkj1On4kTnPa8s-7eTbM=s0-d-e1-ft#https://d1oco4z2z1fhwp.cloudfront.net/templates/default/90/divider.png\" style=\"outline: none; clear: both; border: 0px; height: auto; float: none; width: 100%; max-width: 316px; display: block !important;\" title=\"Image\" width=\"316\">\r\n                           " +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"<div style=\"background-color:transparent\">\r\n" +
				$"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:#a2acb7;word-break:break-word\">\r\n" +
				$"<div style=\"display: table; border-collapse: collapse; width: 100%;\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:5px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
				$"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
				$"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
				$"<div align=\"center\" style=\"padding-left:0px;padding-right:0px\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr style=\"line-height:0px;line-height:0px\">\r\n" +
				$"<td align=\"center\" style=\"padding-right:0px;padding-left:0px\">\r\n" +
				$"<a href=\"https://gsuite.google.com/u/0/marketplace/app/bee_templates_for_gmail/1023465560860\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=https://gsuite.google.com/u/0/marketplace/app/bee_templates_for_gmail/1023465560860&amp;hl=vi&amp;source=gmail&amp;ust=1716384549768000\">\r\n" +
				$"<img align=\"center\" alt=\"Image\" border=\"0\" src=\"https://ci3.googleusercontent.com/proxy/MAqYmv5quoXEzD1BYreTt0JrJ3uoNY7ctAGV6SL0I25H9_3dWIMOSFb6JYLFnlv555ljKX5FNB9ejSq6KjLUqyUCP15f2uUvObY596nADncHqDpTV8AY141hEDaRH4tRVjYLwdpl-bVeClODRd5pYbodcGlCRhIbQiRpsS-OpSK0w3YKbMmL5iq0ou-D-xk=s0-d-e1-ft#https://d15k2d11r6t6rl.cloudfront.net/public/users/Integrators/BeeProAgency/53601_173071/editor_images/footer_Gmail_w_25.png\" style=\"outline: none; text-decoration-line: none; clear: both; border: none; height: auto; float: none; width: 100%; max-width: 330px; display: block !important;\" title=\"Image\" width=\"330\">\r\n" +
				$"</a>\r\n</td>\r\n                         " +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"<div style=\"background-color:transparent\">\r\n" +
				$"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:transparent;word-break:break-word\">\r\n" +
				$"<div style=\"display:table;border-collapse:collapse;width:100%;background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
				$"<tbody><tr>\r\n" +
				$"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
				$"<tbody><tr style=\"background-color:transparent\">\r\n                    " +
				$"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:5px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
				$"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
				$"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
				$"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;min-width:100%\" width=\"100%\">\r\n" +
				$"<tbody>\r\n" +
				$"<tr style=\"vertical-align:top\">\r\n" +
				$"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;padding-right:10px;padding-left:10px;padding-top:10px;padding-bottom:10px;min-width:100%\">\r\n" +
				$"<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"0px\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;border-top:1px solid transparent\" width=\"100%\">\r\n" +
				$"<tbody>\r\n" +
				$"<tr style=\"vertical-align:top\">\r\n" +
				$"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;font-size:0px;line-height:0px\">\r\n\r\n" +
				$"\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody>\r\n" +
				$"</table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody>\r\n" +
				$"</table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</div>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody></table>\r\n" +
				$"</td>\r\n" +
				$"</tr>\r\n" +
				$"</tbody>\r\n" +
				$"</table>\r\n" +
				$"</div>\r\n" +
				$"</div>" +
			$"</body></html>"
			);

			var result = await _emailService.SendEmail(message);
			return StatusCode(result.Status, result.Message);
		}
	}
}
