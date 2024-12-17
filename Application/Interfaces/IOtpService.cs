namespace Application.Interfaces;

public interface IOtpService 
{
    Task<bool> SendSms(string mobileNumber, CancellationToken cancellationToken);
}
