namespace SharedContracts.Commands;

public class FinalizeOrderStatusCommand
{
    public Guid TrackingCode { get; set; }
    public decimal Amount { get; set; }
}
