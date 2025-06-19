using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PluginCRMZTD
{
    public class PreCreateProduct : IPlugin
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

            if (request.LogicalName != "product")
            {
                return;
            }

            //unidad de venta
            QueryExpression queryUomSchedule = new QueryExpression()
            {
                EntityName = "uomschedule",
                ColumnSet = new ColumnSet("name", "uomscheduleid"),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, "Unidad predeterminada")

                    }
                }
            };

            EntityCollection uomCollection = service.RetrieveMultiple(queryUomSchedule);

            if (uomCollection.Entities.Count > 0)
            {
                Entity uomschedule = uomCollection.Entities[0];

                request["defaultuomscheduleid"] = new EntityReference("uomschedule", uomschedule.Id);

            }


            //unidad predeterminada
            QueryExpression queryUom = new QueryExpression()
            {
                EntityName = "uom",
                ColumnSet = new ColumnSet("name", "uomid"),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, "Unidad principal")

                    }
                }
            };

            EntityCollection uomDefaultCollection = service.RetrieveMultiple(queryUom);

            if (uomDefaultCollection.Entities.Count > 0)
            {
                Entity uomschedule = uomDefaultCollection.Entities[0];

                request["defaultuomid"] = new EntityReference("uom", uomschedule.Id);

            }

            request["quantitydecimal"] = 2;
            request["description"] = request["name"];







        }
    }
}
