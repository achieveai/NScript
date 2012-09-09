using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TestWebApplication
{
    /// <summary>
    /// Summary description for SrcMapper
    /// </summary>
    public class SrcMapper : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");

            var path = context.Request.Params["path"].Substring(1);
            context.Response.ContentType = "text";

            using (StreamReader reader = new StreamReader(path))
            {
                context.Response.Write(reader.ReadToEnd());
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}