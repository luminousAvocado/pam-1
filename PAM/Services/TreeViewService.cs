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

            var bureauList = _orgService.GetBureaus();
            var unitList = _orgService.GetUnits();

            //foreach (Bureau item in bureauList)
            //{
            //    Debug.WriteLine(item.BureauId);
            //}
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
