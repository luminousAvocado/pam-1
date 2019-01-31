using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Models;

namespace PAM.Services
{
    public class TreeViewService
    {
        private readonly OrganizationService _orgService;
        private List<TreeViewNode> unitList = new List<TreeViewNode>();

        public TreeViewService(OrganizationService orgService)
        {
            _orgService = orgService;

            var bureauList = _orgService.GetBureaus();
            var unitList = _orgService.GetUnits();

            //var bureauTree = CreateBureaus(bureauList);
            var unitTree = CreateUnits(unitList);
            Debug.WriteLine("*** START ***");

            foreach(TreeViewNode item in unitTree)
            {
                Debug.WriteLine("*** NEW ***");
                Debug.WriteLine(item.Text);
                Debug.WriteLine(item.Children);
            }
        }

        public List<TreeViewNode> CreateBureaus(ICollection<Bureau> list)
        {
            List<TreeViewNode> bureauList = new List<TreeViewNode>();

            foreach(Bureau bureau in list)
            {
                TreeViewNode temp = new TreeViewNode
                {
                    Id = bureau.BureauId,
                    Type = "Bureau",
                    Text = bureau.Description
                };
                bureauList.Add(temp);
            }
            return bureauList;
        }

        // Create Unit with hieararchy and then later another method to add the parent unit as children to bureau
        public List<TreeViewNode> CreateUnits(ICollection<Unit> list)
        {
            Debug.WriteLine(list.Count);
            if(list.Count == 0)
            {
                return unitList;
            }
            foreach(Unit unit in list.ToList())
            {
                if(unit.ParentId != null)
                {
                    TreeViewNode parent = unitList.Find(x => x.Id == unit.ParentId);
                    if(parent != null)
                    {
                        TreeViewNode temp = new TreeViewNode
                        {
                            Id = unit.UnitId,
                            Type = "Unit",
                            Text = unit.Name,
                            Children = new List<TreeViewNode>()
                        };
                        parent.Children.Add(temp);
                        list.Remove(unit);
                    }
                }
                else
                {
                    TreeViewNode temp = new TreeViewNode
                    {
                        Id = unit.UnitId,
                        Type = "Unit",
                        Text = unit.Name,
                        Children = new List<TreeViewNode>()
                    };
                    unitList.Add(temp);
                    list.Remove(unit);
                }
            }

            return CreateUnits(list);
        }
    }

    public class TreeViewNode
    {
        public int Id { get; set; }
        public string Type { get; set; } // Signifies Unit or Bureau
        public string Text { get; set; }
        public List<TreeViewNode> Children { get; set; }
        // Additional treeview properties
    }


}
