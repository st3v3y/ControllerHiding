using System;

namespace ControllerHiding.Models
{
    public class HiddenGetData
    {
        public string Identifier { get; set; }
        public string RedirectAction { get; set; }
        public object SubModel { get; set; }
    }
}