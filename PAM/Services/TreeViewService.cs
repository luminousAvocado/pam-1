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
        }

        public List<TreeViewNode> GenerateTree()
        {
            var bureauList = _orgService.GetBureaus();
            var unitList = _orgService.GetUnits();

            MyTree = CreateBureaus(bureauList);
            CreateUnits(unitList);

            return MyTree;
        }

        public List<TreeViewNode> CreateBureaus(ICollection<Bureau> list)
        {
            List<TreeViewNode> bureauList = new List<TreeViewNode>();

            foreach(Bureau bureau in list)
            {
                TreeViewNode temp = new TreeViewNode
                {
                    id = bureau.BureauId,
                    type = "Bureau",
                    text = bureau.Description,
                    nodes = new List<TreeViewNode>()
                };
                bureauList.Add(temp);
                bureauDictionary.Add(temp.id, temp);
            }            
            return bureauList;
        }

        public int CreateUnits(ICollection<Unit> unitList)
        {
            if(unitList.Count == 0)
            {
                return 1;
            }
            else
            {
                foreach (Unit unit in unitList.ToList())
                {
                    if (unit.ParentId == null)
                    {
                        TreeViewNode parent = bureauDictionary[unit.BureauId];
                        TreeViewNode temp = new TreeViewNode
                        {
                            id = unit.UnitId,
                            type = "Unit",
                            text = unit.Name
                        };
                        parent.nodes.Add(temp);
                        unitDictionary.Add(temp.id, temp);
                        unitList.Remove(unit);
                    }
                    else if (unitDictionary.ContainsKey((int)unit.ParentId))
                    {
                        TreeViewNode parent = unitDictionary[(int)unit.ParentId];
                        TreeViewNode temp = new TreeViewNode
                        {
                            id = unit.UnitId,
                            type = "Unit",
                            text = unit.Name,
                        };
                        if(parent.nodes != null)
                        {
                            parent.nodes.Add(temp);
                        }
                        else
                        {
                            parent.nodes = new List<TreeViewNode>();
                            parent.nodes.Add(temp);
                        }
                        unitDictionary.Add(temp.id, temp);
                        unitList.Remove(unit);
                    }
                }
                return CreateUnits(unitList);
            }
        }
    }

    public class TreeViewNode
    {
        public int id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public List<TreeViewNode> nodes { get; set; }
    }


}
