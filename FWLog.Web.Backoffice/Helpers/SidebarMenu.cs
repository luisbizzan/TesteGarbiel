using FWLog.AspNet.Identity.Extensions;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace FWLog.Web.Backoffice.Helpers
{
    public class SidebarMenu
    {
        private IPrincipal _user;

        public List<IMenuItem> Items { get; set; }

        public SidebarMenu(IPrincipal user)
        {
            _user = user;
            Items = new List<IMenuItem>();
        }

        public IHtmlString Render()
        {
            var builder = new StringBuilder();

            foreach (IMenuItem item in Items)
            {
                item.BuildHtmlIfPermitted(_user, builder);
            }

            return new HtmlString(builder.ToString());
        }

        public void SetItems(params IMenuItem[] items)
        {
            Items.AddRange(items);
        }
    }

    public interface IMenuItem
    {
        void BuildHtmlIfPermitted(IPrincipal user, StringBuilder builder);
        bool HasPermissionForBuilding(IPrincipal user);
    }

    public class MenuItem : IMenuItem
    {
        private string _text;

        public string RequirePermission { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }

        public string Text
        {
            get { return _text; }
            set { _text = WebUtility.HtmlEncode(value); }
        }

        public void BuildHtmlIfPermitted(IPrincipal user, StringBuilder builder)
        {
            if (!HasPermissionForBuilding(user))
            {
                return;

            }

            if (string.IsNullOrEmpty(Icon))
            {
                builder.Append(string.Format(@"<li><a href=""{0}"">{1}</a></li>", Url, Text));
            }
            else
            {
                builder.Append(string.Format(@"<li><a href=""{0}""><i class=""{1}""></i> {2}</a></li>", Url, Icon, Text));
            }
        }

        public bool HasPermissionForBuilding(IPrincipal user)
        {
            if (RequirePermission == null)
            {
                return true;
            }

            return user.HasPermission(RequirePermission);
        }
    }

    public class MenuGroup : IMenuItem
    {
        private string _text;

        public string Icon { get; set; }
        public List<IMenuItem> Submenus { get; set; }

        public string Text
        {
            get { return _text; }
            set { _text = WebUtility.HtmlEncode(value); }
        }

        public MenuGroup(string text, string icon)
        {
            Icon = icon;
            Text = text;
            Submenus = new List<IMenuItem>();
        }

        public MenuGroup SetItems(params IMenuItem[] items)
        {
            Submenus.AddRange(items);
            return this;
        }

        public void BuildHtmlIfPermitted(IPrincipal user, StringBuilder builder)
        {
            if (!HasPermissionForBuilding(user))
            {
                return;
            }

            builder
                .Append(string.Format(@"<li><a><i class=""{0}""></i> {1} <span class=""fa fa-chevron-down""></span></a>", Icon, Text))
                .Append(@"<ul class=""nav child_menu"">");

            foreach (IMenuItem subItem in Submenus)
            {
                subItem.BuildHtmlIfPermitted(user, builder);
            }

            builder.Append("</ul></li>");
        }

        public bool HasPermissionForBuilding(IPrincipal user)
        {
            foreach (IMenuItem subItem in Submenus)
            {
                if (subItem.HasPermissionForBuilding(user))
                {
                    return true;
                }
            }

            return false;
        }
    }
}