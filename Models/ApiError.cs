public class ApiError
{
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }

    public ApiError(int errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
