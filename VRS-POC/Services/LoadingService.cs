using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRS_POC.Services
{
    
    public class LoadingService
    {
        public event EventHandler<bool> OnSetLoading;

        public LoadingService()
        {

        }

        public void ShowLoading()
        {
            OnSetLoading?.Invoke(this, true);
        }

        public void HideLoading()
        {
            OnSetLoading?.Invoke(this, false);
        }
    }
}
