using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using UsuarioApi.Configuration;

namespace UsuarioApi.Filter
{
    public class Validate : IAsyncActionFilter
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly TokenValidationOptions _options;

        public Validate(IHttpClientFactory clientFactory, IOptions<TokenValidationOptions> options)
        {
            _clientFactory = clientFactory;
            _options = options.Value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    context.Result = new ObjectResult(new { mensaje = "No autorizado" })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return;
                }

                var token = authHeader.Replace("Bearer ", "");
                var cliente = _clientFactory.CreateClient();

                var contenido = new StringContent(JsonSerializer.Serialize(token), Encoding.UTF8, "application/json");
                var response = await cliente.PostAsync(_options.LoginApiUrl, contenido);

                if (!response.IsSuccessStatusCode)
                {
                    context.Result = new ObjectResult(new { mensaje = "No autorizado" })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return;
                }

                await next();
            }
            catch (Exception ex)
            {
                context.Result = new ObjectResult(new
                {
                    mensaje = "Error en el Servidor",
                    StatusCode = StatusCodes.Status500InternalServerError

                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    } 
}
