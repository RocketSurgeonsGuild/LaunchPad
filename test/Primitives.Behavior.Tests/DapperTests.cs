using System.Data;

using Dapper;

using Microsoft.Data.Sqlite;

namespace Primitives.Behavior.Tests;

/// <summary>
///     The Dapper type handlers the registry generator registers from a module initializer when the project references Dapper: a domain
///     primitive is stored as its value and read back through its rules.
/// </summary>
public class DapperTests
{
    private static async Task<SqliteConnection> Open()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            """
            create table Customers (
                Id text not null primary key,
                Email text not null,
                Family text null,
                Score integer not null,
                Rating text not null,
                Status integer not null,
                Role text not null,
                Delivery integer not null,
                Size text not null,
                Since text not null,
                Opens text not null
            )
            """
        );
        return connection;
    }

    private sealed record CustomerRow(
        CustomerId Id,
        EmailAddress Email,
        LastName? Family,
        Score Score,
        MovieRating Rating,
        OrderStatus Status,
        Role Role,
        DeliveryStatus Delivery,
        Size Size,
        CenturyDay Since,
        OfficeHour Opens
    );

    [Test]
    public async Task Should_Write_And_Read_Domain_Primitives()
    {
        await using var connection = await Open();
        var customer = new CustomerRow(
            CustomerId.Create(),
            EmailAddress.Create("ada@example.com"),
            null,
            Score.Create(42),
            MovieRating.Create(4.25m),
            OrderStatus.Completed,
            Role.ReadOnlyAuditor,
            DeliveryStatus.Processing,
            Size.ExtraLarge,
            CenturyDay.Create(new DateOnly(2026, 11, 23)),
            OfficeHour.Create(new TimeOnly(9, 30))
        );

        await connection.ExecuteAsync(
            "insert into Customers values (@Id, @Email, @Family, @Score, @Rating, @Status, @Role, @Delivery, @Size, @Since, @Opens)",
            customer
        );
        var read = await connection.QuerySingleAsync<CustomerRow>("select * from Customers");

        await read.Should().BeEqualTo(customer);
        // a struct smart enum read back is the member, with its data
        await read.Size.Label.Should().BeEqualTo("Extra large");
    }

    [Test]
    public async Task Should_Query_By_A_Domain_Primitive()
    {
        await using var connection = await Open();
        var id = CustomerId.Create();
        await connection.ExecuteAsync(
            "insert into Customers values (@Id, 'ada@example.com', 'Lovelace', 1, '1', 1, 'admin', 2, 'm', '2026-01-01', '09:00:00')",
            new { Id = id }
        );

        var email = await connection.QuerySingleAsync<EmailAddress>("select Email from Customers where Id = @id", new { id });
        var family = await connection.QuerySingleAsync<LastName>("select Family from Customers where Email = @email", new { email });
        var count = await connection.ExecuteScalarAsync<long>("select count(*) from Customers where Status = @status", new { status = OrderStatus.Pending });
        var delivered = await connection.ExecuteScalarAsync<long>("select count(*) from Customers where Delivery = @delivery", new { delivery = DeliveryStatus.Processing });
        var size = await connection.QuerySingleAsync<Size>("select Size from Customers where Id = @id", new { id });

        await email.Should().BeEqualTo(EmailAddress.Create("ada@example.com"));
        await family.Should().BeEqualTo(LastName.Create("Lovelace"));
        await count.Should().BeEqualTo(1);
        await delivered.Should().BeEqualTo(1);
        await size.Should().BeEqualTo(Size.Medium);
        await size.Weight.Should().BeEqualTo(1m);
    }

    [Test]
    public async Task Should_Write_A_Smart_Enum_Member_Of_A_Dynamic_Parameter()
    {
        await using var connection = await Open();
        await connection.ExecuteAsync(
            "insert into Customers values ('a', 'ada@example.com', null, 1, '1', 2, 'admin', 2, 'm', '2026-01-01', '09:00:00')"
        );

        // Dapper looks the handler up by the runtime type of the value, which is the member's own class for a closed smart enum.
        var parameters = new DynamicParameters();
        parameters.Add("status", OrderStatus.Processing);
        parameters.Add("role", Role.Admin);
        parameters.Add("size", Size.Medium);
        var count = await connection.ExecuteScalarAsync<long>(
            "select count(*) from Customers where Status = @status and Role = @role and Size = @size",
            parameters
        );

        await count.Should().BeEqualTo(1);
    }

    [Test]
    public async Task Should_Write_Null_For_A_Missing_Value()
    {
        await using var connection = await Open();

        var nullFamily = await connection.ExecuteScalarAsync<long>("select @family is null", new { family = (LastName?)null });
        var nullScore = await connection.ExecuteScalarAsync<long>("select @score is null", new { score = default(Score) });
        var family = await connection.QuerySingleAsync<LastName?>("select null");

        await nullFamily.Should().BeEqualTo(1);
        await nullScore.Should().BeEqualTo(1);
        await family.Should().BeNull();
    }

    [Test]
    public async Task Should_Not_Read_A_Value_Its_Rules_Reject()
    {
        await using var connection = await Open();

        var exception = await PrimitiveAssert.Throws<DataException>(() => connection.QuerySingle<Score>("select 500"));

        await exception!.Message.Should().Contain("[RANGE]");
    }
}
