// using System;
// using Rocket.Surgery.Unions;

// namespace Rocket.Surgery.AutoMapper.Tests.Fixtures
// {
//     [Union(AnswerType.Text)]
//     internal sealed class TextAnswerDto : AnswerDto
//     {
//         public TextAnswerDto() : base(AnswerType.Text) { }
//         public string? Label { get; set; }

//         public override bool Equals(object? obj) => Equals(obj as TextAnswerDto);

//         public override bool Equals(AnswerDto? other) => other is TextAnswerDto answer &&
//                    IsEqual(other) &&
//                    Label == answer.Label;

//         public override int GetHashCode() => HashCode.Combine(Label, Type, Id);
//     }
// }

