using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PAM.Data;
using PAM.Models;

namespace PAM.Services
{
    public class TreeViewService
    {
        private readonly OrganizationService _orgService;

        public TreeViewService(OrganizationService orgService)
        {
            _orgService = orgService;
            // TEST
            // Use OrganizationService
            Debug.WriteLine("*** TEST ***");
            var temp = _orgService.GetBureaus();
            Debug.WriteLine(temp);
        }
    }

    public class TreeViewNode
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Text { get; set; }
        public List<TreeViewNode> Children { get; set; }
        // Additional treeview properties
    }


}
