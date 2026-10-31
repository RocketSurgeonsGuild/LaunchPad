using Rocket.Surgery.LaunchPad.Grpc.Joints;
using Rocket.Surgery.LaunchPad.Grpc.Validation;

namespace FluentValidation.Grpc.Tests;

public class GrpcBoundaryTests
{
    // Grpc.dll still references FluentValidation through the Specular provider generated into it: it gets LaunchPad.FluentValidation (and
    // the joint's validator scan) through AspNetCore until RocketSurgeonsGuild/LaunchPad#3069 moves that out. None of its own code uses it.
    [Test]
    public void Grpc_Has_No_Validation_Code()
    {
        var grpc = typeof(GrpcJoint).Assembly;

        grpc.GetTypes().Select(x => x.Namespace).Distinct().ShouldNotContain("Rocket.Surgery.LaunchPad.Grpc.Validation");
        typeof(IValidatorErrorMessageHandler).Assembly.ShouldNotBe(grpc);
    }
}
