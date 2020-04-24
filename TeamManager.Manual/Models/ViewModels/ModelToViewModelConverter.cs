using System;

namespace TeamManager.Manual.Web.Models.ViewModels
{
    public static class ModelToViewModelConverter
    {
        public static void Convert(object model, object viewModel)
        {
            if(model == null)
            {
                throw new NullReferenceException();
            }

            if(viewModel == null)
            {
                throw new NullReferenceException();
            }

            if(viewModel.GetType().BaseType != model.GetType())
            {
                throw new InvalidCastException();
            }

            foreach (var property in model.GetType().GetProperties())
            {
                viewModel.GetType().GetProperty(property.Name).SetValue(viewModel, property.GetValue(model));
            }
        }
    }
}
