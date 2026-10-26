using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpinionDataLoader.Data.Context;
using OpinionDataLoader.Data.DAOs;
using OpinionDataLoader.Data.Interfaces.Daos;
using OpinionDataLoader.Data.Interfaces.Services;
using OpinionDataLoader.Data.Services;

namespace OpinionDataLoader.Load.Hosting;

public static class AppHost
{
    public static IHost CreateHost(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddDbContext<OpinionesOLTPContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                services.AddScoped<IDaoCategoria, DaoCategoria>();
                services.AddScoped<IDaoCliente, DaoCliente>();
                services.AddScoped<IDaoProducto, DaoProducto>();
                services.AddScoped<IDaoRedSocial, DaoRedSocial>();
                services.AddScoped<IDaoTipoEncuesta, DaoTipoEncuesta>();
                services.AddScoped<IDaoTipoClasificacion, DaoTipoClasificacion>();
                services.AddScoped<IDaoWebReview, DaoWebReview>();
                services.AddScoped<IDaoSocialComment, DaoSocialComment>();
                services.AddScoped<IDaoSurvey, DaoSurvey>();

                services.AddScoped<IServiceLoad, ServiceLoad>();

                services.AddScoped<IServiceCategoria>(sp =>
                    new ServiceCategoria(
                        sp.GetRequiredService<IDaoCategoria>(),
                        configuration["RutasCsv:Productos"]!));

                services.AddScoped<IServiceCliente>(sp =>
                    new ServiceCliente(
                        sp.GetRequiredService<IDaoCliente>(),
                        configuration["RutasCsv:Clientes"]!));

                services.AddScoped<IServiceProducto>(sp =>
                    new ServiceProducto(
                        sp.GetRequiredService<IDaoProducto>(),
                        configuration["RutasCsv:Productos"]!));

                services.AddScoped<IServiceWebReview>(sp =>
                    new ServiceWebReview(
                        sp.GetRequiredService<IDaoWebReview>(),
                        sp.GetRequiredService<OpinionesOLTPContext>(),
                        configuration["RutasCsv:WebReviews"]!));

                services.AddScoped<IServiceSocialComment>(sp =>
                    new ServiceSocialComment(
                        sp.GetRequiredService<IDaoSocialComment>(),
                        sp.GetRequiredService<OpinionesOLTPContext>(),
                        configuration["RutasCsv:SocialComments"]!));

                services.AddScoped<IServiceSurvey>(sp =>
                    new ServiceSurvey(
                        sp.GetRequiredService<IDaoSurvey>(),
                        sp.GetRequiredService<OpinionesOLTPContext>(),
                        configuration["RutasCsv:Surveys"]!));
            })
            .Build();
    }
}
