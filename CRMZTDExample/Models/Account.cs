public class Account
{
    public string ERPCode { get; set; } //Código ERP
    public string AccountId { get; set; }
    public string Name { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string EmailAddress1 { get; set; }
    public string Telephone1 { get; set; }
    public string Address1_Country { get; set; }
    public string Address1_PostalCode { get; set; }
    public decimal Creditlimit { get; set; } //Tipo Moneda
    public int PaymentTermCode { get; set; } //Condciones de Pago (optión múltiple)

    public bool CreditOnHold { get; set; } //Dos opciones 

    public string CurrencyId { get; set; } //Dato referencia de búsqueda (lookup)
    
}