using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Waves
{
    public interface IWavesAppService : IApplicationService
    {
        Task<List<string>> GetAddresses();

        Task<object> Redirect(string url, string methodType, string parameters = null);
    }
}
