public class TransactionCallbackViewModel
{
    public int status { get; set; }
    public Result result { get; set; }
}
public class Result
{
    public string? transactionHash { get; set; }
    public int nonce { get; set; }
    public string? from { get; set; }
    public string? status { get; set; }
}