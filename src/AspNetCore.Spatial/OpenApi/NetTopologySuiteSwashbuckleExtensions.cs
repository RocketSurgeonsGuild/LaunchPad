using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi
{
    [ExcludeFromCodeCoverage]
    internal static class NetTopologySuiteSwashbuckleExtensions
    {
        class DocumentFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                swaggerDoc.Components.Schemas.Add(
                    "Point3D",
                    new OpenApiSchema
                    {
                        Type = "array",
                        Description = "Point in 3D space",
                        ExternalDocs = new OpenApiExternalDocs
                        {
                            Url = new Uri("http://geojson.org/geojson-spec.html#id2")
                        },
                        MinItems = 2,
                        MaxItems = 3,
                        Items = new OpenApiSchema
                        {
                            Type = "number"
                        }
                    }
                );
            }
        }

        public static SwaggerGenOptions ConfigureForNetTopologySuite(this SwaggerGenOptions c)
        {
            c.DocumentFilter<DocumentFilter>();
            c.MapType<Geometry>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("http://geojson.org/geojson-spec.html#geometry-objects"),
                    },
                    Type = "object",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new OpenApiString(typeof(Geometry).FullName)
                    },
                    Description = "GeoJSon geometry",
                    Discriminator = new OpenApiDiscriminator
                    {
                        PropertyName = "type",
                    },
                    Required = new HashSet<string> { "type" },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["type"] = new OpenApiSchema
                        {
                            Type = "string",
                            Enum = new List<IOpenApiAny>
                            {
                                new OpenApiString("Point"),
                                new OpenApiString("LineString"),
                                new OpenApiString("Polygon"),
                                new OpenApiString("MultiPoint"),
                                new OpenApiString("MultiLineString"),
                                new OpenApiString("MultiPolygon"),
                            },
                            Description = "the geometry type"
                        }
                    }
                }
            );

            c.MapType<Point>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("http://geojson.org/geojson-spec.html#id2"),
                    },
                    Type = "object",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new OpenApiString(typeof(Point).FullName)
                    },
                    Description = "GeoJSon geometry",
                    AllOf = new List<OpenApiSchema>
                    {
                        new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "Geometry",
                            }
                        },
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.Schema,
                                        Id = "Point3D"
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<LineString>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("http://geojson.org/geojson-spec.html#id3"),
                    },
                    Type = "object",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new OpenApiString(typeof(LineString).FullName)
                    },
                    Description = "GeoJSon geometry",
                    AllOf = new List<OpenApiSchema>
                    {
                        new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "Geometry",
                            }
                        },
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "Point3D"
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<Polygon>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("http://geojson.org/geojson-spec.html#id4"),
                    },
                    Type = "object",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new OpenApiString(typeof(Polygon).FullName)
                    },
                    Description = "GeoJSon geometry",
                    AllOf = new List<OpenApiSchema>
                    {
                        new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "Geometry",
                            }
                        },
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "Point3D"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<MultiPoint>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("http://geojson.org/geojson-spec.html#id4"),
                    },
                    Type = "object",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new OpenApiString(typeof(MultiPoint).FullName)
                    },
                    Description = "GeoJSon geometry",
                    AllOf = new List<OpenApiSchema>
                    {
                        new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "Geometry",
                            }
                        },
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "Point3D"
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<MultiLineString>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("http://geojson.org/geojson-spec.html#id5"),
                    },
                    Type = "object",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new OpenApiString(typeof(MultiLineString).FullName)
                    },
                    Description = "GeoJSon geometry",
                    AllOf = new List<OpenApiSchema>
                    {
                        new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "Geometry",
                            }
                        },
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "Point3D"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<MultiPolygon>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("http://geojson.org/geojson-spec.html#id6"),
                    },
                    Type = "object",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new OpenApiString(typeof(MultiPolygon).FullName)
                    },
                    Description = "GeoJSon geometry",
                    AllOf = new List<OpenApiSchema>
                    {
                        new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "Geometry",
                            }
                        },
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema
                                            {
                                                Reference = new OpenApiReference
                                                {
                                                    Type = ReferenceType.Schema,
                                                    Id = "Point3D"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<GeometryCollection>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("http://geojson.org/geojson-spec.html#geometrycollection"),
                    },
                    Type = "object",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new OpenApiString(typeof(GeometryCollection).FullName)
                    },
                    Required = new HashSet<string> { "type", "geometries" },
                    Description = "GeoJSon geometry collection",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["type"] = new OpenApiSchema
                        {
                            Type = "string",
                            Enum = new List<IOpenApiAny> { new OpenApiString("GeometryCollection") },
                        },
                        ["geometries"] = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "Geometry"
                                }
                            }
                        }
                    }
                }
            );

            c.MapType<Feature>(
                () => new OpenApiSchema
                {
                    Type = "object",
                    Description = "GeoJSon Feature",
                    Required = new HashSet<string> { "type", "id", "geometry" },
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("https://tools.ietf.org/html/rfc7946#section-3.2")
                    },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["type"] = new OpenApiSchema
                        {
                            Type = "string",
                            Enum = new List<IOpenApiAny> { new OpenApiString("Feature") }
                        },
                        ["id"] = new OpenApiSchema
                        {
                            Type = "integer",
                        },
                        ["geometry"] = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "GeometryCollection",
                            }
                        },
                        ["properties"] = new OpenApiSchema
                        {
                            Type = "object"
                        }
                    }
                }
            );

            c.MapType<FeatureCollection>(
                () => new OpenApiSchema
                {
                    Type = "object",
                    Description = "GeoJSon Feature collection",
                    Required = new HashSet<string> { "type", "features" },
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new("https://tools.ietf.org/html/rfc7946#section-3.2")
                    },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["type"] = new OpenApiSchema
                        {
                            Type = "string",
                            Enum = new List<IOpenApiAny> { new OpenApiString("FeatureCollection") }
                        },
                        ["features"] = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "Feature"
                                }
                            }
                        }
                    }
                }
            );


            return c;
        }
    }
}