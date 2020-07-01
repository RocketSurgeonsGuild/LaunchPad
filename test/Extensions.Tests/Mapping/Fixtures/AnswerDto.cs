// using System;
// using System.Collections.Generic;
// using Rocket.Surgery.Unions;

// namespace Rocket.Surgery.AutoMapper.Tests.Fixtures
// {
//     [UnionKey(nameof(Type))]
//     [JsonConverter(typeof(UnionConverter))]
//     internal abstract class AnswerDto : IEquatable<AnswerDto?>
//     {
//         protected AnswerDto(AnswerType type) => Type = type;
//         public AnswerType Type { get; }
//         public Guid Id { get; set; } = Guid.NewGuid();

//         protected bool IsEqual(AnswerDto? other) => other != null && Type == other.Type;
//         public abstract bool Equals(AnswerDto? other);

//         public override bool Equals(object? obj) => Equals(obj as AnswerDto);

//         public static bool operator ==(AnswerDto? answer1, AnswerDto? answer2) => EqualityComparer<AnswerDto>.Default.Equals(answer1!, answer2!);

//         public static bool operator !=(AnswerDto? answer1, AnswerDto? answer2) => !(answer1 == answer2);

//         public override int GetHashCode() => HashCode.Combine(Type, Id);
//     }
// }

