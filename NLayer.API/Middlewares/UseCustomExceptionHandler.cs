using Microsoft.AspNetCore.Diagnostics;
using NLayer.Core.DTOs;
using NLayer.Service.Exceptions;
using System.Text.Json;

namespace NLayer.API.Middlewares
{
    public static class UseCustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app) // IApplicationBuilder = > Program.csdeki middlewareler buradan miras almaktadır, o yüzden tip olarak onu seçtik
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context => // Run sonlandırıcı bir middlewaredir, buradan sonra geri dönüş sağlanır, daha fazla işlem yapılmaz
                {
                    context.Response.ContentType = "application/json"; // Dönüş Tipi
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>(); // Uygulamada fırlatılan hata alınır böylelikle
                    var statusCode = exceptionFeature.Error switch // Hata kaynağını çözme; Bu hata client taraflı mı yoksa server taraflı bir hata mı onu ayırt edebilmek için
                    {
                        ClientSideException => 400,
                        NotFoundException => 404,
                        _ => 500 // Default olarak 500 atılacak, _ o yüzden verildi
                    };
                    context.Response.StatusCode = statusCode;

                    var response = CustomResponseDto<NoContentDto>.Fail(exceptionFeature.Error.Message, statusCode);

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response)); // Response datası JSONa döndürüldü
                });
            });
        }
    }
}
