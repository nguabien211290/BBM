using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Model.Module
{
    public class CityModel
    {
        public int id { get; set; }
        public string tentp { get; set; }
        public List<DistrictModel> Districts { get; set; }
    }

    public class DistrictModel
    {
        public int id { get; set; }
        public int idtp { get; set; }
        public string tentinh { get; set; }
    }
}
