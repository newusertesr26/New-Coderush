using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.ViewModels
{
    //view model for changeroles screen
    public class ChangeRoles
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        //public bool IsTodoRegistered { get; set; }
        //public bool IsMembershipRegistered { get; set; }
        //public bool IsRoleRegistered { get; set; }
        //public bool IsDataMasterRegistered { get; set; }
        //public bool IsLeadMasterRegistered { get; set; }
        //public bool IsExpenseMasterRegistered { get; set; }
        //public bool IsCandidateMasterRegistered { get; set; }
        //public bool IsProjectMasterRegistered { get; set; }
        //public bool IsInvoiceMasterRegistered { get; set; }
        //public bool IsLeavecountRegistered { get; set; }
        public bool HR { get; set; }
        public bool Admin { get; set; }
        public bool SuperAdmin { get; set; }
        public bool Employee { get; set; }
        public bool Sales { get; set; }
    }
}
