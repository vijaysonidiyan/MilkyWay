using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MilkWayIndia.Models;
namespace MilkWayIndia.Controllers
{
    public class EmployeesController : ApiController
    {

        //  Employee[] employees = new Employee[]{
        //   new Employee { ID = 1, Name = "Mark", JoiningDate =
        //      DateTime.Parse(DateTime.Today.ToString()), Age = 30 },
        //   new Employee { ID = 2, Name = "Allan", JoiningDate =
        //      DateTime.Parse(DateTime.Today.ToString()), Age = 35 },
        //   new Employee { ID = 3, Name = "Johny", JoiningDate =
        //      DateTime.Parse(DateTime.Today.ToString()), Age = 21 }
        //};

        //  [Route("api/Employees/GetEmployee")]
        //  [HttpGet]
        //  public IEnumerable<Employee> GetAllEmployees()
        //  {
        //      return employees;
        //  }


        //  [Route("api/Employees/GetEmployeedetail/{id?}")]
        //  [HttpGet]
        //  public IHttpActionResult GetEmployee(int id)
        //  {
        //      var employee = employees.FirstOrDefault((p) => p.ID == id);
        //      if (employee == null)
        //      {
        //          return NotFound();
        //      }
        //      return Ok(employee);
        //  }


        [HttpGet]
        //[ArrayInput("nn")]
        [Route("api/Employees/GetBasicOperationResult/{nn?}")]
        public IHttpActionResult GetBasicOperationResult(string nn)
        {
            //BasicOperationPerform bop = new BasicOperationPerform();
            // double result = bop.PerformOperation(operation, n1, n2, nn);
            //int c = nn.Length;
            //string[] result = new string[c];
           string result = nn;
            int c = nn.Length;

            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string a = "";
            foreach (string s in nn.Split(delimiter))
            {
                 a = a +"-"+ s;
               
            }

            return Ok(a); 
            // return Ok(c);
        }
    }
}
