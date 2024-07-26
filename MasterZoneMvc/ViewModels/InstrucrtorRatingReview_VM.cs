using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class InstrucrtorRatingReview_VM
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public int Rating { get; set; }
        public string ReviewBody { get; set; }
        public int Mode { get; set; }
        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = "Invalid Data!";
            }

            StringBuilder sb = new StringBuilder();

            if (Rating <= 0) { sb.AppendLine(Resources.VisitorPanel.RatingRequired); vm.Valid = false; }

            if (String.IsNullOrEmpty(ReviewBody)) { sb.AppendLine(Resources.VisitorPanel.ReviewBodyRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = Resources.BusinessPanel.ErrorMessage + sb.ToString();
            }

            return vm;
        }
    }
}