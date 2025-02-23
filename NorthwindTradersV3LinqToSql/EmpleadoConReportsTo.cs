using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindTradersV3LinqToSql
{
    class EmpleadoConReportsTo : Employees
    {
        public string ReportsToName { get; set; }
        public string PhotoBase64 { get; set; }
    }
}
