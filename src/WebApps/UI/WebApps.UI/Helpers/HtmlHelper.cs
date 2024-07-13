using System.ComponentModel;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApps.UI.Helpers;

public static class HtmlHelper
{
    public static IHtmlContent EnumDropDownListFor<TEnum>(this IHtmlHelper htmlHelper, string name, TEnum selectedValue, object? htmlAttributes = null)
        where TEnum : struct, Enum
    {
        var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        var selectList = new List<SelectListItem>();
        foreach (var enumValue in enumValues)
        {
            selectList.Add(new SelectListItem
            {
                Value = enumValue.ToString(),
                Text = enumValue.ToString(),
                Selected = enumValue.Equals(selectedValue)
            });
        }

        var htmlAttributesDictionary = AnonymousObjectToHtmlAttributes(htmlAttributes);

        return htmlHelper.DropDownList(name, selectList, htmlAttributesDictionary);
    }
    
    private static IDictionary<string, object?> AnonymousObjectToHtmlAttributes(object? htmlAttributes)
    {
        var dictionary = new Dictionary<string, object?>();
        if (htmlAttributes == null)
        {
            return dictionary;
        }
        
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(htmlAttributes))
        {
            dictionary.Add(property.Name.Replace('_', '-'), property.GetValue(htmlAttributes));
        }
        
        return dictionary;
    }
}