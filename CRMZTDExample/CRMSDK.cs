using Microsoft.PowerPlatform.Dataverse.Client;

public class CRMSDK
{
    public static void getAccountInfoExample(ServiceClient serviceClient, SDKConnection sdkConnection)
    {
        string accountId = "14a9885b-4046-f011-8779-7c1e521b8aaa"; // Replace with a valid account ID

        sdkConnection.getAccountInfo(accountId, serviceClient);
    }

    public static void createAccountExample(SDKConnection sdkConnection, ServiceClient serviceClient)
    {

        Account account = new Account
        {

            Name = "Ana Garcia Rodriguez",
            ExchangeRate = 1.0M,
            EmailAddress1 = "agarciar@dgmail.com",
            Telephone1 = "+502 1234-5678",
            Address1_Country = "Guatemala"
        };

        sdkConnection.CreateAccount(account, serviceClient);

        account = new Account
        {

            Name = "Carlos Eduardo Mendoza",
            ExchangeRate = 1.0M,
            //EmailAddress1 = "agarciar@dgmail.com",
            //Telephone1 = "+502 1234-5678",
            Address1_Country = "Guatemala"

        };

        sdkConnection.CreateAccount(account, serviceClient);

        account = new Account
        {

            Name = "Maria Elena Lopez",
            ExchangeRate = 1.0M,
            //EmailAddress1 = "agarciar@dgmail.com",
            //Telephone1 = "+502 1234-5678",
            Address1_Country = "México",

        };

        sdkConnection.CreateAccount(account, serviceClient);
        account = new Account
        {

            Name = "Jose Miguel Herrera",
            ExchangeRate = 1.0M,
            //EmailAddress1 = "agarciar@dgmail.com",
            //Telephone1 = "+502 1234-5678",
            Address1_Country = "Honduras"
        };

        sdkConnection.CreateAccount(account, serviceClient);

    }

    public static void updateAccountExample(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        Account account = new Account
        {
            AccountId = "14a9885b-4046-f011-8779-7c1e521b8aaa", // Replace with a valid account ID
            Name = "Ana Garcia Rodriguez",
            ExchangeRate = 1.0M
        };


        sdkConnection.UpdateAccount(account, serviceClient);
    }

    public static void deleteAccountExample(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        string accountId = "14a9885b-4046-f011-8779-7c1e521b8aaa"; // Replace with a valid account ID

        sdkConnection.DeleteAccount(accountId, serviceClient);
    }

    public static void getAccountPerCountryExample(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        sdkConnection.getAccountPerCountry("Guatemala", serviceClient);
        sdkConnection.getAccountPerCountry("Honduras", serviceClient);
        sdkConnection.getAccountPerCountry("México", serviceClient);
    }

    public static void updateTelephoneByCountryCodeExample(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        sdkConnection.updateTelephoneByCountryCode("Guatemala", "+502 ", serviceClient);
        sdkConnection.updateTelephoneByCountryCode("Honduras", "+504 ", serviceClient);
        sdkConnection.updateTelephoneByCountryCode("México", "+52 ", serviceClient);
    }



    public static void createOrUpdate(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        /*

        creditlimit
        creditonhold
        paymenttermscode
        transactioncurrencyid
        */


        /*
         pago 30 1
         2/% 2
         Pago 45 3
         Pago 60 4
        */

        var account1 = new Account
        {
            ERPCode = "ERP001",
            Name = "Corporación Tecnológica Moderna S.A.",
            ExchangeRate = 1.00m,
            EmailAddress1 = "contacto@tecnomoderna.com",
            Telephone1 = "+502-2345-6789",
            Address1_Country = "Guatemala",
            Address1_PostalCode = "01001",
            Creditlimit = 50000.00m,
            PaymentTermCode = 1,
            CreditOnHold = false,
            CurrencyId = "GTQ"
        };

        sdkConnection.createOrUpdateAccount(account1, serviceClient);

        var account2 = new Account
        {
            ERPCode = "ERP002",
            Name = "Industrias del Café Chapín Ltda.",
            ExchangeRate = 7.85m,
            EmailAddress1 = "ventas@cafechapin.gt",
            Telephone1 = "+502-7890-1234",
            Address1_Country = "Guatemala",
            Address1_PostalCode = "01010",
            Creditlimit = 75000.00m,
            PaymentTermCode = 2,
            CreditOnHold = false,
            CurrencyId = "USD"
        };

        sdkConnection.createOrUpdateAccount(account2, serviceClient);

    }


    public static void getAccountByFetchXML(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        sdkConnection.getAccountFetch(serviceClient);

    }

    public static void getAccountByERPCodeFetchXML(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        sdkConnection.getAcountByCodeFetch(serviceClient, "ERP001");
    }


    public static void getAccountByERPCodeFetchXMLLink(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        sdkConnection.getAcountByCodeFetchLinkEntity(serviceClient, "ERP001");
    }

    public static void getAcountByCurrencyFetchLinkEntity(SDKConnection sdkConnection, ServiceClient serviceClient)
    {
        sdkConnection.getAcountByCurrencyFetchLinkEntity(serviceClient, "GTQ");
        sdkConnection.getAcountByCurrencyFetchLinkEntity(serviceClient, "USD");
    }
}