using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Composition;

namespace AspNetCore.Tests.Restful;

public class RestfulApiMethodBuilderTests(ITestOutputHelper testOutputHelper) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testOutputHelper))
{
    [Fact]
    public void Should_Have_Method()
    {
        IRestfulApiMethodMatcher builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.Method.Should().Be(RestfulApiMethod.List);
    }

    [Theory]
    [InlineData("MyActionName")]
    [InlineData("myActionName")]
    [InlineData("myactionname")]
    [InlineData("MYACTIONNAME")]
    public void Matches_Method_Name(string nameToMatch)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchName(nameToMatch);

        builder.IsMatch(CreateActionModel(nameof(MyActionName))).Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(OneParameter), 1)]
    [InlineData(nameof(TwoParameter), 2)]
    [InlineData(nameof(ThreeParameter), 3)]
    public void Matches_Method_ParameterCount(string actionName, int count)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchParameterCount(count);

        builder.IsMatch(CreateActionModel(actionName)).Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(OneParameter), 0)]
    [InlineData(nameof(TwoParameter), 1)]
    [InlineData(nameof(ThreeParameter), 4)]
    public void Not_Matches_Method_ParameterCount(string actionName, int count)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchParameterCount(count);

        builder.IsMatch(CreateActionModel(actionName)).Should().BeFalse();
    }

    [Theory]
    [InlineData("My")]
    [InlineData("my")]
    [InlineData("MY")]
    public void Matches_Method_Prefix(string prefix)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchPrefix(prefix);

        builder.IsMatch(CreateActionModel(nameof(MyActionName))).Should().BeTrue();
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("name")]
    [InlineData("NAME")]
    public void Matches_Method_Suffix(string suffix)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchSuffix(suffix);

        builder.IsMatch(CreateActionModel(nameof(MyActionName))).Should().BeTrue();
    }


    [Theory]
    [InlineData(nameof(OneParameter), 0)]
    [InlineData(nameof(TwoParameter), 1)]
    [InlineData(nameof(ThreeParameter), 2)]
    public void Matches_Parameter_Existence(string methodName, int parameterIndex)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.HasParameter(parameterIndex);

        builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
    }


    [Theory]
    [InlineData(nameof(OneParameter), 0, typeof(Guid))]
    [InlineData(nameof(TwoParameter), 1, typeof(object))]
    [InlineData(nameof(ThreeParameter), 2, typeof(int))]
    [InlineData(nameof(MyActionName), 1, typeof(IBaseRequest))]
    public void Matches_Parameter_Type(string methodName, int parameterIndex, Type type)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchParameterType(parameterIndex, type);

        builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
    }


    [Theory]
    [InlineData(nameof(OneParameter), 0, "id")]
    [InlineData(nameof(TwoParameter), 1, "two")]
    [InlineData(nameof(ThreeParameter), 2, "three")]
    public void Matches_Parameter_Name(string methodName, int parameterIndex, string parameterName)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchParameterName(parameterIndex, parameterName);

        builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
    }


    [Theory]
    [InlineData(nameof(MyActionName), 1, "Request")]
    public void Matches_Parameter_Suffix(string methodName, int parameterIndex, string parameterName)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchParameterSuffix(parameterIndex, parameterName);

        builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
    }


    [Theory]
    [InlineData(nameof(MyActionName), 1, "base")]
    public void Matches_Parameter_Prefix(string methodName, int parameterIndex, string parameterName)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
        builder.MatchParameterPrefix(parameterIndex, parameterName);

        builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(MyActionName), true)]
    [InlineData(nameof(ThreeParameter), false)]
    [InlineData(nameof(MyComplexAction), true)]
    public void Matches_Index_Based_Parameters(string name, bool toBe)
    {
        var builder = new RestfulApiMethodBuilder(RestfulApiMethod.Create);
        builder
           .MatchParameterName(^2, "id")
           .MatchParameterType(^1, typeof(IBaseRequest));

        builder.IsMatch(CreateActionModel(name)).Should().Be(toBe);
    }

    [Theory]
    [ClassData(typeof(Matching))]
    public void Matches_Default_Names(Type type, string methodName)
    {
        var builders = new RestfulApiOptions().Builders.Where(z => z.IsValid());
        builders.Any(builder => builder.IsMatch(CreateActionModel(methodName, fromType: type)))
                .Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(NonMatching))]
    public void Does_Not_Match_Default_Names(Type type, string methodName)
    {
        var builders = new RestfulApiOptions().Builders;
        builders.All(builder => builder.IsMatch(CreateActionModel(methodName, fromType: type)))
                .Should().BeFalse();
    }

    private ActionModel CreateActionModel(string methodName, string? name = null, Type? fromType = null)
    {
        return new ActionModel(
            ( fromType ?? GetType() ).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!,
            new List<object>()
        )
        {
            ActionName = name ?? ( fromType is null ? nameof(MyActionName) : methodName )
        };
    }

    private Task<ActionResult<object>> MyComplexAction(int someIdentifier, double someOtherIdentifier, Guid id, RequestResponse baseRequest)
    {
        throw new NotImplementedException();
    }

    private Task<ActionResult<object>> MyActionName(Guid id, Request baseRequest)
    {
        throw new NotImplementedException();
    }

    private Task<ActionResult<object>> OneParameter(Guid id)
    {
        throw new NotImplementedException();
    }

    private Task<ActionResult<object>> TwoParameter(Guid id, object two)
    {
        throw new NotImplementedException();
    }

    private Task<ActionResult<object>> ThreeParameter(long id, object two, int three)
    {
        throw new NotImplementedException();
    }

    private sealed class Request : IRequest;

    private sealed class RequestResponse : IRequest<string>;

    private sealed class Matching : TheoryData<Type, string>
    {
        public Matching()
        {
            foreach (var (type, name) in
                     getActions<ListActions>()
                        .Concat(getActions<GetActions>())
                        .Concat(getActions<PostActions>())
                        .Concat(getActions<PutActions>())
                        .Concat(getActions<DeleteActions>())
                    )
            {
                Add(type, name);
            }

            static IEnumerable<(Type type, string name)> getActions<T>()
            {
                return typeof(T)
                      .GetMethods()
                      .Select(z => z.Name)
                      .Except(typeof(T).BaseType!.GetMethods().Select(z => z.Name))
                      .Select(z => ( typeof(T), z ));
            }
        }

        [PublicAPI]
        public sealed class ListActions
        {
            public Task<ActionResult<object>> ListPeople(Request listRequest)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> SearchPeople(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> ListPeopleIndex(int orgId, int groupId, Request listRequest)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> SearchPeopleIndex(int orgId, Request request)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public sealed class GetActions
        {
            public Task<ActionResult<object>> GetPerson(Request personRequest)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> FindPerson(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> FetchPerson(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> ReadPerson(Request personRequest)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> GetPersonIndex(Request personRequest)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> FindPersonIndex(int orgId, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> FetchPersonIndex(int orgId, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> ReadPersonIndex(int orgId, int groupId, Request personRequest)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public sealed class PostActions
        {
            public Task<ActionResult<object>> PostPerson(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> CreatePerson(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> AddPerson(Request personRequest)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PostPersonIndex(int orgId, int groupId, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> CreatePersonIndex(int orgId, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> AddPersonIndex(int orgId, int groupId, Request personRequest)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public sealed class PutActions
        {
            public Task<ActionResult<object>> PutPerson(Guid personId, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> EditPerson(int id, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> UpdatePerson(long personId, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PutOtherPerson(Guid personId, object personModel)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> EditOtherPerson(int id, object model)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> UpdateOtherPerson(long personId, object personRequest)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PutPersonIndex(int orgId, Guid id, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> EditPersonIndex(int orgId, int id, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> UpdatePersonIndex(int orgId, int groupId, long id, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PutOtherPersonIndex(int orgId, int groupId, Guid personId, object personModel)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> EditOtherPersonIndex(int orgId, int groupId, int id, object model)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> UpdateOtherPersonIndex(int orgId, int groupId, long personId, object personRequest)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public sealed class DeleteActions
        {
            public Task<ActionResult<object>> DeletePerson(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> RemovePerson(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> DeleteOtherPerson(int personId)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> RemoveOtherPerson(Guid personId)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> DeletePersonIndex(int org, int groupId, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> RemovePersonIndex(int org, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> DeleteOtherPersonIndex(int org, int personId)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> RemoveOtherPersonIndex(int org, Guid personId)
            {
                throw new NotImplementedException();
            }
        }
    }

    private sealed class NonMatching : TheoryData<Type, string>
    {
        public NonMatching()
        {
            foreach (var (type, name) in
                     getActions<ListActions>()
                        .Concat(getActions<GetActions>())
                        .Concat(getActions<PostActions>())
                        .Concat(getActions<PutActions>())
                        .Concat(getActions<DeleteActions>())
                    )
            {
                Add(type, name);
            }

            static IEnumerable<(Type type, string name)> getActions<T>()
            {
                return typeof(T)
                      .GetMethods()
                      .Select(z => z.Name)
                      .Except(typeof(T).BaseType!.GetMethods().Select(z => z.Name))
                      .Select(z => ( typeof(T), z ));
            }
        }

        [PublicAPI]
        public sealed  class ListActions
        {
            public Task<ActionResult<object>> PeopleList(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PeopleSearch(Request request)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public sealed  class GetActions
        {
            public Task<ActionResult<object>> PeopleGet(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PeopleFind(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PeopleFetch(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PeopleRead(Request request)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public sealed  class PostActions
        {
            public Task<ActionResult<object>> PersonPost(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PersonCreate(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PersonAdd(Request request)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public sealed class PutActions
        {
            public Task<ActionResult<object>> PersonPut(Guid id, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PersonEdit(int id, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PersonUpdate(long id, Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PutPerson(Guid id, object asdf)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> EditPerson(int id, object asdf)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> UpdatePerson(long id, object asdf)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public sealed  class DeleteActions
        {
            public Task<ActionResult<object>> PersonDelete(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PersonRemove(Request request)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PersonDel(int id)
            {
                throw new NotImplementedException();
            }

            public Task<ActionResult<object>> PersonRem(Guid id)
            {
                throw new NotImplementedException();
            }
        }
    }
}
