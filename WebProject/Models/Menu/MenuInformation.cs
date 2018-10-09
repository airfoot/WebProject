using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models.Menu
{
    public class MenuInformation
    {
        public MenuInformation(string menuName, string displayName, string controller, string action, string url, string cssClass)
        {
            
            this.MenuName = menuName;
            this.DisplayName = displayName;
            this.Controller = controller;
            this.Action = action;
            this.Url = url;
            this.CssClass = cssClass;
            SubMenu = new List<MenuInformation>();
        }
        public MenuInformation(string menuName,string displayName, string cssClass)
        {
            this.MenuName = menuName;
            this.DisplayName = displayName;
            this.CssClass = cssClass;
            SubMenu = new List<MenuInformation>();
        }
        public MenuInformation(string menuName)
        {
            this.MenuName = menuName;
            SubMenu = new List<MenuInformation>();
        }
        public MenuInformation()
        {
            SubMenu = new List<MenuInformation>();
        }
        public string MenuName { get; set; }
        public string DisplayName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Url { get; set; }
        public string CssClass { get; set; }
        public List<MenuInformation> SubMenu { get; set; }
    }

    
}