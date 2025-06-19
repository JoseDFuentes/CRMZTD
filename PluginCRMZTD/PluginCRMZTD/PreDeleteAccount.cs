using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCRMZTD
{
    public class PreDeleteAccount : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            ITracingService tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


        

            string entityPreImageName = "PreImageDeleteAcccount";

            if (!context.PreEntityImages.Contains(entityPreImageName))
            {
                return;
            }

            Entity preImageAccount = context.PreEntityImages[entityPreImageName];

            Entity custLog = new Entity("ap_logclientes");

            custLog["ap_descripcion"] = $"Borrado registro entidad clientes {preImageAccount["accountid"]} {preImageAccount["name"]}";

            service.Create(custLog);






        }


    }
}
