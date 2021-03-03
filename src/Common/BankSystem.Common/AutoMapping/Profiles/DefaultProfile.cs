namespace BankSystem.Common.AutoMapping.Profiles
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Interfaces;

    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            this.ConfigureMapping();
        }

        private void ConfigureMapping()
        {
            var allTypes = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .ToArray();

            var withBidirectionalMapping = allTypes
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && t.GetInterfaces()
                                .Where(i => i.IsGenericType)
                                .Select(i => i.GetGenericTypeDefinition())
                                .Contains(typeof(IMapWith<>)))
                .SelectMany(t =>
                    t.GetInterfaces()
                        .Where(i => i.IsGenericType &&
                                    i.GetGenericTypeDefinition() == typeof(IMapWith<>))
                        .SelectMany(i => i.GetGenericArguments())
                        .Select(s => new
                        {
                            Type1 = t,
                            Type2 = s
                        })
                )
                .ToArray();

            //Create bidirectional mapping for all types implementing the IMapWith<TModel> interface
            foreach (var mapping in withBidirectionalMapping)
            {
                this.CreateMap(mapping.Type1, mapping.Type2);
                this.CreateMap(mapping.Type2, mapping.Type1);
            }

            // Create custom mapping for all types implementing the IHaveCustomMapping interface
            var customMappings = allTypes.Where(t => t.IsClass
                                                     && !t.IsAbstract
                                                     && typeof(IHaveCustomMapping).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<IHaveCustomMapping>()
                .ToArray();

            foreach (var mapping in customMappings)
            {
                mapping.ConfigureMapping(this);
            }
        }
    }
}