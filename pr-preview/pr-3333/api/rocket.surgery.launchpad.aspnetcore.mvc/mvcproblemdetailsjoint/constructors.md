# MvcProblemDetailsJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [MvcProblemDetailsJoint](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcproblemdetailsjoint.md)
- Kind: `Constructors`
- Members: `1`

Fills in the title and type of problem details from the client error mapping of MVC's `Mvc.ApiBehaviorOptions`, and turns off MVC's automatic 400 for an invalid model state (LaunchPad validates requests itself). Implements the `Joints.IServiceJoint`

<a id="constructor"></a>

## MvcProblemDetailsJoint

- Name: `Constructor`

```csharp
public class MvcProblemDetailsJoint
{
    public MvcProblemDetailsJoint()
    {
        // ...
    }
}
```
