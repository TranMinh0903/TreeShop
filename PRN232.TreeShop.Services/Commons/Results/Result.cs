namespace PRN232.LaptopShop.Services.Commons.Results
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public int StatusCode { get; }
        public T? Value { get; }

        public string? Errors { get; }
        private Result(bool isSuccess, int statusCode, T? value, string? errors = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Value = value;
            Errors = errors;
        }

   
        public static Result<T> Success(T? value, int statusCode = 200)
            => new(true, statusCode, value);

        public static Result<T> Failure(T? value, int statusCode = 400, string? errors = null)
            => new(false, statusCode, value, errors);
    }
}
