using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumHelper<T> 
     where T : Enum
{
    public static IEnumerable<SelectListItem> GetSelectListItems(IStringLocalizer localizer = null)
    {
        IList<SelectListItem> selectListItems = new List<SelectListItem>();
        Array values = typeof(T).GetEnumValues();
        for (int i = 0; i < values.Length; i++)
        {
            string label = GetDisplayValue((T)values.GetValue(i));
            if (localizer != null)
            {
                label = localizer[label];
            }

            selectListItems.Add(new SelectListItem(label, values.GetValue(i).ToString()));
        }

        return selectListItems;
    }

    public static string GetDisplayValue(T value, IStringLocalizer localizer = null)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        DisplayAttribute[] descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

        string response = string.Empty;
        if (descriptionAttributes != null &&  descriptionAttributes.Length > 0)
            if (descriptionAttributes[0] != null && descriptionAttributes[0].ResourceType != null)
            response = lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

        if (descriptionAttributes == null) response = string.Empty;
        else  response = (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();

        if (localizer != null)
            return localizer[response];

        return response;
    }

    private static string lookupResource(Type resourceManagerProvider, string resourceKey)
    {
        foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
            {
                System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                return resourceManager.GetString(resourceKey);
            }
        }

        return resourceKey; // Fallback with the key name
    }
}