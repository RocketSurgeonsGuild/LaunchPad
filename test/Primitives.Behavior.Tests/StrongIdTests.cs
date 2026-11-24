using Rocket.Surgery.LaunchPad.Primitives;

using static Primitives.Behavior.Tests.PrimitiveAssert;

namespace Primitives.Behavior.Tests;

public class StrongIdTests
{
    [Test]
    public async Task Should_Create_A_New_Guid_Of_Its_Version()
    {
        var first = CustomerId.Create();
        var second = CustomerId.Create();

        await first.Value.Version.Should().BeEqualTo(7);
        await AccountId.Create().Value.Version.Should().BeEqualTo(4);
        await first.Should().NotBeEqualTo(second);
        await first.IsDefault.Should().BeFalse();
    }

    [Test]
    public async Task Should_Wrap_A_Value()
    {
        var guid = Guid.CreateVersion7();
        await Accepts<CustomerId, Guid>(guid);
        await Accepts<AccountId, Guid>(guid);
        await Accepts<OrderNumber, int>(42);
        await Accepts<OrderNumber, int>(int.MinValue);
        await Accepts<TransactionId, long>(long.MaxValue);
        await Accepts<Sku, string>("SKU-1");
        await Accepts<PartNumber, string>(" ab-12 ", "AB-12");
    }

    [Test]
    public async Task Should_Reject_An_Empty_Id()
    {
        var error = await Rejects<CustomerId, Guid>(Guid.Empty, "EMPTY");
        await error.Message.Should().BeEqualTo("CustomerId must not be empty.");
        await Rejects<AccountId, Guid>(Guid.Empty, "EMPTY");
        await Rejects<OrderNumber, int>(0, "EMPTY");
        await Rejects<TransactionId, long>(0, "EMPTY");
        await Rejects<Sku, string>(" ", "EMPTY");
        await Rejects<Sku, string>(null!, "NULL_INPUT");
        await Rejects<Sku, string>(new string('s', 51), "LENGTH");
        // unless RejectEmpty is false
        await Accepts<SequenceNumber, long>(0);
    }

    [Test]
    public async Task Should_Not_Bypass_Its_Rules_Through_A_Conversion()
    {
        await Throws<DomainPrimitiveValidationException>(() => _ = (CustomerId)Guid.Empty);
        await Throws<DomainPrimitiveValidationException>(() => _ = (OrderNumber)0);
        await Throws<DomainPrimitiveValidationException>(() => _ = (Sku)"");
        var guid = Guid.CreateVersion7();
        await ( (Guid)(CustomerId)guid ).Should().BeEqualTo(guid);
        await ( (int)(OrderNumber)7 ).Should().BeEqualTo(7);
    }

    [Test]
    public async Task Should_Have_An_Empty_Id()
    {
        // the empty ID of a struct is its default value; of a class, an ID that holds the empty value
        await CustomerId.Empty.IsDefault.Should().BeTrue();
        await CustomerId.Empty.Should().BeEqualTo(default);
        await AccountId.Empty.Value.Should().BeEqualTo(Guid.Empty);
        await TransactionId.Empty.Value.Should().BeEqualTo(0L);
        await Throws<InvalidOperationException>(() => _ = OrderNumber.Empty.Value);
    }

    [Test]
    public async Task Should_Not_Create_An_Id_That_Is_Not_A_Guid()
    {
        await Throws<NotSupportedException>(() => OrderNumber.Create());
        await Throws<NotSupportedException>(() => TransactionId.Create());
        await Throws<NotSupportedException>(() => Sku.Create());
    }

    [Test]
    public async Task Should_Work_As_A_Key()
    {
        var guid = Guid.CreateVersion7();
        var lookup = new Dictionary<CustomerId, string> { [CustomerId.Create(guid)] = "found" };
        var set = new HashSet<AccountId> { AccountId.Create(guid), AccountId.Create(guid) };

        await lookup[CustomerId.Create(guid)].Should().BeEqualTo("found");
        await set.Count.Should().BeEqualTo(1);
        await CustomerId.Create(guid).GetHashCode().Should().BeEqualTo(CustomerId.Create(guid).GetHashCode());
    }

    [Test]
    public async Task Should_Order_Its_Values()
    {
        await ( OrderNumber.Create(1) < OrderNumber.Create(2) ).Should().BeTrue();
        await ( OrderNumber.Create(2) >= OrderNumber.Create(2) ).Should().BeTrue();
        // strings ordinally: upper case before lower case
        await Sku.Create("B").CompareTo(Sku.Create("a")).Should().BeLessThan(0);
        await ( TransactionId.Create(5) > TransactionId.Create(3) ).Should().BeTrue();
    }

    [Test]
    public async Task Should_Describe_Itself_As_A_Strong_Id()
    {
        await CustomerId.Description.IsStrongId.Should().BeTrue();
        await CustomerId.Description.Format.Should().BeEqualTo("uuid");
        await OrderNumber.Description.IsStrongId.Should().BeTrue();
        await Sku.Description.MaxLength.Should().BeEqualTo(50);
    }
}

public class SmartEnumTests
{
    [Test]
    public async Task Should_List_Every_Member_In_Declaration_Order()
    {
        await OrderStatus.All.Should().BeEquivalentTo([OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Completed], CollectionOrdering.Matching);
        await Role.All.Count.Should().BeEqualTo(3);
        // the list can't be changed through a cast
        await Assert.That(OrderStatus.All is OrderStatus[]).IsFalse();
        await Assert.That(OrderStatus.All is ICollection<OrderStatus> { IsReadOnly: false }).IsFalse();
    }

    [Test]
    public async Task Should_Find_A_Member_By_Its_Value()
    {
        await Accepts<OrderStatus, int>(2);
        await OrderStatus.FromValue(2).Should().BeEqualTo(OrderStatus.Processing);
        await OrderStatus.TryFromValue(3, out var completed).Should().BeTrue();
        await completed.Should().BeEqualTo(OrderStatus.Completed);
        await Role.FromValue("admin").Should().BeEqualTo(Role.Admin);

        var error = await Rejects<OrderStatus, int>(99, "UNKNOWN_MEMBER");
        await error.Message.Should().BeEqualTo("OrderStatus has no member with the value '99'.");
        await Rejects<Role, string>("root", "UNKNOWN_MEMBER");
        await Rejects<Role, string>(null!, "UNKNOWN_MEMBER");
        await OrderStatus.TryFromValue(int.MinValue, out _).Should().BeFalse();
    }

    [Test]
    public async Task Should_Find_A_Member_By_Its_Name()
    {
        await OrderStatus.FromName("Pending").Should().BeEqualTo(OrderStatus.Pending);
        await OrderStatus.FromName("pending", ignoreCase: true).Should().BeEqualTo(OrderStatus.Pending);
        await OrderStatus.TryFromName("pending", false, out _).Should().BeFalse();
        await OrderStatus.TryFromName("", true, out _).Should().BeFalse();
        await OrderStatus.TryFromName("  ", true, out _).Should().BeFalse();
        await OrderStatus.TryFromName(null, true, out _).Should().BeFalse();
        var exception = await Throws<DomainPrimitiveValidationException>(() => OrderStatus.FromName("Shipped"));
        await exception!.Error.Code.Should().BeEqualTo("UNKNOWN_MEMBER");
        await Role.FromName("ReadOnlyAuditor").Value.Should().BeEqualTo("auditor");
    }

    [Test]
    public async Task Should_Name_Its_Members()
    {
        await OrderStatus.Completed.Name.Should().BeEqualTo("Completed");
        await OrderStatus.Completed.ToString().Should().BeEqualTo("Completed");
        await Role.ReadOnlyAuditor.ToString().Should().BeEqualTo("ReadOnlyAuditor");
        // a member created from its value is the same member, with the same name
        await OrderStatus.Create(1).Name.Should().BeEqualTo("Pending");
        await Role.Create("user").Should().BeEqualTo(Role.User);
    }

    [Test]
    public async Task Should_Parse_A_Name_Or_A_Value()
    {
        await OrderStatus.Parse("processing", null).Should().BeEqualTo(OrderStatus.Processing);
        await OrderStatus.Parse("3", null).Should().BeEqualTo(OrderStatus.Completed);
        await Role.Parse("admin", null).Should().BeEqualTo(Role.Admin);
        await OrderStatus.TryParse("unknown", null, out _).Should().BeFalse();
        await OrderStatus.TryParse(null, null, out _).Should().BeFalse();
        await Throws<FormatException>(() => OrderStatus.Parse("42", null));
    }

    [Test]
    public async Task Should_Make_A_Member_From_Its_Value_As_The_Same_Instance()
    {
        // Create, FromValue, FromName and Parse return the member's own field, of the member's own class
        await Assert.That(OrderStatus.Create(2)).IsSameReferenceAs(OrderStatus.Processing);
        await Assert.That(OrderStatus.FromName("Completed")).IsSameReferenceAs(OrderStatus.Completed);
        await Assert.That(Role.Parse("auditor", null)).IsSameReferenceAs(Role.ReadOnlyAuditor);
        await Assert.That(OrderStatus.Create(1)).IsTypeOf<OrderStatus.Pending_>();
        await Assert.That(Role.Admin).IsTypeOf<Role.Admin_>();
    }

    [Test]
    public async Task Should_Switch_Over_The_Classes_Of_Its_Members()
    {
        // the smart enum is closed: a switch over its members' classes needs no default arm (IDE0072 doesn't know closed classes yet, and
        // dotnet format would add one)
#pragma warning disable IDE0072
        static string Describe(OrderStatus status) => status switch
        {
            OrderStatus.Pending_ => "waiting",
            OrderStatus.Processing_ => "working",
            OrderStatus.Completed_ => "done",
        };
#pragma warning restore IDE0072

        await Describe(OrderStatus.Pending).Should().BeEqualTo("waiting");
        await Describe(OrderStatus.Create(2)).Should().BeEqualTo("working");
        await Describe(OrderStatus.Completed).Should().BeEqualTo("done");
        await OrderStatus.All.Select(Describe).Should().BeEquivalentTo(["waiting", "working", "done"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Carry_The_Data_Of_Each_Member()
    {
        await OrderStatus.Pending.Label.Should().BeEqualTo("Waiting");
        await OrderStatus.Processing.Label.Should().BeEqualTo("In progress");
        await OrderStatus.Processing.Fee.Should().BeEqualTo(2.5m);
        await OrderStatus.Create(3).Label.Should().BeEqualTo("Done");
        await Role.Admin.Rank.Should().BeEqualTo(100);
        await Role.FromValue("auditor").Rank.Should().BeEqualTo(1);
    }

    [Test]
    public async Task Should_Let_A_Member_Override_Behaviour()
    {
        await OrderStatus.Pending.CanCancel.Should().BeTrue();
        await OrderStatus.Processing.CanCancel.Should().BeTrue();
        await OrderStatus.Completed.CanCancel.Should().BeFalse();
        await OrderStatus.Create(3).CanCancel.Should().BeFalse();
    }

    [Test]
    public async Task Should_Compare_Members_By_Value()
    {
        await ( OrderStatus.Pending == OrderStatus.Create(1) ).Should().BeTrue();
        await ( OrderStatus.Pending != OrderStatus.Completed ).Should().BeTrue();
        await OrderStatus.Pending.Equals(OrderStatus.Processing).Should().BeFalse();
        await OrderStatus.Pending.Equals((object)OrderStatus.Pending).Should().BeTrue();
        await OrderStatus.Pending.Equals(null).Should().BeFalse();
        await ( OrderStatus.Pending == null ).Should().BeFalse();
        await ( (OrderStatus?)null == null ).Should().BeTrue();
        await OrderStatus.Pending.GetHashCode().Should().BeEqualTo(OrderStatus.Create(1).GetHashCode());
        await ( (int)OrderStatus.Completed ).Should().BeEqualTo(3);
        await Priority.High.Value.Should().BeEqualTo(10);
        await OrderStatus.Processing.ToString("G", null).Should().BeEqualTo("Processing");
    }

    [Test]
    public async Task Should_Describe_Its_Members()
    {
        await OrderStatus.Description.IsSmartEnum.Should().BeTrue();
        await OrderStatus.Description.AllowedValues.Should().BeEquivalentTo(["completed", "pending", "processing"], CollectionOrdering.Matching);
        await Role.Description.AllowedValues.Should().BeEquivalentTo(["admin", "readOnlyAuditor", "user"], CollectionOrdering.Matching);
    }
}

/// <summary>
///     Smart enums declared as a <c>readonly partial record struct</c>: no member classes, but Match, Map and Switch, and a default value that is
///     none of the members.
/// </summary>
public class StructSmartEnumTests
{
    [Test]
    public async Task Should_List_Every_Member_In_Declaration_Order()
    {
        await DeliveryStatus.All.Should().BeEquivalentTo([DeliveryStatus.Pending, DeliveryStatus.Processing, DeliveryStatus.Completed], CollectionOrdering.Matching);
        await Size.All.Should().BeEquivalentTo([Size.Small, Size.Medium, Size.ExtraLarge], CollectionOrdering.Matching);
        // the list can't be changed through a cast
        await Assert.That(DeliveryStatus.All is DeliveryStatus[]).IsFalse();
        await Assert.That(DeliveryStatus.All is ICollection<DeliveryStatus> { IsReadOnly: false }).IsFalse();
    }

    [Test]
    public async Task Should_Find_A_Member_By_Its_Value()
    {
        await Accepts<DeliveryStatus, int>(2);
        await Accepts<Size, string>("xl");
        await DeliveryStatus.FromValue(2).Should().BeEqualTo(DeliveryStatus.Processing);
        await DeliveryStatus.TryFromValue(3, out var completed).Should().BeTrue();
        await completed.Should().BeEqualTo(DeliveryStatus.Completed);
        await Size.FromValue("m").Should().BeEqualTo(Size.Medium);

        var error = await Rejects<DeliveryStatus, int>(99, "UNKNOWN_MEMBER");
        await error.Message.Should().BeEqualTo("DeliveryStatus has no member with the value '99'.");
        await Rejects<Size, string>("xxl", "UNKNOWN_MEMBER");
        await Rejects<Size, string>(null!, "UNKNOWN_MEMBER");
        await DeliveryStatus.TryFromValue(int.MinValue, out _).Should().BeFalse();
    }

    [Test]
    public async Task Should_Find_A_Member_By_Its_Name()
    {
        await DeliveryStatus.FromName("Pending").Should().BeEqualTo(DeliveryStatus.Pending);
        await DeliveryStatus.FromName("pending", ignoreCase: true).Should().BeEqualTo(DeliveryStatus.Pending);
        await DeliveryStatus.TryFromName("pending", false, out _).Should().BeFalse();
        await DeliveryStatus.TryFromName("", true, out _).Should().BeFalse();
        await DeliveryStatus.TryFromName("  ", true, out _).Should().BeFalse();
        await DeliveryStatus.TryFromName(null, true, out _).Should().BeFalse();
        var exception = await Throws<DomainPrimitiveValidationException>(() => DeliveryStatus.FromName("Shipped"));
        await exception!.Error.Code.Should().BeEqualTo("UNKNOWN_MEMBER");
        await Size.FromName("ExtraLarge").Value.Should().BeEqualTo("xl");
    }

    [Test]
    public async Task Should_Name_Its_Members()
    {
        await DeliveryStatus.Completed.Name.Should().BeEqualTo("Completed");
        await DeliveryStatus.Completed.ToString().Should().BeEqualTo("Completed");
        await DeliveryStatus.Processing.ToString("G", null).Should().BeEqualTo("Processing");
        await Size.ExtraLarge.ToString().Should().BeEqualTo("ExtraLarge");
        // a member created from its value is the same member, with the same name
        await DeliveryStatus.Create(1).Name.Should().BeEqualTo("Pending");
        await Size.Create("s").Should().BeEqualTo(Size.Small);
    }

    [Test]
    public async Task Should_Parse_A_Name_Or_A_Value()
    {
        await DeliveryStatus.Parse("processing", null).Should().BeEqualTo(DeliveryStatus.Processing);
        await DeliveryStatus.Parse("3", null).Should().BeEqualTo(DeliveryStatus.Completed);
        await Size.Parse("medium", null).Should().BeEqualTo(Size.Medium);
        await DeliveryStatus.TryParse("unknown", null, out _).Should().BeFalse();
        await DeliveryStatus.TryParse(null, null, out _).Should().BeFalse();
        await Throws<FormatException>(() => DeliveryStatus.Parse("42", null));
    }

    [Test]
    public async Task Should_Compare_Members_By_Value()
    {
        await ( DeliveryStatus.Pending == DeliveryStatus.Create(1) ).Should().BeTrue();
        await ( DeliveryStatus.Pending != DeliveryStatus.Completed ).Should().BeTrue();
        await DeliveryStatus.Pending.Equals((object)DeliveryStatus.Pending).Should().BeTrue();
        await DeliveryStatus.Pending.GetHashCode().Should().BeEqualTo(DeliveryStatus.Create(1).GetHashCode());
        await ( (int)DeliveryStatus.Completed ).Should().BeEqualTo(3);
        await ( Size.ExtraLarge == Size.Create("xl") ).Should().BeTrue();
        await ( (string)Size.Medium ).Should().BeEqualTo("m");
    }

    [Test]
    public async Task Should_Match_Every_Member()
    {
        await DeliveryStatus.Processing.Match(() => "pending", () => "processing", () => "completed").Should().BeEqualTo("processing");
        await Size.Medium.Map(small => small.Value + "!", medium => medium.Label + "?", extraLarge => extraLarge.Value).Should().BeEqualTo("Medium?");

        var switched = "";
        DeliveryStatus.Completed.Switch(() => switched = "pending", () => switched = "processing", () => switched = "completed");
        await switched.Should().BeEqualTo("completed");

        // the default struct is no member
        await Throws<InvalidOperationException>(() => default(DeliveryStatus).Match(() => 1, () => 2, () => 3));
        await Throws<InvalidOperationException>(() => default(Size).Switch(() => { }, () => { }, () => { }));
    }

    [Test]
    public async Task Should_Have_No_Value_When_Default()
    {
        var status = default(DeliveryStatus);

        await status.IsDefault.Should().BeTrue();
        await status.Name.Should().BeEqualTo(string.Empty);
        await status.ToString().Should().BeEqualTo(string.Empty);
        await Throws<InvalidOperationException>(() => _ = status.Value);
        // equality and hashing don't throw
        await status.Should().BeEqualTo(default);
        await status.Should().NotBeEqualTo(DeliveryStatus.Pending);
        _ = status.GetHashCode();
        // a member isn't the default value
        await DeliveryStatus.Pending.IsDefault.Should().BeFalse();
        await Size.Small.IsDefault.Should().BeFalse();

        // the default of a smart enum with data has none of its data either
        var size = default(Size);
        await size.IsDefault.Should().BeTrue();
        await Assert.That(size.Label).IsNull();
        await size.Weight.Should().BeEqualTo(0m);
        await size.Should().NotBeEqualTo(Size.Small);
    }

    [Test]
    public async Task Should_Carry_The_Data_Of_Each_Member()
    {
        await Size.Small.Label.Should().BeEqualTo("Small");
        await Size.Small.Weight.Should().BeEqualTo(0.5m);
        await Size.Medium.Label.Should().BeEqualTo("Medium");
        await Size.Medium.Weight.Should().BeEqualTo(1m);
        await Size.ExtraLarge.Label.Should().BeEqualTo("Extra large");
        await Size.ExtraLarge.Weight.Should().BeEqualTo(2.25m);
        await Size.All.Select(static z => z.Weight).Should().BeEquivalentTo([0.5m, 1m, 2.25m], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Make_A_Member_With_Its_Data_From_Its_Value_Or_Its_Name()
    {
        // Create, TryCreate, FromValue, FromName and Parse return the member's own field, with its data
        await Size.Create("xl").Label.Should().BeEqualTo("Extra large");
        await Size.Create("xl").Weight.Should().BeEqualTo(2.25m);
        await Size.TryCreate("m", out var medium, out _).Should().BeTrue();
        await medium.Label.Should().BeEqualTo("Medium");
        await Size.FromValue("s").Weight.Should().BeEqualTo(0.5m);
        await Size.TryFromValue("s", out var small).Should().BeTrue();
        await small.Label.Should().BeEqualTo("Small");
        await Size.FromName("extralarge", ignoreCase: true).Label.Should().BeEqualTo("Extra large");
        await Size.Parse("xl", null).Weight.Should().BeEqualTo(2.25m);
        await Size.Parse("Medium", null).Label.Should().BeEqualTo("Medium");
        // a member made again is equal to the member, data and all
        await Size.Create("m").Should().BeEqualTo(Size.Medium);
    }

    [Test]
    public async Task Should_Describe_Its_Members()
    {
        await DeliveryStatus.Description.IsSmartEnum.Should().BeTrue();
        await DeliveryStatus.Description.AllowedValues.Should().BeEquivalentTo(["completed", "pending", "processing"], CollectionOrdering.Matching);
        await Size.Description.IsSmartEnum.Should().BeTrue();
        await Size.Description.AllowedValues.Should().BeEquivalentTo(["extraLarge", "medium", "small"], CollectionOrdering.Matching);
    }
}
