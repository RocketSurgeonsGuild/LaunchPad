using System.Text.Json;
using AutoMapper;

namespace Rocket.Surgery.Extensions.AutoMapper
{
    public class SystemJsonTextProfile : Profile
    {
        public SystemJsonTextProfile()
        {
            var converter = new JsonElementConverter();
            CreateMap<JsonElement, byte[]?>().ConvertUsing(converter);
            CreateMap<JsonElement, string?>().ConvertUsing(converter);
            CreateMap<JsonElement?, byte[]?>().ConvertUsing(converter);
            CreateMap<JsonElement?, string?>().ConvertUsing(converter);
            CreateMap<byte[]?, JsonElement>().ConvertUsing(converter);
            CreateMap<string?, JsonElement>().ConvertUsing(converter);
            CreateMap<byte[]?, JsonElement?>().ConvertUsing(converter);
            CreateMap<string?, JsonElement?>().ConvertUsing(converter);
            CreateMap<JsonElement?, JsonElement?>().ConvertUsing(converter);
            CreateMap<JsonElement?, JsonElement>().ConvertUsing(converter);
            CreateMap<JsonElement, JsonElement?>().ConvertUsing(converter);
            CreateMap<JsonElement, JsonElement>().ConvertUsing(converter);
        }

        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        /// <value>The name of the profile.</value>
        public override string ProfileName => nameof(SystemJsonTextProfile);
    }
}