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
            CenturyDay.Create(new DateOnly(2026, 11, 23)),
            OfficeHour.Create(new TimeOnly(9, 30))
        );

        await connection.ExecuteAsync(
            "insert into Customers values (@Id, @Email, @Family, @Score, @Rating, @Status, @Role, @Since, @Opens)",
            customer
        );
        var read = await connection.QuerySingleAsync<CustomerRow>("select * from Customers");

        await read.Should().BeEqualTo(customer);
    }

    [Test]
    public async Task Should_Query_By_A_Domain_Primitive()
    {
        await using var connection = await Open();
        var id = CustomerId.Create();
        await connection.ExecuteAsync(
            "insert into Customers values (@Id, 'ada@example.com', 'Lovelace', 1, '1', 1, 'admin', '2026-01-01', '09:00:00')",
            new { Id = id }
        );

        var email = await connection.QuerySingleAsync<EmailAddress>("select Email from Customers where Id = @id", new { id });
        var family = await connection.QuerySingleAsync<LastName>("select Family from Customers where Email = @email", new { email });
        var count = await connection.ExecuteScalarAsync<long>("select count(*) from Customers where Status = @status", new { status = OrderStatus.Pending });

        await email.Should().BeEqualTo(EmailAddress.Create("ada@example.com"));
        await family.Should().BeEqualTo(LastName.Create("Lovelace"));
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
