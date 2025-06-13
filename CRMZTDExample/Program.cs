using Microsoft.PowerPlatform.Dataverse.Client;
ClientConfiguration clientConfiguration = new ClientConfiguration
{
    ClientId = "***********************c39",
    ClientSecret = "***************************Ipidff",
    TenantId = "****************************f5e", 
    Resource = "https://ambiente.crm.dynamics.com"    ,
    User = "admin@umbrellacorp.com",
    Password = "XXXXXXXXXXX" 
        
};


SDKConnection sdkConnection = new SDKConnection(){
    clientConfiguration = clientConfiguration   
};


ServiceClient serviceClient = sdkConnection.CreateServiceClient();


//CRMSDK.getAccountInfoExample(serviceClient, sdkConnection); 
//CRMSDK.createAccountExample(sdkConnection, serviceClient);
//CRMSDK.UpdateAccountExample(sdkConnection, serviceClient);    
//CRMSDK.DeleteAccountExample(sdkConnection, serviceClient);
//CRMSDK.getAccountPerCountryExample(sdkConnection, serviceClient);
//CRMSDK.updateTelephoneByCountryCodeExample(SEKConnection, serviceClient);

//CREAR O ACTUALIZAR UNA CUENTA BÚSCANDOLA POR CÓDIGO ERP
//CRMSDK.createOrUpdate(sdkConnection, serviceClient);

//OBTENER REGISTROS MEDIANTE FETCHXNK
//CRMSDK.getAccountByFetchXML(sdkConnection, serviceClient);

//FILTRAR REGISTROS MEDIANTE FETCHXML
//CRMSDK.getAccountByERPCodeFetchXML(sdkConnection, serviceClient);

//AGREGAR ENTIDADES RELACIONADAS EN FETCHXML
//CRMSDK.getAccountByERPCodeFetchXMLLink(sdkConnection, serviceClient);

//AGREGAR CONDICIONES EN ENTIDADES RELACIONADAS EN FETCHXML
CRMSDK.getAcountByCurrencyFetchLinkEntity(sdkConnection, serviceClient);



Console.ReadLine();



