namespace testPronia.Interfaces
{
    public interface IEmailService
    {
        Task SendMailAsync(string mailTo, string subject, string body, bool isHTML);
        
    }
}
