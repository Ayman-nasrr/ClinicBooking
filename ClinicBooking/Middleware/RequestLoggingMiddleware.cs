using System.Diagnostics;

namespace ClinicBooking.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;

        public RequestLoggingMiddleware(
            RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                await next(context);
            }
            finally
            {
                sw.Stop();

                Console.WriteLine(
                    $"[{context.Request.Method}] " +
                    $"{context.Request.Path} | " +
                    $"Status: {context.Response.StatusCode} | " +
                    $"Time: {sw.ElapsedMilliseconds} ms");
            }
        }
    }
}