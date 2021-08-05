using Microsoft.AspNetCore.Mvc;

namespace _4drafts.Services
{
    public interface IHtmlHelper
    {
        public string RenderRazorViewToString(Controller controller, string viewName, object model = null);
    }
}
