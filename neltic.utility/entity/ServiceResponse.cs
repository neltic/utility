using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

public class ServiceResponse<T>
{
    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }
    public T Response { get; set; }

    public ServiceResponse()
    {
        Set(0, string.Empty);
    }
    public ServiceResponse(HttpStatusCode status)
    {
        Set(status, string.Empty);
    }
    public ServiceResponse(HttpStatusCode status, string message)
    {
        Set(status, message);
    }
    public ServiceResponse(HttpStatusCode status, string message, T response)
    {
        Set(status, message, response);
    }
    public ServiceResponse(HttpStatusCode status, T response)
    {
        Set(status, response);
    }

    public void Set(HttpStatusCode status, string message, T response)
    {
        this.Response = response;
        this.Status = status;
        this.Message = message;
    }
    public void Set(HttpStatusCode status, T response)
    {
        this.Response = response;
        this.Status = status;
        this.Message = string.Empty;
    }
    public void Set(HttpStatusCode status, string message)
    {
        this.Status = status;
        this.Message = message;
    }
}

