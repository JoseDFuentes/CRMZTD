using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PluginCRMZTD
{
    public class PreCreateAccountIPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            ITracingService tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (!context.InputParameters.Contains("Target"))
            {
                return;
            }

            Entity request = (Entity)context.InputParameters["Target"];

            if (request.LogicalName != "account")
            {
                return;
            }

            request["ownershipcode"] = new OptionSetValue(2);
            request["industrycode"] = new OptionSetValue(43);
        }
    
    }
}
