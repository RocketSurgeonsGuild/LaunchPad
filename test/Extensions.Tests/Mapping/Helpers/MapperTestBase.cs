namespace Extensions.Tests.Mapping.Helpers;

public abstract class MapperTestBase() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    protected SettingsTask VerifyMethod(MethodResult result, object mapper, params object[] instances)
    {
        return Verify(result.Map(mapper, instances)).UseParameters(result.ToString());
    }

    protected SettingsTask VerifyEachMethod(MethodResult result, object mapper, params object[] instances)
    {
        return Verify(result.MapEach(mapper, instances)).UseParameters(result.ToString());
    }
}
