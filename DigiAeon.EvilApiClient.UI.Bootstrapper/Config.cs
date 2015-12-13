using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Hosting;
using DigiAeon.EvilApiClient.UI.Bootstrapper.Interfaces;

namespace DigiAeon.EvilApiClient.UI.Bootstrapper
{
    public class Config : IConfig
    {
        public string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["Username"];
            }
        }
        public string SiteHeader
        {
            get
            {
                return ConfigurationManager.AppSettings["SiteHeader"];
            }
        }

        public List<string> CustomerFileAllowedFileExtensions
        {
            get
            {
                return ConfigurationManager.AppSettings["CustomerFile.AllowedFileExtensions"].Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            }
        }

        public string CustomerFileFolderMapPath
        {
            get
            {
                return HostingEnvironment.MapPath(ConfigurationManager.AppSettings["CustomerFile.FolderMapPath"]);
            }
        }

        public string EvilApiApiBaseAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["EvilApi.ApiBaseAddress"];
            }
        }

        public string EvilApiUploadUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["EvilApi.UploadUrl"];
            }
        }

        public string EvilApiUploadCustomerAction
        {
            get
            {
                return ConfigurationManager.AppSettings["EvilApi.UploadCustomerAction"];
            }
        }

        public string EvilApiGetCustomerUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["EvilApi.GetCustomerUrl"];
            }
        }
    }
}