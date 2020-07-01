using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Rocket.Surgery.AspNetCore.FluentValidation.NewtonsoftJson
{
    /// <summary>
    /// A RFC 7807 compliant <see cref="Newtonsoft.Json.JsonConverter" /> for <see cref="FluentValidationProblemDetails" />.
    /// </summary>
    [PublicAPI]
    public sealed class ValidationProblemDetailsNewtonsoftJsonConverter : JsonConverter<FluentValidationProblemDetails>
    {
        /// <inheritdoc />
        public override FluentValidationProblemDetails ReadJson(
            JsonReader reader,
            Type objectType,
            FluentValidationProblemDetails existingValue,
            bool hasExistingValue,
            [NotNull] JsonSerializer serializer
        )
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            var annotatedProblemDetails = serializer.Deserialize<AnnotatedProblemDetails>(reader);
            if (annotatedProblemDetails == null)
            {
                return null!;
            }

            var problemDetails = existingValue ?? new FluentValidationProblemDetails();
            annotatedProblemDetails.CopyTo(problemDetails);
            return problemDetails;
        }

        /// <inheritdoc />
        public override void WriteJson(
            [NotNull] JsonWriter writer,
            FluentValidationProblemDetails value,
            [NotNull] JsonSerializer serializer
        )
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var annotatedProblemDetails = new AnnotatedProblemDetails(value);
            serializer.Serialize(writer, annotatedProblemDetails);
        }

        internal class AnnotatedProblemDetails
        {
            /// <summary>
            /// Required for JSON.NET deserialization.
            /// </summary>
#pragma warning disable CS8618
            public AnnotatedProblemDetails() { }
#pragma warning disable CS8618

            /// <summary>
            /// Required for JSON.NET deserialization.
            /// </summary>
            public AnnotatedProblemDetails(FluentValidationProblemDetails problemDetails)
            {
                Detail = problemDetails.Detail;
                Instance = problemDetails.Instance;
                Status = problemDetails.Status;
                Title = problemDetails.Title;
                Type = problemDetails.Type;

                foreach (var kvp in problemDetails.Extensions)
                {
                    Extensions[kvp.Key] = kvp.Value;
                }

                Rules = problemDetails.Rules;
                foreach (var kvp in problemDetails.ValidationErrors)
                {
                    Errors[kvp.Key] = kvp.Value;
                }
            }

            [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
            public int? Status { get; set; }

            [JsonProperty(PropertyName = "detail", NullValueHandling = NullValueHandling.Ignore)]
            public string Detail { get; set; }

            [JsonProperty(PropertyName = "instance", NullValueHandling = NullValueHandling.Ignore)]
            public string Instance { get; set; }

            [JsonExtensionData]
            // ReSharper disable once MemberCanBePrivate.Global
            public IDictionary<string, object> Extensions { get; } =
                new Dictionary<string, object>(StringComparer.Ordinal);

            [JsonProperty(PropertyName = "errors")]
            public IDictionary<string, FluentValidationProblemDetail[]> Errors { get; } =
                new Dictionary<string, FluentValidationProblemDetail[]>(StringComparer.Ordinal);

            [JsonProperty(PropertyName = "rules")]
            public IEnumerable<string> Rules { get; internal set; } = Array.Empty<string>();

            public void CopyTo(FluentValidationProblemDetails problemDetails)
            {
                problemDetails.Type = Type;
                problemDetails.Title = Title;
                problemDetails.Status = Status;
                problemDetails.Instance = Instance;
                problemDetails.Detail = Detail;

                foreach (var (key, value) in Extensions)
                {
                    problemDetails.Extensions[key] = value;
                }

                Rules = problemDetails.Rules;
                foreach (var (key, value) in problemDetails.ValidationErrors)
                {
                    Errors[key] = value;
                }
            }
        }
    }
}