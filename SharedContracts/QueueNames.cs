namespace SharedContracts;

public class QueueNames
{
    private const string RabbitUri = "queue:";
    public static Uri GetMessageUri(string key)
    {
        return new Uri(RabbitUri + key.PascalToKebabCaseMessage());
    }
    public static Uri GetActivityUri(string key)
    {
        return new Uri(GetActivityQueueName(key));
    }
    public static string GetActivityQueueName(string key)
    {
        var kebabCase = key.PascalToKebabCaseActivity();
        if (kebabCase.EndsWith('-'))
        {
            kebabCase = kebabCase.Remove(kebabCase.Length - 1);
        }
        return RabbitUri + kebabCase + '_' + "execute";
    }
}
