using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.ModelBinding
{
    public class IgnoreBindingMetadataProvider :
        IBindingMetadataProvider,
        IDisplayMetadataProvider,
        IValidationMetadataProvider
    {
        // public void CreateBindingMetadata(BindingMetadataProviderContext context)
        // {
        //     context.BindingMetadata.IsBindingAllowed = false;
        // }
        //
        // public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        // {
        //     context.DisplayMetadata.ShowForDisplay = false;
        //     context.DisplayMetadata.ShowForEdit = false;
        // }
        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
            if (context.Key.PropertyInfo?.GetCustomAttribute<BindableAttribute>()?.Bindable == false)
            {
                context.BindingMetadata.IsBindingAllowed = false;
            }
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context) { }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context) { }
    }
}