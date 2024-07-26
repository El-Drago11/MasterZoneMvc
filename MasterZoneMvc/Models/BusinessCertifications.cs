namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Certificate Profiles assigned to Businesses by SuperAdmin
    /// </summary>
    public class BusinessCertifications
    {
        public long CertificateId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
    }

}