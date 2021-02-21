// using System;
// using System.Collections.Generic;
// using Rocket.Surgery.Unions;

// namespace Rocket.Surgery.AutoMapper.Tests.Fixtures
// {
//     [UnionKey(nameof(Type))]
//     [JsonConverter(typeof(UnionConverter))]
//     public abstract class AnswerModel : IEquatable<AnswerModel?>
//     {
//         protected AnswerModel(AnswerType type) => Type = type;
//         public AnswerType Type { get; }
//         public Guid Id { get; private set; }

//         protected bool IsEqual(AnswerModel? other) => other != null && Type == other.Type;
//         public abstract bool Equals(AnswerModel? other);

//         public override bool Equals(object? obj) => Equals(obj as AnswerModel);

//         public override int GetHashCode() => HashCode.Combine(Type, Id);

//         public static bool operator ==(AnswerModel? model1, AnswerModel? model2) => EqualityComparer<AnswerModel>.Default.Equals(model1!, model2!);

//         public static bool operator !=(AnswerModel? model1, AnswerModel? model2) => !(model1 == model2);
//     }
// }

