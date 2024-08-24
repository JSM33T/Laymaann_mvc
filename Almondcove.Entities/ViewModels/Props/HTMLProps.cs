using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laymaann.Entities.ViewModels.Props
{
    public class HTMLProps
    {
        public string DataBsTheme { get; set; }
        public string HeaderClass { get; set; }
        public string BaseHref { get; set; }
        public string NavClass { get; set; }
        public string BodyClass { get; set; }
        public string IsLoaderActive { get; set; }
        public bool HasCode { get; set; }

        public HTMLProps()
        {
            DataBsTheme = "";
            HeaderClass = "navbar navbar-expand-lg fixed-top bg-light navbar-frost";
            BaseHref = "";
            NavClass = "";
            BodyClass = "";
            IsLoaderActive = "active";
            HasCode = false;
        }
    }
}
