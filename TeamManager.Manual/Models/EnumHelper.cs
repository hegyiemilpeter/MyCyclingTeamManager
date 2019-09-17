using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumHelper<T> 
     where T : Enum
{
    public static IEnumerable<SelectListItem> GetSelectListItems()
    {
        IList<SelectListItem> selectListItems = new List<SelectListItem>();
        Array values = typeof(T).GetEnumValues();
        for (int i = 0; i < values.Length; i++)
        {
            selectListItems.Add(new SelectListItem(GetDisplayValue((T)values.GetValue(i)), values.GetValue(i).ToString()));
        }

        return selectListItems;
    }

    public static string GetDisplayValue(T value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        var descriptionAttributes = fieldInfo.GetCustomAttributes(
            typeof(DisplayAttribute), false) as DisplayAttribute[];

        if (descriptionAttributes[0].ResourceType != null)
            return lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

        if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
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