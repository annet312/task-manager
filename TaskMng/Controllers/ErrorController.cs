namespace TaskMng.Controllers
{
    using System;
    using System.Web.Mvc;
	using Moveax.Mvc.ErrorHandler;

    public class ErrorController : ErrorHandlerControllerBase
    {
        /// <summary>Default action.</summary>
        /// <param name="errorDescription">The error description.</param>
        public override ActionResult Default(ErrorDescription errorDescription)
        {
            return this.View("Default");
        }

        /// <summary>HTTP 404 Error: Not found.</summary>
        /// <param name="errorDescription">The error description.</param>
        public ActionResult Http404(ErrorDescription errorDescription)
        {
            return this.View();
        }

		/// <summary>Called before the action method is invoked. Use it for error logging etc</summary>
        /// <param name="errorDescription">The error description.</param>
        protected override void HandleError(ErrorDescription errorDescription)
        {
            // TODO: Add a logging code here (just a reminder).
        }
    }
}
