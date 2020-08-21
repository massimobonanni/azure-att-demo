using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AzureTableService.Core.Entities;

namespace WebSite.Models.Employees
{
    public class SeedViewModel
    {
        
        [Range(1,1000,ErrorMessage = "Number of employees to generate must be betwen 1 and 1000")]
        public int NumberOfEmployees { get; set; }
        
    }
}
