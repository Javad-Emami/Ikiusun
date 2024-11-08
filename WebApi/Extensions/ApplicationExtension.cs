using Swashbuckle.AspNetCore.SwaggerUI;
using WebApi.Extensions.Middlewares;

namespace WebApi.Extensions;

public static class ApplicationExtension
{
    public static WebApplication Config(this WebApplication app, IConfiguration configuration)
    {
        app.UseRouting();
        app.UseCors("CorsPolicy");

        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.DocExpansion(DocExpansion.List);
                c.SwaggerEndpoint("/swagger/v1/swagger.json","Ikiusun WebApi V1");
                c.DocumentTitle = "Ikiusun AI Application";
            });
        }

        if (app.Environment.IsProduction()) 
        {
            //app.UseSpaApp();
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseMiddleware<CustomExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => 
        {
            endpoints.MapControllers();
        });

        return app;
    }
}
