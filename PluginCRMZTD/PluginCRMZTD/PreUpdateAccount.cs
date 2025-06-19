using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCRMZTD
{
    public class PreUpdateAccount : IPlugin
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


            if (request.Attributes.Contains("paymenttermscode"))
            {
                OptionSetValue paymTermCode = (OptionSetValue)request["paymenttermscode"];

                if (paymTermCode.Value == 10)//10 pago al contado
                {
                    request["creditlimit"] = new Money(0);
                }
            }

        }

    }
}
