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
        public static readonly Domain Repository = new Domain();

        static TestModel()
        {
            Repository.AddRule<TestModel>(x => x.If(a => a.ValidateName).Then(r =>
            {
                r.RequireThat(a => !string.IsNullOrEmpty(a.Name)).WithModelViolation("Name cannot be null");
                r.RequireThat(a => a.Name == a.ConfirmName).WithModelViolation("Names must be the same");
            }));

            Repository.AddRule<TestModel>(x => x.If(a => a.ValidateNumber).
                RequireThat(a => a.ConfirmNumber > a.Number).WithModelViolation("Second must be greater than the first"));

            Repository.AddRule<TestModel>(x => x.If(a => a.ValidateNumber).
                RequireThat(a => a.Number != null).WithModelViolation("Number must have value"));
        }

        #region name

        [custom]
        public string Name { get; set; }

        public bool ValidateName { get; set; }

        [custom]
        public string ConfirmName { get; set; }

        #endregion

        #region number

        [custom]
        public int? Number { get; set; }

        public bool ValidateNumber { get; set; }

        [custom]
        public int? ConfirmNumber { get; set; }

        #endregion

        [Required]
        public string TestAllOk { get; set; }
    }

    public class customAttribute : BackToFront.Web.WebValidateAttribute
    {
        protected override Domain GetRepository(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            return TestModel.Repository;
        }
    }

    public class TestController : Controller
    {
        [HttpGet]
        public ActionResult TestForm()
        {
            return View(new TestModel());
        }

        [HttpPost]
        public ActionResult TestForm(TestModel model)
        {
            return RedirectToAction("TestForm");
        }
    }
}
