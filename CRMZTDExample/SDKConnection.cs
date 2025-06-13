using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;


public class SDKConnection
{

    public ClientConfiguration clientConfiguration { get; set; }
    public ServiceClient CreateServiceClient()
    {
        /*
        string conectionString = $"AuthType=ClientSecret;" +
                                    $"ClientId={clientConfiguration.ClientId};" +
                                  $"ClientSecret={clientConfiguration.ClientSecret};" +
                                  $"Url={clientConfiguration.Resource};" +
                                $"LoginPrompt=Auto;RequireNewInstance=True";*/


        string conectionString = $"AuthType=OAuth;" +
                                        $"AppId={clientConfiguration.ClientId};" +
                                        $"UserName={clientConfiguration.User};" +
                                      $"Password={clientConfiguration.Password};" +
                                      $"Url={clientConfiguration.Resource};" +
                                    $"LoginPrompt=Auto;RequireNewInstance=True";


        ServiceClient serviceClient = new ServiceClient(conectionString);
        return serviceClient;

    }

    public void getAccountInfo(string accountId, ServiceClient serviceClient)
    {

        Entity accountInfo = serviceClient.Retrieve("account", new Guid(accountId), new ColumnSet(true));

        Console.WriteLine($"Account Name: {accountInfo.Attributes["name"]}");
        Console.WriteLine($"Account ExchRate: {accountInfo.Attributes["exchangerate"]}");



    }

    public void CreateAccount(Account _account, ServiceClient serviceClient)
    {
        Entity account = new Entity("account");
        account["name"] = _account.Name;
        account["exchangerate"] = _account.ExchangeRate;
        account["emailaddress1"] = _account.EmailAddress1;
        account["telephone1"] = _account.Telephone1;
        account["address1_country"] = _account.Address1_Country;
        account["address1_postalcode"] = _account.Address1_PostalCode;

        Guid accountId = serviceClient.Create(account);
        Console.WriteLine($"Account created with ID: {accountId.ToString()}");
    }


    public void UpdateAccount(Account _account, ServiceClient serviceClient)
    {
        Entity account = new Entity("account", new Guid(_account.AccountId));
        account["name"] = _account.Name;
        account["exchangerate"] = _account.ExchangeRate;
        account["emailaddress1"] = _account.EmailAddress1;
        account["telephone1"] = _account.Telephone1;

        serviceClient.Update(account);
        Console.WriteLine($"Account updated with ID: {_account.AccountId}");
    }

    public void DeleteAccount(string accountId, ServiceClient serviceClient)
    {
        serviceClient.Delete("account", new Guid(accountId));
        Console.WriteLine($"Account deleted with ID: {accountId}");
    }


    public void getAccountPerCountry(string country, ServiceClient serviceClient)
    {
        Console.WriteLine($"Retrieving accounts in country: {country}");


        QueryExpression query = new QueryExpression("account");
        query.ColumnSet = new ColumnSet("name", "exchangerate", "telephone1", "address1_country");
        query.Criteria.AddCondition("address1_country", ConditionOperator.Equal, country);



        EntityCollection accounts = serviceClient.RetrieveMultiple(query);

        foreach (var account in accounts.Entities)
        {
            Console.WriteLine($"Account Name: {account["name"]}, Exchange Rate: {account["exchangerate"]}, Country: {account["address1_country"]}");
        }
    }


    public void updateTelephoneByCountryCode(string countryCode, string phoneCode, ServiceClient serviceClient)
    {
        Console.WriteLine($"Updating telephone for accounts in country code: {countryCode}");

        QueryExpression query = new QueryExpression("account");
        query.ColumnSet = new ColumnSet("name", "telephone1", "address1_country");
        query.Criteria.AddCondition("address1_country", ConditionOperator.Equal, countryCode);

        EntityCollection accounts = serviceClient.RetrieveMultiple(query);

        foreach (var account in accounts.Entities)
        {

            if (account["telephone1"] is null)
            {
                continue;
            }
            string newTelephone = account["telephone1"].ToString();
            if (newTelephone.StartsWith(phoneCode))
            {
                Console.WriteLine($"Account {account["name"]} already has the phone code {phoneCode}. Skipping update.");
                continue;
            }
            account["telephone1"] = phoneCode + account["telephone1"].ToString();

            serviceClient.Update(account);
            Console.WriteLine($"Updated Account Name: {account["name"]}, New Telephone: {newTelephone}");
        }
    }

    public void createOrUpdateAccount(Account _account, ServiceClient serviceClient)
    {

        /*
       creditlimit
       creditonhold
       paymenttermscode
       transactioncurrencyid
       */
        bool existsAccount = false;

        if (string.IsNullOrEmpty(_account.ERPCode))
        {
            Console.WriteLine("ERPCode is required for creating or updating an account.");
            return;
        }

        QueryExpression accountQuery = new QueryExpression("account");
        accountQuery.ColumnSet = new ColumnSet("ap_codigoerp", "accountid");
        accountQuery.Criteria.AddCondition("ap_codigoerp", ConditionOperator.Equal, _account.ERPCode);
        EntityCollection accounts = serviceClient.RetrieveMultiple(accountQuery);
        Entity account;
        if (accounts.Entities.Count > 0)
        {
            existsAccount = true;
            _account.AccountId = accounts.Entities[0].Id.ToString();
            account = new Entity("account", new Guid(_account.AccountId));

        }
        else
        {
            existsAccount = false;
            account = new Entity("account");
            account["ap_codigoerp"] = _account.ERPCode;
        }


        account["name"] = _account.Name;
        account["exchangerate"] = _account.ExchangeRate;
        account["emailaddress1"] = _account.EmailAddress1;
        account["telephone1"] = _account.Telephone1;
        account["address1_country"] = _account.Address1_Country;
        account["address1_postalcode"] = _account.Address1_PostalCode;

        account["creditlimit"] = new Money(_account.Creditlimit); // Tipo Moneda
        account["creditonhold"] = _account.CreditOnHold; // Dos opciones
        account["paymenttermscode"] = new OptionSetValue(_account.PaymentTermCode); // Condciones de Pago (opción múltiple)
                                                                                    //account["transactioncurrencyid"] = new EntityReference("transactioncurrency", new Guid(_account.CurrencyId)); 

        EntityReference currencyReference = GetCurrencyReference(_account.CurrencyId, serviceClient);
        if (currencyReference != null)
        {
            account["transactioncurrencyid"] = currencyReference; // Dato referencia de búsqueda (lookup)
        }
        else
        {
            Console.WriteLine($"Currency with ISO code {_account.CurrencyId} not found.");
            return;
        }


        if (existsAccount)
        {
            serviceClient.Update(account);
            Console.WriteLine($"Account updated with ID: {_account.AccountId}");
        }
        else
        {
            Guid accountId = serviceClient.Create(account);
            Console.WriteLine($"Account created with ID: {accountId.ToString()}");
        }



    }

    public EntityReference GetCurrencyReference(string _isoCurrencyCode, ServiceClient serviceClient)
    {
        if (string.IsNullOrEmpty(_isoCurrencyCode))
        {
            return null;
        }

        /*
        Entidad Divisa: transactioncurrency
        Criterio de búsqueda: isocurrencycode
        */

        QueryExpression query = new QueryExpression("transactioncurrency");
        query.ColumnSet = new ColumnSet("isocurrencycode", "transactioncurrencyid");
        query.Criteria.AddCondition("isocurrencycode", ConditionOperator.Equal, _isoCurrencyCode);

        EntityCollection currencyCollection = serviceClient.RetrieveMultiple(query);
        if (currencyCollection.Entities.Count == 0)
        {
            return null; // No se encontró la divisa con el código ISO proporcionado
        }


        Entity currency = currencyCollection.Entities[0];
        return new EntityReference("transactioncurrency", currency.Id);
    }


    public void getAccountFetch(ServiceClient serviceClient)
    {
        string fetchXml = @"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
                <entity name='account'>
                    <attribute name='accountid'/>
                    <attribute name='telephone1'/>
                    <attribute name='name'/>
                    <attribute name='statecode'/>
                <filter type='and'>
			        <condition attribute='telephone1' operator='not-null' />
		        </filter>
                </entity>
            </fetch>";

        string fetchXMLERP = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
	<entity name='account'>
		<attribute name='statecode'/>
		<attribute name='name'/>
		<attribute name='accountid'/>
		<attribute name='transactioncurrencyid'/>
		<attribute name='creditlimit'/>
		<attribute name='paymenttermscode'/>
		<attribute name='ap_codigoerp'/>
		<order attribute='ap_codigoerp' descending='false'/>
		<filter type='and'>
			<condition attribute='ap_codigoerp' operator='not-null'/>
		</filter>
	</entity>
</fetch>";

        FetchExpression fetchExpression = new FetchExpression(fetchXMLERP);

        EntityCollection accounts = serviceClient.RetrieveMultiple(fetchExpression);

        if (accounts.Entities.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        Console.WriteLine("Accounts found:");

        foreach (Entity account in accounts.Entities)
        {
            Console.WriteLine($"ERP Code {account["ap_codigoerp"]} - Name: {account["name"]} - Credit Limit {((Money)account["creditlimit"]).Value} - Payment Terms : {((OptionSetValue)account["paymenttermscode"]).Value}");

        }

    }

    public void getAcountByCodeFetch(ServiceClient serviceClient, string erpCode)
    {

        string fetchXMLERP = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
	<entity name='account'>
		<attribute name='statecode'/>
		<attribute name='name'/>
		<attribute name='accountid'/>
		<attribute name='transactioncurrencyid'/>
		<attribute name='creditlimit'/>
		<attribute name='paymenttermscode'/>
		<attribute name='ap_codigoerp'/>
		<order attribute='ap_codigoerp' descending='false'/>
		<filter type='and'>
			<condition attribute='ap_codigoerp' operator='eq' value='{0}'/>
		</filter>
	</entity>
</fetch>";

        fetchXMLERP = string.Format(fetchXMLERP, erpCode);

        FetchExpression fetchExpression = new FetchExpression(fetchXMLERP);

        EntityCollection accounts = serviceClient.RetrieveMultiple(fetchExpression);

        if (accounts.Entities.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        Console.WriteLine("Accounts found:");

        foreach (Entity account in accounts.Entities)
        {
            string currencyIsoCode = string.Empty;
            if (account.Contains("transactioncurrencyid"))
            {
                EntityReference currencyRef = (EntityReference)account["transactioncurrencyid"];
                Entity currency = serviceClient.Retrieve(currencyRef.LogicalName, currencyRef.Id, new ColumnSet("isocurrencycode"));
                if (currency != null && currency.Contains("isocurrencycode"))
                {
                    currencyIsoCode = currency["isocurrencycode"].ToString();
                }
            }

            Console.WriteLine($"ERP Code {account["ap_codigoerp"]} - Name: {account["name"]} - Currency {currencyIsoCode} - Credit Limit {((Money)account["creditlimit"]).Value} - Payment Terms : {((OptionSetValue)account["paymenttermscode"]).Value}");




        }

    }

    public void getAcountByCodeFetchLinkEntity(ServiceClient serviceClient, string erpCode)
    {

        string fetchXMLERP = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
	<entity name='account'>
		<attribute name='statecode'/>
		<attribute name='name'/>
		<attribute name='accountid'/>
		<attribute name='transactioncurrencyid'/>
		<attribute name='creditlimit'/>
		<attribute name='paymenttermscode'/>
		<attribute name='ap_codigoerp'/>
		<order attribute='ap_codigoerp' descending='false'/>
		<filter type='and'>
			<condition attribute='ap_codigoerp' operator='eq' value='{0}'/>
		</filter>
        <link-entity alias='currency' name='transactioncurrency' to='transactioncurrencyid' from='transactioncurrencyid' link-type='outer' >
			<attribute name='isocurrencycode'/>
		</link-entity>
	</entity>
</fetch>";

        fetchXMLERP = string.Format(fetchXMLERP, erpCode);

        FetchExpression fetchExpression = new FetchExpression(fetchXMLERP);

        EntityCollection accounts = serviceClient.RetrieveMultiple(fetchExpression);

        if (accounts.Entities.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        Console.WriteLine("Accounts found:");

        foreach (Entity account in accounts.Entities)
        {
            string currencyIsoCode = string.Empty;

            if (account.Contains("transactioncurrencyid"))
            {
                AliasedValue currencyAliased = (AliasedValue)account["currency.isocurrencycode"];
                if (currencyAliased != null && currencyAliased.Value != null)
                {
                    currencyIsoCode = currencyAliased.Value.ToString();
                }
            }

            Console.WriteLine($"ERP Code {account["ap_codigoerp"]} - Name: {account["name"]} - Currency {currencyIsoCode} - Credit Limit {((Money)account["creditlimit"]).Value} - Payment Terms : {((OptionSetValue)account["paymenttermscode"]).Value}");




        }

    }


    public void getAcountByCurrencyFetchLinkEntity(ServiceClient serviceClient, string _currencyIsoCode)
    {

        string fetchXMLERP = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
	<entity name='account'>
		<attribute name='statecode'/>
		<attribute name='name'/>
		<attribute name='accountid'/>
		<attribute name='transactioncurrencyid'/>
		<attribute name='creditlimit'/>
		<attribute name='paymenttermscode'/>
		<attribute name='ap_codigoerp'/>
		<order attribute='ap_codigoerp' descending='false'/>
		<filter type='and'>
			<condition attribute='ap_codigoerp' operator='not-null'/>
		</filter>
        <link-entity alias='currency' name='transactioncurrency' to='transactioncurrencyid' from='transactioncurrencyid' link-type='inner' >
		<attribute name='isocurrencycode'/>
            <filter type='and'>
			<condition attribute='isocurrencycode' operator='eq' value='{0}'/>
		</filter>
		</link-entity>
	</entity>
</fetch>";

        fetchXMLERP = string.Format(fetchXMLERP, _currencyIsoCode);

        FetchExpression fetchExpression = new FetchExpression(fetchXMLERP);

        EntityCollection accounts = serviceClient.RetrieveMultiple(fetchExpression);

        if (accounts.Entities.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        Console.WriteLine($"Accounts found with Currency Code:{_currencyIsoCode}");

        foreach (Entity account in accounts.Entities)
        {
            string currencyIsoCode = string.Empty;

            if (account.Contains("transactioncurrencyid"))
            {
                AliasedValue currencyAliased = (AliasedValue)account["currency.isocurrencycode"];
                if (currencyAliased != null && currencyAliased.Value != null)
                {
                    currencyIsoCode = currencyAliased.Value.ToString();
                }
            }

            Console.WriteLine($"ERP Code {account["ap_codigoerp"]} - Name: {account["name"]} - Currency { currencyIsoCode } - Credit Limit {((Money)account["creditlimit"]).Value} - Payment Terms : {((OptionSetValue)account["paymenttermscode"]).Value}");
            
           
           
            
        }
        
    }

}