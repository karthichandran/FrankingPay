using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace FrankingPay.Core.Selenium
{
    public class Base
    {
        protected static void WaitForReady(IWebDriver webDriver)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(120);
            WebDriverWait wait = new WebDriverWait(webDriver, timeSpan);
            wait.Until(driver => {
                bool isAjaxFinished = (bool)((IJavaScriptExecutor)driver).
                    ExecuteScript("return jQuery.active == 0");
                try
                {
                    var loader = driver.FindElement(By.ClassName("loader-mask")).GetAttribute("style");
                    Console.WriteLine(loader);
                    return loader.Split(':')[1] == " none;";
                }
                catch
                {
                    return isAjaxFinished;
                }
            });
        }

        protected static void WaitForSelectionReady(IWebDriver webDriver,string elemID)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(60);
            WebDriverWait wait = new WebDriverWait(webDriver, timeSpan);
            wait.Until(driver => {
               
                try
                {
                    var selectElm = webDriver.FindElement(By.Id(elemID));
                    var ddl = new SelectElement(selectElm);
                    var optCnt = ddl.Options.Count;
                    return optCnt > 1;
                }
                catch
                {
                    return false;
                }
            });
        }

        protected static void WaitForElementReady(IWebDriver webDriver, string elemID)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(60);
            WebDriverWait wait = new WebDriverWait(webDriver, timeSpan);
            wait.Until(driver => {

                try
                {
                    var selectElm = webDriver.FindElement(By.Id(elemID));
                    return selectElm != null;
                }
                catch
                {
                    return false;
                }
            });
        }

        protected static void WaitFor(IWebDriver webDriver, int inSeconds = 0)
        {
            Thread.Sleep(inSeconds * 1000);
        }

        protected static IWebDriver GetChromeDriver()
        {
            try
            {
               
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-infobars");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--start-maximized");

               // options.AddArgument("--disable-print-preview");


                //options.AddUserProfilePreference("pdfjs.disabled", true);
                //options.AddUserProfilePreference("download.prompt_for_download", false);
                //options.AddUserProfilePreference("download.directory_upgrade", true);
                //options.AddUserProfilePreference("plugins.plugins_disabled", "Chrome PDF Viewer");
                //options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);

                ChromeDriver driver = new ChromeDriver(options);
                return driver;                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
