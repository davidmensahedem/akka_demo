using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Hosting;
using Demo.Api.Actors;

namespace Demo.Api.ExtensionServices
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterActor(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            var actorSystem = "WeatherForecastActorSystem";

            services.AddAkka(actorSystem, builder =>
            {
                builder.WithActors((system, registry, resolver) =>
                        RegisterActor<WeatherForecastProcessorActor>(system, registry, resolver));

                builder.WithActors((system, registry, resolver) =>
                       RegisterActor<WebSocketActor>(system, registry, resolver, "ABACDSDSWebSocketActor"));
            });

            return services;
        }

        private static Props GetActorProps<AT>(IDependencyResolver resolver, SupervisorStrategy s) where AT : ActorBase
        {
            return resolver.Props<AT>().WithSupervisorStrategy(s);
        }

        private static void RegisterActor<T>(ActorSystem system, IActorRegistry registry, IDependencyResolver resolver, string name = null!) where T : ActorBase
        {
            var strategy = new OneForOneStrategy(3, TimeSpan.FromSeconds(3), ex =>
            {
                if (ex is not ActorInitializationException)
                    return Directive.Resume;

                system.Terminate().Wait(1000);

                return Directive.Stop;
            });

            var prop = GetActorProps<T>(resolver, strategy);

            var actor = system.ActorOf(prop, name ?? nameof(T));

            registry.Register<T>(actor);
        }
    }
}
