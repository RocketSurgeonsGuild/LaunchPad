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
    public async Task Should_Match_Every_Member()
    {
        await OrderStatus.Processing.Match(() => "pending", () => "processing", () => "completed").Should().BeEqualTo("processing");
        await Role.User.Map(admin => admin.Value + "!", user => user.Value + "?", auditor => auditor.Value).Should().BeEqualTo("user?");

        var switched = "";
        OrderStatus.Completed.Switch(() => switched = "pending", () => switched = "processing", () => switched = "completed");
        await switched.Should().BeEqualTo("completed");

        // the default struct is no member
        await Throws<InvalidOperationException>(() => default(OrderStatus).Match(() => 1, () => 2, () => 3));
    }

    [Test]
    public async Task Should_Have_No_Value_When_Default()
    {
        var status = default(OrderStatus);

        await status.IsDefault.Should().BeTrue();
        await status.Name.Should().BeEqualTo(string.Empty);
        await Throws<InvalidOperationException>(() => _ = status.Value);
        // equality and hashing don't throw
        await status.Should().BeEqualTo(default);
        await status.Should().NotBeEqualTo(OrderStatus.Pending);
        _ = status.GetHashCode();
    }

    [Test]
    public async Task Should_Describe_Its_Members()
    {
        await OrderStatus.Description.IsSmartEnum.Should().BeTrue();
        await OrderStatus.Description.AllowedValues.Should().BeEquivalentTo(["completed", "pending", "processing"], CollectionOrdering.Matching);
        await Role.Description.AllowedValues.Should().BeEquivalentTo(["admin", "readOnlyAuditor", "user"], CollectionOrdering.Matching);
    }
}
