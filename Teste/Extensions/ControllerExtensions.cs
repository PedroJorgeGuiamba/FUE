using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Teste.Extensions
{
    public static class ControllerExtensions
    {      
        public static async Task<string> RenderViewToStringAsync(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = controller.HttpContext.RequestServices
                    .GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.FindView(controller.ControllerContext, viewName, false).View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewContext.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
