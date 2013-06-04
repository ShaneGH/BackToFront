using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackToFront.TestApp.Controllers
{
    public class TestModel
    {
        public static readonly Repository Repository = new Repository();

        static TestModel() 
        {
            Repository.AddRule<TestModel>(x => x.If(a => a.ValidateFlag)
                .RequireThat(a => a.Name == a.RequireThat)
                // TODO: custom error message
                .WithModelViolation("Name must be \"Shane\""));
        }

        [custom]
        public string Name { get; set; }

        public bool ValidateFlag { get; set; }

        public string RequireThat { get; set; }

        [Required]
        public string TestAllOk { get; set; }
    }

    public class customAttribute : BackToFront.Web.WebValidateAttribute
    {
        protected override Repository GetRepository(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            return TestModel.Repository;
        }
    }

    public class TestController : Controller
    {
        [HttpGet]
        public ActionResult TestForm()
        {
            return View(new TestModel { RequireThat = "Shane" });
        }
    }
}
