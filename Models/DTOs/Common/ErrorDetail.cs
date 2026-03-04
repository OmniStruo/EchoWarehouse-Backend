namespace EchoWarehouse.Models.DTOs.Common;

public class ErrorDetail
{
    public string Key { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public ErrorDetail()
    {
    }

    public ErrorDetail(string key, string message)
    {
        Key = key;
        Message = message;
    }
}