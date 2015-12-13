using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using DigiAeon.EvilApiClient.Services.Interfaces.CustomerFile;
using DigiAeon.EvilApiClient.UI.Bootstrapper.Interfaces;
using DigiAeon.EvilApiClient.UI.Models.CustomerFile;
using DigiAeon.EvilApiClient.UI.ViewModels.CustomerFile;

namespace DigiAeon.EvilApiClient.UI.Controllers
{
    public class CustomerFileController : ControllerBase
    {
        public CustomerFileController(IConfig config, ICustomerFileService customerFileService) : base(config)
        {
            CustomerFileService = customerFileService;
        }

        private ICustomerFileService CustomerFileService { get; }

        [HttpGet]
        [ActionName(CustomerFileControllerConstants.UploadAction)]
        public ActionResult Upload(UploadViewModel viewModel)
        {
            var model = new UploadModel(viewModel, Config, CustomerFileService);

            return View(model.ViewModel);
        }

        [HttpPost]
        [ActionName(CustomerFileControllerConstants.UploadAction)]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPost(UploadViewModel viewModel)
        {
            var model = new UploadModel(viewModel, Config, CustomerFileService);

            string uploadedFileName;
            if (model.UploadFile(out uploadedFileName))
            {
                return RedirectToAction(CustomerFileControllerConstants.FileProcessorAction, new {fileName = uploadedFileName});
            }

            return View(model.ViewModel);
        }

        [HttpGet]
        [ActionName(CustomerFileControllerConstants.FileProcessorAction)]
        public ActionResult FileProcessor(string fileName)
        {
            return View();
        }

        [HttpGet]
        [ActionName(CustomerFileControllerConstants.StartProcessingFileAction)]
        public string StartProcessingFile(string fileName)
        {
            var fileUploaded = Session["FileUploaded"] as List<string>;

            if (fileUploaded == null)
            {
                fileUploaded = new List<string>();
                Session["FileUploaded"] = fileUploaded;
            }

            if (!fileUploaded.Contains(fileName))
            {
                using (var reader = System.IO.File.OpenText(Path.Combine(Config.CustomerFileFolderMapPath, fileName)))
                {
                    CustomerFileService.UploadCustomersAndBroadcastResult(Config.Username, reader, fileName, HttpContext.GetOwinContext().Authentication.User.Identity.Name);
                }

                fileUploaded.Add(fileName);

                return "STARTED";
            }

            return "ALREADY_STARTED";
        }
    }

    public class CustomerFileControllerConstants
    {
        public const string ControllerName = "CustomerFile";
        public const string UploadAction = "Upload";
        public const string FileProcessorAction = "FileProcessor";
        public const string StartProcessingFileAction = "StartProcessingFile";
    }
}