using Rocket.Surgery.LaunchPad.Primitives;

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

// Each member is made with new of its own class, named after it with an underscore, which the generator writes nested in the smart enum.
[SmartEnum<int>]
public closed partial class OrderStatus
{
    public static readonly OrderStatus Pending = new Pending_(1, "Waiting", 0m);
    public static readonly OrderStatus Processing = new Processing_(2, "In progress", 2.5m);
    public static readonly OrderStatus Completed = new Completed_(3, "Done", 0m);

    // Data every member carries, given after its value.
    public string Label { get; }
    public decimal Fee { get; }

    // Behaviour a member changes in its own part of its class.
    public virtual bool CanCancel => true;
}

public partial class OrderStatus
{
    public partial class Completed_
    {
        public override bool CanCancel => false;
    }
}

[SmartEnum<string>]
public closed partial class Role
{
    public static readonly Role Admin = new Admin_("admin", 100);
    public static readonly Role User = new User_("user", 10);
    public static readonly Role ReadOnlyAuditor = new ReadOnlyAuditor_("auditor", 1);

    // A readonly field is data too.
    public readonly int Rank;
}

[SmartEnum<int>]
public closed partial class Priority
{
    public static readonly Priority Low = new Low_(1);
    public static readonly Priority High = new High_(10);
}

// A smart enum can be a readonly partial record struct too: its members are its static readonly fields made with new(value).
[SmartEnum<int>]
public readonly partial record struct DeliveryStatus
{
    public static readonly DeliveryStatus Pending = new(1);
    public static readonly DeliveryStatus Processing = new(2);
    public static readonly DeliveryStatus Completed = new(3);
}

// A struct smart enum carries data like a closed one, given after its value: a get-only auto-property or a readonly field.
[SmartEnum<string>]
public readonly partial record struct Size
{
    public static readonly Size Small = new("s", "Small", 0.5m);
    public static readonly Size Medium = new("m", "Medium", 1m);
    public static readonly Size ExtraLarge = new("xl", "Extra large", 2.25m);

    public string Label { get; }
    public readonly decimal Weight;
}
