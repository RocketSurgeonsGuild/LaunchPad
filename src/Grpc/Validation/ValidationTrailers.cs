using System;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    [Serializable]
    public class ValidationTrailers
    {
        public string PropertyName { get; set; }

        public string ErrorMessage { get; set; }

        public object AttemptedValue { get; set; }
    }
}