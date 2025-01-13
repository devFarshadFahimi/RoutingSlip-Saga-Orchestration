namespace SharedContracts.Commands;

public class CheckingForPaymentStatusCommand
{

    public string SrcIban { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public Guid TrackingCode { get; set; }
    public decimal Amount { get; set; }
}
