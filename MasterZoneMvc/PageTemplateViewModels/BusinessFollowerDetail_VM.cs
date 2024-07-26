using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessFollowerDetail_VM
    {

        public long Id { get; set; }
        public int TotalCountFollowing { get; set; }
        public long FavouriteUserLoginId { get; set; }
        public long FollowingUserLoginId { get; set; }
        public int TotalCountFollower { get; set; }
        public int Mode { get; set; }
    }
}