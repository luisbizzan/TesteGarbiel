using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace System.Web.Mvc
{
    public static class DartHtml
    {
        public static MvcHtmlString MultiSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            bool liveSearch = false, bool multipleSelect = false, bool showTick = false, bool selectAll = false, int? maxSelection = null)
        {
            return htmlHelper.MultiSelectFor(expression, selectList, null, null, liveSearch, multipleSelect, showTick, selectAll, maxSelection);
        }

        public static MvcHtmlString MultiSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes,
            bool liveSearch = false, bool multipleSelect = false, bool showTick = false, bool selectAll = false, int? maxSelection = null)
        {
            return htmlHelper.MultiSelectFor(expression, selectList, null, htmlAttributes, liveSearch, multipleSelect, showTick, selectAll, maxSelection);
        }

        public static MvcHtmlString MultiSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel,
            bool liveSearch = false, bool multipleSelect = false, bool showTick = false, bool selectAll = false, int? maxSelection = null)
        {
            return htmlHelper.MultiSelectFor(expression, selectList, optionLabel, null, liveSearch, multipleSelect, showTick, selectAll, maxSelection);
        }

        public static MvcHtmlString MultiSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes,
            bool liveSearch = false, bool multipleSelect = false, bool showTick = false, bool selectAll = false, int? maxSelection = null)
        {
            IDictionary<string, object> attributes = PrepareHtmlAttributes(htmlAttributes, liveSearch, multipleSelect, showTick, selectAll, maxSelection);

            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, attributes);
        }

        public static MvcHtmlString MultiSelect(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList,
            bool liveSearch = false, bool multipleSelect = false, bool showTick = false, bool selectAll = false, int? maxSelection = null)
        {
            return htmlHelper.MultiSelect(name, selectList, optionLabel: null, htmlAttributes: null,
                liveSearch: liveSearch, multipleSelect: multipleSelect, showTick: showTick, selectAll: selectAll, maxSelection: maxSelection);
        }

        public static MvcHtmlString MultiSelect(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes,
            bool liveSearch = false, bool multipleSelect = false, bool showTick = false, bool selectAll = false, int? maxSelection = null)
        {
            return htmlHelper.MultiSelect(name, selectList, optionLabel: null, htmlAttributes: htmlAttributes,
                liveSearch: liveSearch, multipleSelect: multipleSelect, showTick: showTick, selectAll: selectAll, maxSelection: maxSelection);
        }

        public static MvcHtmlString MultiSelect(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel,
            bool liveSearch = false, bool multipleSelect = false, bool showTick = false, bool selectAll = false, int? maxSelection = null)
        {
            return htmlHelper.MultiSelect(name, selectList, optionLabel: optionLabel, htmlAttributes: null,
                liveSearch: liveSearch, multipleSelect: multipleSelect, showTick: showTick, selectAll: selectAll, maxSelection: maxSelection);
        }

        public static MvcHtmlString MultiSelect(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes,
            bool liveSearch = false, bool multipleSelect = false, bool showTick = false, bool selectAll = false, int? maxSelection = null)
        {
            IDictionary<string, object> attributes = PrepareHtmlAttributes(htmlAttributes, liveSearch, multipleSelect, showTick, selectAll, maxSelection);

            return htmlHelper.DropDownList(name, selectList, optionLabel, attributes);
        }

        private static IDictionary<string, object> PrepareHtmlAttributes(IDictionary<string, object> htmlAttributes, bool liveSearch, bool multipleSelect, bool showTick, bool selectAll, int? maxSelection = null)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }

            if (htmlAttributes.ContainsKey("class"))
            {
                htmlAttributes["class"] = "selectpicker " + htmlAttributes["class"];
            }
            else
            {
                htmlAttributes.Add("class", "selectpicker");
            }

            SetAttribute(htmlAttributes, "data-selected-text-format", "count > 2");
            SetAttribute(htmlAttributes, "data-width", "100%", !htmlAttributes.ContainsKey("data-width"));
            SetAttribute(htmlAttributes, "data-live-search", "true", liveSearch);
            SetAttribute(htmlAttributes, "multiple", null, multipleSelect);
            SetAttribute(htmlAttributes, "show-tick", null, showTick);
            SetAttribute(htmlAttributes, "data-actions-box", "true", multipleSelect && selectAll);
            SetAttribute(htmlAttributes, "data-max-options",
                (maxSelection != null && maxSelection.HasValue) ? maxSelection.Value.ToString() : "",
                multipleSelect && (maxSelection != null && maxSelection.HasValue));

            return htmlAttributes;
        }

        private static void SetAttribute(IDictionary<string, object> htmlAttributes, string attribute, string value, bool condition = true)
        {
            if (condition)
            {
                if (htmlAttributes.ContainsKey(attribute))
                {
                    htmlAttributes[attribute] = value;
                }
                else
                {
                    htmlAttributes.Add(attribute, value);
                }
            }
        }

    }

}