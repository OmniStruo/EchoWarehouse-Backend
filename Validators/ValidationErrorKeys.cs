namespace EchoWarehouse.Validators;

public class ValidationErrorKeys
{
    // General validation errors
    public const string RequiredField = "UI_Error_FieldIsRequired";
    public const string InvalidFormat = "UI_Error_InvalidFormat";
    public const string InvalidCredentials = "UI_Error_InvalidCredentials";
    public const string UnexpectedError = "UI_Error_UnexpectedError";
    public const string ValidationFailed = "UI_Error_ValidationFailed";
    
    // Authentication-specific errors
    public const string PasswordTooShort = "UI_Error_Auth_PasswordTooShort";
    public const string PasswordMismatch = "UI_Error_Auth_PasswordMismatch";
    public const string UserAlreadyExists = "UI_Error_Auth_UserAlreadyExists";
    public const string EmailAlreadyExists = "UI_Error_Auth_EmailAlreadyExists";
    public const string UserInactive = "UI_Error_Auth_UserInactive";
    public const string UserNotFound = "UI_Error_Auth_UserNotFound";
    public const string FailedToLogoutUser = "UI_Error_Auth_FailedToLogoutUser";
    public const string FailedToCreateUser = "UI_Error_Auth_FailedToCreateUser";
    
}