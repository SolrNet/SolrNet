using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace SampleSolrApp {
    public class _Default : Page {
        public void Page_Load(object sender, EventArgs e) {
            HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(HttpContext.Current);
        }
    }
}