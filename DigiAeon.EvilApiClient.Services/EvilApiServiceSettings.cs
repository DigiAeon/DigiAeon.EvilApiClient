using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiAeon.EvilApiClient.Services.Interfaces
{
    public class EvilApiServiceSettings
    {
        public string ApiBaseAddress { get; set; }

        public string UploadUrl { get; set; }

        public string UploadCustomerAction { get; set; }

        public string GetCustomerUrl { get; set; }
    }
}
