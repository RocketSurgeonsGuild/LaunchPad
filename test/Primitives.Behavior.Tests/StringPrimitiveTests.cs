using Rocket.Surgery.LaunchPad.Primitives;

using static Primitives.Behavior.Tests.PrimitiveAssert;

namespace Primitives.Behavior.Tests;

public class StringPrimitiveTests
{
    [Test]
    public async Task Should_Accept_And_Normalize_A_Value()
    {
        await Accepts<FirstName, string>("  Ada ", "Ada");
        await Accepts<LastName, string>("Lovelace");
        await Accepts<DisplayName, string>("  ada   king\tlovelace ", "ADA KING LOVELACE");
        await Accepts<Tag, string>(" DotNet ", "dotnet");
        await Accepts<Indented, string>("  value  ", "value  ");
        await Accepts<Ragged, string>("  value  ", "  value");
    }

    [Test]
    public async Task Should_Reject_A_Value_Outside_Its_Length()
    {
        await Rejects<FirstName, string>("   ", "LENGTH");
        await Rejects<LastName, string>(new string('a', 101), "LENGTH");
        await Rejects<Pin, string>("123", "LENGTH");
        await Accepts<Pin, string>("1234");
        await Rejects<Pin, string>("12345", "LENGTH");
        await Rejects<Initials, string>("A", "LENGTH");
        await Accepts<Initials, string>("ABCDE");
        await Rejects<Initials, string>("ABCDEF", "LENGTH");
    }

    [Test]
    public async Task Should_Name_The_Bounds_In_Its_Errors()
    {
        var error = await Rejects<FirstName, string>("", "LENGTH");

        await error.Message.Should().BeEqualTo("FirstName must be at least 1 character(s). Got 0.");
    }

    [Test]
    public async Task Should_Reject_Null()
    {
        await Rejects<FirstName, string>(null!, "NULL_INPUT");
        await Rejects<LastName, string>(null!, "NULL_INPUT");
        await Rejects<Unconstrained, string>(null!, "NULL_INPUT");
    }

    [Test]
    public async Task Should_Reject_An_Empty_Value_When_Not_Empty()
    {
        await Rejects<DisplayName, string>(" \t ", "EMPTY");
        // an unconstrained string accepts an empty value
        await Accepts<Unconstrained, string>("");
    }

    [Test]
    public async Task Should_Apply_The_Default_Maximum_Length()
    {
        // LaunchPadPrimitivesMaxLength, 4096 unless the project says otherwise
        await Accepts<Unconstrained, string>(new string('a', 4096));
        var error = await Rejects<Unconstrained, string>(new string('a', 4097), "LENGTH");
        await error.Message.Should().Contain("at most 4096");

        // far past the limit the value is rejected before it is normalized
        await Rejects<Unconstrained, string>(new string('a', 1_000_000), "LENGTH");
    }

    [Test]
    public async Task Should_Check_Every_Pattern_With_Its_Own_Error()
    {
        var format = await Rejects<ProductCode, string>("AB-12", "FORMAT");
        await format.Message.Should().BeEqualTo("Must be in format XX-0000");
        await Accepts<ProductCode, string>(" AB-1234 ", "AB-1234");

        var lower = await Rejects<Word, string>("ABC", "LOWER_ONLY");
        await lower.Message.Should().BeEqualTo("Word has an invalid format.");
        var shortWord = await Rejects<Word, string>("ab", "TOO_SHORT");
        await shortWord.Message.Should().BeEqualTo("Three or more.");
        await Accepts<Word, string>("abc");
    }

    [Test]
    public async Task Should_Use_The_Error_Code_And_Message_Of_A_Rule()
    {
        var empty = await Rejects<ApiSecret, string>("", "REQUIRED");
        await empty.Message.Should().BeEqualTo("A secret is required.");

        var weak = await Rejects<ApiSecret, string>("too-short", "WEAK");
        // the message doesn't repeat the value: a rejected secret is not leaked
        await weak.Message.Should().NotContain("too-short");
        await Accepts<ApiSecret, string>(new string('s', 32));
    }

    [Test]
    public async Task Should_Run_The_Normalizers_And_Validators_Of_The_Project()
    {
        await Accepts<Shout, string>("hello", "HELLO!");
        // normalized before it is validated: the x is upper case by the time NoLetterXValidator sees it
        var error = await Rejects<Shout, string>("box", "NO_X");
        await error.Message.Should().BeEqualTo("No X allowed.");
        await Accepts<ShoutClass, string>("  hi!  ", "HI!");
    }

    [Test]
    public async Task Should_Normalize_To_Form_C()
    {
        // e + combining acute accent is the same text as é
        var decomposed = "José";
        var name = await Accepts<FirstName, string>(decomposed, "José");

        await name.Should().BeEqualTo(FirstName.Create("José"));
    }

    [Test]
    public async Task Should_Reject_Unpaired_Surrogates()
    {
        await Rejects<FirstName, string>("a\ud800b", "INVALID_UNICODE");
        // a surrogate pair is fine
        await Accepts<FirstName, string>("🚀");
    }

    [Test]
    public async Task Should_Have_No_Value_When_Default()
    {
        var name = default(FirstName);

        await name.IsDefault.Should().BeTrue();
        await name.ToString().Should().BeEqualTo(string.Empty);
        await Throws<InvalidOperationException>(() => _ = name.Value);
        await Throws<InvalidOperationException>(() => _ = (string)name);
        await FirstName.Create("Ada").IsDefault.Should().BeFalse();
    }

    [Test]
    public async Task Should_Throw_The_Error_It_Rejects_With()
    {
        var exception = await Throws<DomainPrimitiveValidationException>(() => FirstName.Create(""));

        await exception!.Error.Code.Should().BeEqualTo("LENGTH");
        await exception.ParamName.Should().BeEqualTo("value");
        await exception.Message.Should().StartWith("[LENGTH] FirstName must be at least 1 character(s).");
    }

    [Test]
    public async Task Should_Describe_Its_Rules()
    {
        await FirstName.Description.MinLength.Should().BeEqualTo(1);
        await FirstName.Description.MaxLength.Should().BeEqualTo(100);
        await ProductCode.Description.Patterns.Should().BeEquivalentTo([@"^[A-Z]{2}-\d{4}$"]);
        await DisplayName.Description.MinLength.Should().BeEqualTo(1);
        await EmailAddress.Description.Format.Should().BeEqualTo("email");
        await EmailAddress.Description.MaxLength.Should().BeEqualTo(320);
        await Website.Description.Format.Should().BeEqualTo("uri");
        await FirstName.Description.Type.Should().BeEqualTo(typeof(FirstName));
        await FirstName.Description.ValueType.Should().BeEqualTo(typeof(string));
        await FirstName.PrimitiveName.Should().BeEqualTo("FirstName");
    }

    [Test]
    public async Task Should_List_The_Codes_Of_Its_Errors()
    {
        await FirstName.Errors.Length.Should().BeEqualTo("LENGTH");
        await FirstName.Errors.NullInput.Should().BeEqualTo("NULL_INPUT");
        await ApiSecret.Errors.Required.Should().BeEqualTo("REQUIRED");
        await ApiSecret.Errors.Weak.Should().BeEqualTo("WEAK");
        await Word.Errors.LowerOnly.Should().BeEqualTo("LOWER_ONLY");
    }

    [Test]
    public async Task Should_Complete_A_Nested_Type() => await Accepts<Catalog.Product.NestedSlug, string>("Hello-World", "hello-world");
}

public class StringKindTests
{
    [Test]
    public async Task Should_Validate_An_Email_Address()
    {
        await Accepts<EmailAddress, string>("  Ada@Example.COM ", "ada@example.com");
        await Accepts<EmailAddress, string>("first.last+tag@sub.example.co");
        await Rejects<EmailAddress, string>("not-an-email", "FORMAT");
        await Rejects<EmailAddress, string>("a@b@c.com", "FORMAT");
        await Rejects<EmailAddress, string>("", "EMPTY");
        await Rejects<EmailAddress, string>(new string('a', 315) + "@x.com", "LENGTH");
        await Rejects<ShortEmail, string>(new string('a', 25) + "@x.com", "LENGTH");
    }

    [Test]
    public async Task Should_Validate_A_Phone_Number()
    {
        await Accepts<PhoneNumber, string>(" +14155552671 ", "+14155552671");
        await Rejects<PhoneNumber, string>("4155552671", "FORMAT");
        await Rejects<PhoneNumber, string>("+0123", "FORMAT");
    }

    [Test]
    public async Task Should_Validate_A_Url_And_Its_Scheme()
    {
        await Accepts<Website, string>("https://example.com/path?q=1");
        await Accepts<Website, string>("http://example.com");
        await Rejects<Website, string>("ftp://example.com", "FORMAT");
        await Rejects<Website, string>("/relative", "FORMAT");

        await Accepts<SecureFtpUrl, string>("ftp://files.example.com");
        await Accepts<SecureFtpUrl, string>("HTTPS://example.com");
        var error = await Rejects<SecureFtpUrl, string>("http://example.com", "FORMAT");
        await error.Message.Should().BeEqualTo("SecureFtpUrl must be an absolute HTTPS/FTP URL.");
    }

    [Test]
    public async Task Should_Validate_A_Slug()
    {
        await Accepts<ArticleSlug, string>(" My-First-Post ", "my-first-post");
        await Rejects<ArticleSlug, string>("my post", "FORMAT");
        await Rejects<ArticleSlug, string>("-leading", "FORMAT");
        await Rejects<ArticleSlug, string>("double--dash", "FORMAT");
    }

    [Test]
    public async Task Should_Validate_Codes_Against_Their_Registries()
    {
        await Accepts<Country, string>(" us ", "US");
        await Accepts<CountryClass, string>("ca", "CA");
        await Rejects<Country, string>("ZZ", "FORMAT");
        await Rejects<Country, string>("USA", "LENGTH");
        await Accepts<Currency, string>("eur", "EUR");
        await Rejects<Currency, string>("ABC", "FORMAT");
        await Rejects<Currency, string>("EU", "LENGTH");
        await Accepts<Language, string>("EN", "en");
        await Rejects<Language, string>("e1", "FORMAT");
    }

    [Test]
    public async Task Should_Validate_A_Username()
    {
        await Accepts<UserName, string>("ada_lovelace.1");
        await Rejects<UserName, string>("ab", "LENGTH");
        await Rejects<UserName, string>("ada lovelace", "FORMAT");
        await Accepts<Handle, string>("abcde");
        await Rejects<Handle, string>("abcd", "LENGTH");
        // the kind's code applies to the kind's own checks
        await Rejects<Handle, string>("abc de", "HANDLE");
    }

    [Test]
    public async Task Should_Accept_Any_Password_Hash_But_An_Empty_One()
    {
        await Accepts<PasswordHashValue, string>("$2a$12$R9h/cIPz0gi.URNNX3kh2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUW");
        var error = await Rejects<PasswordHashValue, string>("   ", "EMPTY");
        await error.Message.Should().NotContain("   ");
    }

    [Test]
    public async Task Should_Validate_Hardware_And_Network_Codes()
    {
        await Accepts<ThemeColor, string>("#ff00aa", "#FF00AA");
        await Accepts<ThemeColor, string>("#abc", "#ABC");
        await Rejects<ThemeColor, string>("ff00aa", "FORMAT");
        await Accepts<ClientIp, string>("192.168.0.1");
        await Accepts<ClientIp, string>("::1");
        await Accepts<ClientIp, string>("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
        await Rejects<ClientIp, string>("256.1.1.1", "FORMAT");
        await Accepts<DeviceMac, string>("00:1a:2b:3c:4d:5e", "00:1A:2B:3C:4D:5E");
        await Rejects<DeviceMac, string>("00:1A:2B:3C:4D", "FORMAT");
        await Accepts<VehicleVin, string>("1hgcm82633a004352", "1HGCM82633A004352");
        // I, O and Q are never in a VIN
        await Rejects<VehicleVin, string>("1HGCM82633A00435O", "FORMAT");
    }

    [Test]
    public async Task Should_Validate_An_Iban_And_Its_Check_Digits()
    {
        await Accepts<BankAccount, string>("gb82west12345698765432", "GB82WEST12345698765432");
        await Accepts<BankAccount, string>("DE89370400440532013000");
        var error = await Rejects<BankAccount, string>("GB82WEST12345698765433", "FORMAT");
        await error.Message.Should().Contain("check digits");
        await Rejects<BankAccount, string>("GB82", "FORMAT");
    }

    [Test]
    public async Task Should_Validate_An_Isbn()
    {
        await Accepts<BookIsbn, string>("978-0-306-40615-7");
        await Accepts<BookIsbn, string>("0306406152");
        await Rejects<BookIsbn, string>("12345", "FORMAT");
    }
}
