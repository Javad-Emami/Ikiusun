using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common;

public class ApiResult<T>
{
    public int Code { get; }
    public string Message { get; }
    public T Result { get; }

    public ApiResult(T result)
    {
        Code = 200;
        Message = "Successful";
        Result = result;
    }

    public ApiResult(T result, int code)
    {
        Message = code switch
        {
            200 => "Successful",
            _ => "Unsuccessful"
        };
        Code = code;
        Result = result;
    }

    public ApiResult(T result, int code, string  message)
    {
        Code = code;
        Message = message;  
        Result = result;
    }
}

public class CreateApiResult<T> : ApiResult<CreateDto<T>>
{
    public CreateApiResult(T id, int code,string message) : base(new CreateDto<T>() { Id = id}, code,message)
    {
        
    }

    public CreateApiResult(T id): base(new CreateDto<T>() { Id = id})
    {
        
    }

    public CreateApiResult(T id, int code): base(new CreateDto<T>() { Id = id}, code)
    {
        
    }
}
