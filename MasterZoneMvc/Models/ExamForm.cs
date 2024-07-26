using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ExamForm
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessMasterId { get; set; }
        public long BusinessId { get; set; }
        public long CenterNo { get; set; }
        public string Title { get; set; }
        public string EstablishedYear { get; set; }
        public string BusinessLogo { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }
        public string SecretaryNumber { get; set; }
        public string RegistrarOfficeNumber { get; set; }
        public string Email { get; set; }
        public string WebsiteLink { get; set; }
        public string ImportantInstruction { get; set; }
        public string ExamFormLogo { get; set; }
        public string NameWithAddress { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public int Status { get; set; }
        public DateTime DeletedOn { get; set; }

    }
}