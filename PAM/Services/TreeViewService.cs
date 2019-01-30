using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PAM.Data;

namespace PAM.Services
{
    public class TreeViewService
    {
        private readonly AppDbContext _dbContext;

        public TreeViewService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
