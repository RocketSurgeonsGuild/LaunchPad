using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Rocket.Surgery.LaunchPad.Foundation;

#pragma warning disable 8618

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation
{
    /// <summary>
    /// A RFC 7807 compliant <see cref="JsonConverter" /> for <see cref="FluentValidationProblemDetails" />.
    /// </summary>
    [PublicAPI]
    public sealed class ValidationProblemDetailsConverter : JsonConverter<FluentValidationProblemDetails>
    {
        /// <inheritdoc />
        public override FluentValidationProblemDetails Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var annotatedProblemDetails = JsonSerializer.Deserialize<AnnotatedProblemDetails>(ref reader, options);

            var problemDetails = new FluentValidationProblemDetails();
            annotatedProblemDetails?.CopyTo(problemDetails);
            return problemDetails;
        }

        /// <inheritdoc />
        public override void Write(
            [JetBrains.Annotations.NotNull] Utf8JsonWriter writer,
            FluentValidationProblemDetails value,
            JsonSerializerOptions options
        )
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var annotatedProblemDetails = new AnnotatedProblemDetails(value);
            JsonSerializer.Serialize(writer, annotatedProblemDetails, options);
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [UsedImplicitly]
        internal class AnnotatedProblemDetails
        {
            public AnnotatedProblemDetails() { }

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

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("status")]
            public int? Status { get; set; }

            [JsonPropertyName("detail")]
            public string Detail { get; set; }

            [JsonPropertyName("instance")]
            public string Instance { get; set; }

            [JsonExtensionData]
            public IDictionary<string, object> Extensions { get; } =
                new Dictionary<string, object>(StringComparer.Ordinal);

            [JsonPropertyName("errors")]
            public IDictionary<string, FluentValidationProblemDetail[]> Errors { get; } =
                new Dictionary<string, FluentValidationProblemDetail[]>(StringComparer.Ordinal);

            [JsonPropertyName("rules")]
            public IEnumerable<string> Rules { get; internal set; } = Array.Empty<string>();

            public void CopyTo(FluentValidationProblemDetails problemDetails)
            {
                problemDetails.Type = Type;
                problemDetails.Title = Title;
                problemDetails.Status = Status;
                problemDetails.Instance = Instance;
                problemDetails.Detail = Detail;

                foreach (var kvp in Extensions)
                {
                    problemDetails.Extensions[kvp.Key] = kvp.Value;
                }

                Rules = problemDetails.Rules;
                foreach (var kvp in problemDetails.ValidationErrors)
                {
                    Errors[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}