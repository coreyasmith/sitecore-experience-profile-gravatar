using System;
using System.Web.Mvc;
using CoreyAndRick.Project.Website.Models;
using CoreyAndRick.Project.Website.Repositories;

namespace CoreyAndRick.Project.Website.Controllers
{
    public class ContactController : BaseController
    {
        private readonly ContactRepository _repository;

        public ContactController(ContactRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ActionResult ContactEditor()
        {
            var model = _repository.GetModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult IdentifyContact(ContactEditorModel model)
        {
            _repository.IdentifyContact(model);
            return RedirectToReferrer();
        }

        [HttpPost]
        public ActionResult UpdateContact(ContactEditorModel model)
        {
            _repository.UpdateContact(model);
            return RedirectToReferrer();
        }
    }
}
