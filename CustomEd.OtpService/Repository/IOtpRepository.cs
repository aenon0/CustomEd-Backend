namespace CustomEd.OtpService.Repository;

public interface IOtpRepository
{
    public Task Add(Otp otp);

    public Task<bool> ExistsByEmailAddress(string emailAddress);

    public Task<Otp> GetByEmailAddress(string emailAddress);

    public Task Update(Otp otp);
}
