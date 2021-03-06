using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Http.TestLibrary;
using System.Web;

namespace HttpSimulatorTests
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void CurrentIsNotNull()
        {
            using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
            {
                Assert.IsNotNull(HttpContext.Current);
            }
        }

        /// <summary>
        /// Determines whether this instance [can get set session].
        /// </summary>
        [Test]
        public void CanGetSetSession()
        {
            using (var simulator = new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
            {
                simulator.Context.Session["Test"] = "Success";
                Assert.AreEqual("Success", simulator.Context.Session["Test"]);
            }
        }

        [Test]
        public void CanSimulateFormGetWithQueryString()
        {
            using (var simulator = new HttpSimulator())
            {
                var form = new NameValueCollection();
                form.Add("Test1", "Value1");
                form.Add("Test2", "Value2");
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx?Test1=Value1&Test2=Value2"));
                Assert.AreEqual(form, simulator.Context.Request.QueryString);
            }
        }

        /// <summary>
        /// Determines whether this instance [can simulate form post].
        /// </summary>
        [Test]
        public void CanSimulateFormPost()
        {
            using (var simulator = new HttpSimulator())
            {
                var form = new NameValueCollection();
                form.Add("Test1", "Value1");
                form.Add("Test2", "Value2");
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx"), form);

                Assert.AreEqual("Value1", simulator.Context.Request.Form["Test1"]);
                Assert.AreEqual("Value2", simulator.Context.Request.Form["Test2"]);
                Assert.AreEqual(new Uri("http://localhost/Test.aspx"), simulator.Context.Request.Url);
            }

            using (var simulator = new HttpSimulator())
            {
                simulator.SetFormVariable("Test1", "Value1")
                  .SetFormVariable("Test2", "Value2")
                  .SimulateRequest(new Uri("http://localhost/Test.aspx"));

                Assert.AreEqual("Value1", simulator.Context.Request.Form["Test1"]);
                Assert.AreEqual("Value2", simulator.Context.Request.Form["Test2"]);
                Assert.AreEqual(new Uri("http://localhost/Test.aspx"), simulator.Context.Request.Url);
            }
        }

        [Test]
        public void CanSimulateSession()
        {
            using (var simulator = new HttpSimulator())
            {
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx"));

                Assert.IsNotNull(simulator.Context.Session.SessionID);
                simulator.Context.Session.Add("item", "value");
                Assert.AreEqual(1, simulator.Context.Session.Count);
                Assert.AreEqual("value", simulator.Context.Session["item"]);
            }
        }

        [Test]
        public void CanSimulateUserAgent() {
            using (var simulator = new HttpSimulator()) {
                var headers = new NameValueCollection();
                headers.Add("User-Agent", "Agent1");
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx"), HttpVerb.POST, headers);
                Assert.AreEqual("Agent1", simulator.Context.Request.UserAgent);
            }
        }

        [Test]
        [Platform(Exclude = "Mono")]
        public void CanGetSetCookies() {
            using (var simulator = new HttpSimulator())
            {
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx"), HttpVerb.POST);
                simulator.Context.Response.Cookies.Add(new HttpCookie("a", "b"));
                Assert.AreEqual("b", HttpContext.Current.Response.Cookies["a"].Value);
            }
        }

        [Test]
        [Platform(Exclude = "Mono")]
        public void CanSimulateMapPath()
        {
            using (var simulator = new HttpSimulator())
            {
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx"));

                Assert.AreEqual("c:\\InetPub\\wwwRoot\\test\\test.js", simulator.Context.Server.MapPath("~/test/test.js"));
                Assert.AreEqual("c:\\InetPub\\wwwRoot\\test.js", simulator.Context.Server.MapPath("test.js"));
            }
        }

        /// <summary>
        /// Determines whether this instance [can simulate form post].
        /// </summary>
        [Test]
        [Platform(Exclude = "Mono")]
        public void CanSimulateFormPostOnHttpContext()
        {
            using (var simulator = new HttpSimulator())
            {
                var form = new NameValueCollection();
                form.Add("Test1", "Value1");
                form.Add("Test2", "Value2");
                simulator.SimulateRequest(new Uri("http://localhost/Test.aspx"), form);

                Assert.AreEqual("Value1", HttpContext.Current.Request.Form["Test1"]);
                Assert.AreEqual("Value2", HttpContext.Current.Request.Form["Test2"]);
                Assert.AreEqual(new Uri("http://localhost/Test.aspx"), HttpContext.Current.Request.Url);
            }

            using (var simulator = new HttpSimulator())
            {
                simulator.SetFormVariable("Test1", "Value1")
                  .SetFormVariable("Test2", "Value2")
                  .SimulateRequest(new Uri("http://localhost/Test.aspx"));

                Assert.AreEqual("Value1", HttpContext.Current.Request.Form["Test1"]);
                Assert.AreEqual("Value2", HttpContext.Current.Request.Form["Test2"]);
                Assert.AreEqual(new Uri("http://localhost/Test.aspx"), HttpContext.Current.Request.Url);
            }
        }
    }
}

