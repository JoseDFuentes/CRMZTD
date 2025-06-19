using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCRMZTD
{
    public class PostUpdateAccount: IPlugin
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


            string entityImageName = "ImageUpdateAccount";
            

            if (!context.PreEntityImages.Contains(entityImageName))
            {
                return;
            }

            if (!context.PostEntityImages.Contains(entityImageName))
            {
                return;
            }

            if (request.Attributes.Contains("creditlimit"))
            {
                Entity postImageAccount = context.PostEntityImages[entityImageName];
                Entity preImageAccount = context.PreEntityImages[entityImageName];

                if (postImageAccount.Attributes.Contains("creditlimit") && preImageAccount.Attributes.Contains("creditlimit"))
                {
                    decimal creditLimitPre = ((Money)preImageAccount["creditlimit"]).Value;
                    decimal creditLimitPost = ((Money)postImageAccount["creditlimit"]).Value;

                    decimal diferencia = (creditLimitPost - creditLimitPre);

                    if (diferencia > 100)
                    {
                        Entity custLog = new Entity("ap_logclientes");

                        custLog["ap_descripcion"] = $"Actualización de limite de credito, cliente: {preImageAccount["name"]} diferencia : {diferencia}";

                        service.Create(custLog);
                    }    


                }



            }






        }
    }
}
