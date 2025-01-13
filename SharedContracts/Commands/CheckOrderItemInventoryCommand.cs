namespace SharedContracts.Commands;
public class CheckOrderItemInventoryCommand
{
    public Guid TrackingCode { get; set; }
    public decimal Amount { get; set; }
}
