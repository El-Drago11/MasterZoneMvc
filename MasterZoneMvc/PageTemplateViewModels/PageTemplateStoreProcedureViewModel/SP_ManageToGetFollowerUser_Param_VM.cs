using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_ManageToGetFollowerUser_Param_VM
    {

        public long Id { get; set; }
        public long FavouriteUserLoginId { get; set; }
        public long FollowingUserLoginId { get; set; }
        public int Mode { get; set; }
    }
}