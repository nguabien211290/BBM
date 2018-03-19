using BBM.Business.Model.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace BBM.Business
{
    public class ExportExcelBusiness
    {
        public void ToExcel(HttpResponseBase Response, int type)
        {
            var data = new DataTable();

            var isWork = true;

            switch (type)
            {
                case (int)TypeView.Product:
                    break;
                default:
                    isWork = false;
                    break;
            }

            if (!isWork)
                return;

            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = GetData();
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=FileName.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        private DataTable GetData()
        {
            // Here we create a DataTable with four columns.
            DataTable dtSample = new DataTable();
            dtSample.Columns.Add("Dosage", typeof(int));
            dtSample.Columns.Add("Drug", typeof(string));
            dtSample.Columns.Add("Patient", typeof(string));
            dtSample.Columns.Add("Date", typeof(DateTime));

            // Here we add five DataRows.
            dtSample.Rows.Add(25, "Indocin", "David", DateTime.Now);
            dtSample.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            dtSample.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            dtSample.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            dtSample.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);
            dtSample.Rows.Add(25, "Indocin", "David", DateTime.Now);
            dtSample.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            dtSample.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            dtSample.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            dtSample.Rows.Add(25, "Indocin", "David", DateTime.Now);
            dtSample.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            dtSample.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            dtSample.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            dtSample.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);
            dtSample.Rows.Add(25, "Indocin", "David", DateTime.Now);

            return dtSample;

        }
    }

}