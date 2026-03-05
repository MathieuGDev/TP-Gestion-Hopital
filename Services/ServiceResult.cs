namespace tp_hospital.Services;


// soit une valeur de succes, soit un message d'erreur.
public class ServiceResult<T>
{
    public T?     Value      { get; private set; }
    public bool   IsSuccess  { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public int    StatusCode   { get; private set; }

    public static ServiceResult<T> Success(T value) =>
        new() { IsSuccess = true, Value = value, StatusCode = 200 };

    public static ServiceResult<T> Created(T value) =>
        new() { IsSuccess = true, Value = value, StatusCode = 201 };

    public static ServiceResult<T> NotFound(string message) =>
        new() { IsSuccess = false, ErrorMessage = message, StatusCode = 404 };

    public static ServiceResult<T> Conflict(string message) =>
        new() { IsSuccess = false, ErrorMessage = message, StatusCode = 409 };

    public static ServiceResult<T> ConcurrencyConflict(string message) =>
        new() { IsSuccess = false, ErrorMessage = message, StatusCode = 409 };
}
