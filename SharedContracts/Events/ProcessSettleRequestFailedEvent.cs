namespace SharedContracts.Events;
public class ProcessSettleRequestFailedEvent
{
    public Guid TrackingCode { get; set; }
    public string? FailedDescriptionMessage { get; set; }
    public string? FailedDescriptionCode { get; set; }
}
public class SettleRequestReceivedEvent
{
    public Guid TrackingCode { get; set; }
    public decimal Amount { get; set; }
}
public class SettlementRequestSucceededEvent
{
    public Guid TrackingCode { get; set; }

}

public class SettlementRequestFailedEvent
{
    public Guid TrackingCode { get; set; }
    public string? FailedDescriptionMessage { get; set; }
    public string? FailedDescriptionCode { get; set; }
}

public class ProcessSettleRequestEvent
{
    public Guid TrackingCode { get; set; }
    public decimal Amount { get; set; }
    public string? FailedDescriptionMessage { get; set; }
    public string? FailedDescriptionCode { get; set; }
}
