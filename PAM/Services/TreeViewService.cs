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
        private List<TreeViewNode> MyTree = new List<TreeViewNode>();
        private Dictionary<int, TreeViewNode> bureauDictionary = new Dictionary<int, TreeViewNode>();
        private Dictionary<int, TreeViewNode> unitDictionary = new Dictionary<int, TreeViewNode>();

        public TreeViewService(OrganizationService orgService)
        {
            _orgService = orgService;

            var bureauList = _orgService.GetBureaus();
            var unitList = _orgService.GetUnits();

            // Create toplevel tree with bureaus
            MyTree = CreateBureaus(bureauList);
            CreateUnits(unitList);


            foreach(TreeViewNode temp in MyTree)
            {
                Debug.WriteLine("*** NEW ***");
                Debug.WriteLine(temp.Children.Count);
            }
            //foreach(TreeViewNode item in unitTree)
            //{
            //    Debug.WriteLine("*** NEW ***");
            //    Debug.WriteLine(item.Text);
            //    Debug.WriteLine(item.Children);
            //}
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
                    Text = bureau.Description,
                    Children = new List<TreeViewNode>()
                };
                bureauList.Add(temp);
                bureauDictionary.Add(temp.Id, temp);
            }
            
            return bureauList;
        }

        // Create Unit with hieararchy and then later another method to add the parent unit as children to bureau
        public void CreateUnits(ICollection<Unit> unitList)
        {
            foreach(Unit unit in unitList)
            {
                if(unit.ParentId == null)
                {
                    TreeViewNode parent = bureauDictionary[unit.BureauId];
                    TreeViewNode temp = new TreeViewNode
                    {
                        Id = unit.UnitId,
                        Type = "Unit",
                        Text = unit.Name,
                        Children = new List<TreeViewNode>()
                    };
                    parent.Children.Add(temp);
                    unitDictionary.Add(temp.Id, temp);
                }
            }
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
