using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAM.Data;
using PAM.Models;
using Newtonsoft.Json;

namespace PAM.Controllers
{
    public class SelectUnitController : Controller
    {
        public SelectUnitController()
        {
        }

        // Make db query to get list of units
        // Also do treeview in here or make a separate method for that to call in here
        // RETURN A VIEW TO SYSTEMS
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var test = _TreeView();
            //Debug.WriteLine("*** TEST ***");
            //Debug.WriteLine(test);
            //Debug.WriteLine(test);
            //`````````````ViewData["UnitList"] = test;

            return View("../Request/SelectUnit");
        }

        //public string _TreeView()
        //{
        //    var UnitList = _dbContext.Units.Where(m => m.UnitId > 0).ToList();

        //    DataSet ds = new DataSet();
        //    ds = ToDataSet(UnitList);
        //    DataTable table = ds.Tables[0];
        //    DataRow[] parentMenus = table.Select("ParentId is null");
        //    var sb = new StringBuilder();
        //    sb.Append("[");
        //    string unorderedList = GenerateUL(parentMenus, table, sb);
        //    Debug.WriteLine("***");
        //    Debug.WriteLine(unorderedList);
        //    return unorderedList + "]";
        //}

        //private string GenerateUL(DataRow[] menu, DataTable table, StringBuilder sb)
        //{
        //    if (menu.Length > 0)
        //    {
        //        foreach (DataRow dr in menu)
        //        {
        //            sb.Append("{");
        //            // Was UnitId, now ParentId, compare results
        //            string pid = dr["UnitId"].ToString();
        //            string unitName = dr["Name"].ToString();
        //            sb.Append(String.Format(@" text: ""{0}""", unitName));
        //            // TEST
        //            //string unitid = dr["UnitId"].ToString();
        //            //sb.Append(String.Format(@" ""unitid"": ""{0}"",", unitid));
        //            //string url = dr["BureauId"].ToString();
        //            //sb.Append(String.Format(@" ""href"": ""{1}""", url));
        //            string parentId = dr["ParentId"].ToString();
        //            DataRow[] subMenu = table.Select(String.Format("ParentId = '{0}'", pid));
        //            if (subMenu.Length > 0 && !pid.Equals(parentId))
        //            {
        //                var subMenuBuilder = new StringBuilder();
        //                sb.Append(String.Format(@", nodes: ["));
        //                sb.Append(GenerateUL(subMenu, table, subMenuBuilder));
        //                sb.Append("]");
        //            }
        //            sb.Append("},");
        //        }
        //        sb.Remove(sb.Length - 1, 1);
        //    }
        //    return sb.ToString();
        //}

        //public DataSet ToDataSet<T>(List<T> items)
        //{
        //    DataTable dataTable = new DataTable(typeof(T).Name);

        //    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach(PropertyInfo prop in Props)
        //    {
        //        dataTable.Columns.Add(prop.Name);
        //    }

        //    foreach(T item in items)
        //    {
        //        var values = new object[Props.Length];
        //        for(int i = 0; i < Props.Length; i++)
        //        {
        //            values[i] = Props[i].GetValue(item, null);
        //        }
        //        dataTable.Rows.Add(values);
        //    }

        //    DataSet ds = new DataSet();
        //    ds.Tables.Add(dataTable);

        //    return ds;
        //}
    }
}
