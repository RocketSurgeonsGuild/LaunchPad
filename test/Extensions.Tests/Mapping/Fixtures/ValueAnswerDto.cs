// using System;
// using Rocket.Surgery.Unions;

// namespace Rocket.Surgery.AutoMapper.Tests.Fixtures
// {
//     [Union(AnswerType.Value)]
//     internal sealed class ValueAnswerDto : AnswerDto
//     {
//         public ValueAnswerDto() : base(AnswerType.Value) { }
//         public string? Value { get; set; }

//         public override bool Equals(AnswerDto? other) => other is ValueAnswerDto answer &&
//                    IsEqual(other) &&
//                    Value == answer.Value;

//         public override int GetHashCode() => HashCode.Combine(Value, Type, Id);
//     }
// }

