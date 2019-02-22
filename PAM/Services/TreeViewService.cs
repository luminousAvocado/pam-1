using System.Collections.Generic;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Models;

namespace PAM.Services
{
    public class TreeViewService
    {
        private readonly OrganizationService _organizationService;


        public TreeViewService(OrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        public List<TreeViewNode> GenerateTree()
        {
            List<TreeViewNode> tree = new List<TreeViewNode>();
            Dictionary<string, TreeViewNode> nodeDictionary = new Dictionary<string, TreeViewNode>();
            var bureauList = _organizationService.GetBureaus();
            var unitList = _organizationService.GetUnits();

            foreach (var bureau in bureauList)
            {
                var node = new TreeViewNode(bureau);
                tree.Add(node);
                nodeDictionary.Add("b" + node.id, node);
            }

            int unitsProcessed = 0;
            while (unitsProcessed < unitList.Count)
            {
                foreach (var unit in unitList)
                {
                    if (unit.UnitId < 0) continue;
                    var node = new TreeViewNode(unit);
                    var parentKey = unit.ParentId == null ? "b" + unit.BureauId : "u" + unit.ParentId;
                    var parent = nodeDictionary[parentKey];
                    if (parent.nodes == null) parent.nodes = new List<TreeViewNode>();
                    parent.nodes.Add(node);
                    nodeDictionary.Add("u" + node.id, node);
                    unit.UnitId = -1;
                    ++unitsProcessed;
                }
            }

            return tree;
        }

        public string GenerateTreeInJson()
        {
            return JsonConvert.SerializeObject(GenerateTree());
        }
    }


    public class TreeViewNode
    {
        public int id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public bool selectable { get; set; } = true;

        public int? displayOrder { get; set; }
        public int bureauId { get; set; }
        public string bureauName { get; set; }

        public List<TreeViewNode> nodes { get; set; }

        public TreeViewNode(Bureau bureau)
        {
            id = bureau.BureauId;
            type = "Bureau";
            text = bureau.Name;
        }

        public TreeViewNode(Unit unit)
        {
            id = unit.UnitId;
            type = "Unit";
            text = unit.Name;
            displayOrder = unit.DisplayOrder;
            bureauId = unit.BureauId;
            bureauName = unit.Bureau.Name;
        }
    }
}
