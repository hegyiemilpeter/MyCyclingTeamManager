using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManager.Manual.Infrastructure.TagHelpers
{
    [HtmlTargetElement("racedate")]
    public class RaceDateTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory urlHelperFactory;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public RaceDateTagHelper(IUrlHelperFactory urlHelper)
        {
            urlHelperFactory = urlHelper;
        }

        public int NumberOfMonths { get; set; } = 6;

        public int StartYear { get; set; }

        public int StartMonth { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContextData);

            DateTime baseDate = new DateTime(StartYear == 0 ? DateTime.Now.Year : StartYear, StartMonth == 0 ? DateTime.Now.Month : StartMonth, 1);
            TagBuilder backArrow = CreateButton(urlHelper, baseDate.AddMonths(-1), "btn btn-outline-primary btn-sm m-1", "<<");
            output.Content.AppendHtml(backArrow);

            for (int i = NumberOfMonths; i > 0; i--)
            {
                TagBuilder previousMonthButton = CreateButton(urlHelper, baseDate.AddMonths(-1 * i), "btn btn-outline-primary btn-sm m-1");
                output.Content.AppendHtml(previousMonthButton);
            }

            TagBuilder currentMonthButton = CreateButton(urlHelper, baseDate, "btn btn-primary btn-sm m-1");
            output.Content.AppendHtml(currentMonthButton);

            for (int i = 1; i <= NumberOfMonths; i++)
            {
                TagBuilder nextMonthButton = CreateButton(urlHelper, baseDate.AddMonths(i), "btn btn-outline-primary btn-sm m-1");
                output.Content.AppendHtml(nextMonthButton);
            }

            TagBuilder forwardArrow = CreateButton(urlHelper, baseDate.AddMonths(1), "btn btn-outline-primary btn-sm m-1", ">>");
            output.Content.AppendHtml(forwardArrow);
        }

        private static TagBuilder CreateButton(IUrlHelper urlHelper, DateTime dateOfActualMonth, string cssClass, string label = null)
        {
            TagBuilder button = new TagBuilder("a");
            button.AddCssClass(cssClass);

            string href = urlHelper.ActionLink("Index", "Races", new { year = dateOfActualMonth.Year, month = dateOfActualMonth.Month });
            button.Attributes.Add("href", href);

            string year = "'" + dateOfActualMonth.Year.ToString().Substring(2, 2);
            button.InnerHtml.Append(label ?? $"{year} {dateOfActualMonth.ToString("MMMM")}");

            return button;
        }
    }
}
