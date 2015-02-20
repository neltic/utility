using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

public class AjaxResponse
{
    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }
    public int Id { get; set; }
    public bool Ok { get { return Status == 0 || Status == HttpStatusCode.OK; } set { Status = value ? 0 : HttpStatusCode.InternalServerError; } }

    public AjaxResponse()
    {
        Set(0, string.Empty, 0);
    }
    public AjaxResponse(HttpStatusCode status, string message)
    {
        Set(status, message);
    }
    public AjaxResponse(HttpStatusCode status, string message, int id)
    {
        Set(status, message, id);
    }

    public void Set(HttpStatusCode status, string message, int id)
    {
        this.Id = id;
        this.Status = status;
        this.Message = message;
    }

    public void Set(HttpStatusCode status, string message)
    {
        this.Status = status;
        this.Message = message;
    }

}