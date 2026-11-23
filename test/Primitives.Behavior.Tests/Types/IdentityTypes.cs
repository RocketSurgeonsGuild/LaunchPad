namespace Primitives.Behavior.Tests;

[StrongId<Guid>]
public readonly partial record struct CustomerId;

[StrongId<Guid>(GuidVersion = 4)]
public sealed partial record AccountId;

[StrongId<int>]
public readonly partial record struct OrderNumber;

[StrongId<long>]
public sealed partial record TransactionId;

[StrongId<long>(RejectEmpty = false)]
public readonly partial record struct SequenceNumber;

[StrongId<string>, MinLength(1), MaxLength(50)]
public readonly partial record struct Sku;

[StrongId<string>, Trim, UpperCase]
public sealed partial record PartNumber;

[SmartEnum<int>]
public readonly partial record struct OrderStatus
{
    public static readonly OrderStatus Pending = new(1);
    public static readonly OrderStatus Processing = new(2);
    public static readonly OrderStatus Completed = new(3);
}

[SmartEnum<string>]
public sealed partial record Role
{
    public static readonly Role Admin = new("admin");
    public static readonly Role User = new("user");
    public static readonly Role ReadOnlyAuditor = new("auditor");
}

[SmartEnum<int>]
public sealed partial record Priority
{
    public static readonly Priority Low = new(1);
    public static readonly Priority High = new(10);
}
