using System.Net;
using System.Net.Mail;

namespace Protoscend.Services;

// ── ContactFormModel lives in Models/ContactFormModel.cs ──
// Do NOT define it here — that caused the ambiguity errors.

public class EmailSettings
{
    public string SmtpHost { get; set; } = "";
    public int SmtpPort { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromAddress { get; set; } = "";
    public string FromName { get; set; } = "PROTOSCEND Website";
    public string ToAddress { get; set; } = "";
}

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(EmailSettings settings) => _settings = settings;

    public async Task<(bool success, string error)> SendEnquiryAsync(ContactFormModel model)
    {
        try
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            // 1 — Internal notification to Protoscend inbox
            var internalMail = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = $"[PROTOSCEND] New Enquiry — {model.FirstName} {model.LastName}",
                IsBodyHtml = true,
                Body = BuildInternalHtml(model)
            };
            internalMail.To.Add(_settings.ToAddress);
            internalMail.ReplyToList.Add(
                new MailAddress(model.Email, $"{model.FirstName} {model.LastName}"));
            await client.SendMailAsync(internalMail);

            // 2 — Auto-reply to the sender
            var autoReply = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = "We received your message — PROTOSCEND",
                IsBodyHtml = true,
                Body = BuildAutoReplyHtml(model)
            };
            autoReply.To.Add(
                new MailAddress(model.Email, $"{model.FirstName} {model.LastName}"));
            await client.SendMailAsync(autoReply);

            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private static string BuildInternalHtml(ContactFormModel m) => $"""
        <!DOCTYPE html><html><head><meta charset="utf-8"/></head>
        <body style="margin:0;padding:0;background:#1a1a1e;font-family:'Segoe UI',Arial,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0">
            <tr><td align="center" style="padding:40px 20px;">
              <table width="600" cellpadding="0" cellspacing="0"
                     style="background:#222227;border:1px solid rgba(224,32,32,0.3);border-top:3px solid #e02020;">
                <tr><td style="padding:32px 40px 24px;">
                  <p style="margin:0 0 4px;font-size:11px;letter-spacing:3px;color:#e02020;text-transform:uppercase;">NEW ENQUIRY</p>
                  <h1 style="margin:0;font-size:24px;color:#e8e8ec;font-weight:700;letter-spacing:2px;">PROTOSCEND</h1>
                </td></tr>
                <tr><td style="padding:0 40px 32px;">
                  <table width="100%" cellpadding="12" cellspacing="0"
                         style="background:#252529;border:1px solid rgba(255,255,255,0.07);">
                    <tr><td style="color:#606075;font-size:11px;letter-spacing:2px;text-transform:uppercase;width:140px;">Name</td>
                        <td style="color:#e8e8ec;font-size:15px;">{m.FirstName} {m.LastName}</td></tr>
                    <tr><td style="color:#606075;font-size:11px;letter-spacing:2px;text-transform:uppercase;">Email</td>
                        <td><a href="mailto:{m.Email}" style="color:#e02020;font-size:15px;">{m.Email}</a></td></tr>
                    {(!string.IsNullOrWhiteSpace(m.Company) ? $"<tr><td style='color:#606075;font-size:11px;letter-spacing:2px;text-transform:uppercase;'>Company</td><td style='color:#e8e8ec;font-size:15px;'>{m.Company}</td></tr>" : "")}
                    <tr><td style="color:#606075;font-size:11px;letter-spacing:2px;text-transform:uppercase;">Service</td>
                        <td style="color:#e8e8ec;font-size:15px;">{m.Service}</td></tr>
                    <tr><td colspan="2" style="padding-top:16px;">
                      <p style="margin:0 0 8px;color:#606075;font-size:11px;letter-spacing:2px;text-transform:uppercase;">Message</p>
                      <p style="margin:0;color:#e8e8ec;font-size:15px;line-height:1.7;border-left:2px solid #e02020;padding-left:12px;">{m.Message}</p>
                    </td></tr>
                  </table>
                </td></tr>
                <tr><td style="padding:16px 40px 32px;border-top:1px solid rgba(255,255,255,0.07);">
                  <p style="margin:0;font-size:11px;color:#606075;">&copy; 2026 PROTOSCEND (Pty) Ltd | Automated notification.</p>
                </td></tr>
              </table>
            </td></tr>
          </table>
        </body></html>
        """;

    private static string BuildAutoReplyHtml(ContactFormModel m) => $"""
        <!DOCTYPE html><html><head><meta charset="utf-8"/></head>
        <body style="margin:0;padding:0;background:#1a1a1e;font-family:'Segoe UI',Arial,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0">
            <tr><td align="center" style="padding:40px 20px;">
              <table width="600" cellpadding="0" cellspacing="0"
                     style="background:#222227;border:1px solid rgba(224,32,32,0.3);border-top:3px solid #e02020;">
                <tr><td style="padding:32px 40px 24px;">
                  <p style="margin:0 0 4px;font-size:11px;letter-spacing:3px;color:#e02020;text-transform:uppercase;">YOUR TURNING POINT IN SOFTWARE</p>
                  <h1 style="margin:0;font-size:24px;color:#e8e8ec;font-weight:700;letter-spacing:2px;">PROTOSCEND</h1>
                </td></tr>
                <tr><td style="padding:0 40px 32px;">
                  <h2 style="margin:0 0 16px;color:#e8e8ec;font-size:20px;">Hi {m.FirstName}, we've received your message.</h2>
                  <p style="margin:0 0 16px;color:#a0a0b0;font-size:15px;line-height:1.7;">
                    Thank you for reaching out. One of our team members will review your enquiry
                    and get back to you within <strong style="color:#e8e8ec;">1–2 business days</strong>.
                  </p>
                  <table cellpadding="12" cellspacing="0" width="100%"
                         style="background:#252529;border:1px solid rgba(255,255,255,0.07);border-left:3px solid #e02020;margin-bottom:24px;">
                    <tr><td>
                      <p style="margin:0 0 4px;color:#606075;font-size:11px;letter-spacing:2px;text-transform:uppercase;">Reference</p>
                      <p style="margin:0;color:#e8e8ec;font-size:13px;font-family:monospace;">
                        PSC-{DateTime.UtcNow:yyyyMMdd}-{Math.Abs(m.Email.GetHashCode()) % 9999:D4}
                      </p>
                    </td></tr>
                  </table>
                  <p style="margin:0;color:#a0a0b0;font-size:14px;">Warm regards,<br/><strong style="color:#e8e8ec;">The Protoscend Team</strong></p>
                </td></tr>
                <tr><td style="padding:16px 40px 32px;border-top:1px solid rgba(255,255,255,0.07);">
                  <p style="margin:0;font-size:11px;color:#606075;">&copy; 2026 PROTOSCEND (Pty) Ltd | South Africa</p>
                </td></tr>
              </table>
            </td></tr>
          </table>
        </body></html>
        """;
}
