namespace NodaTime.Mapping.Tests.Helpers;

public abstract class MapperTestBase() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    protected SettingsTask VerifyMethod(MethodResult result, object mapper, params object[] instances) => Verify(result.Map(mapper, instances)).UseParameters(result.ToString());

    protected SettingsTask VerifyEachMethod(MethodResult result, object mapper, params object[] instances) => Verify(result.MapEach(mapper, instances)).UseParameters(result.ToString());
}
