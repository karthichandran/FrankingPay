using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Winnovative;
using System.Xml;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace FrankingPay.Core.Selenium
{
   public class ArticlePaymentProcess:Base
    {
        static BankAccountDetailsDto _bankLogin;
        private static void GeneratePDF(IWebDriver webdriver, string downloadPath, string fileName)
        {
            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "qiQ3JTc2JWdqZHdhMWRpaStnbH8lPDwlNjQrNDcrPDw8PA==";

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            htmlToPdfConverter.ConversionDelay = 2;

            // Set a property to enable the conversion of URI links from HTML to PDF
            // If you leave the property not set conversion of URI links from HTML to PDF is enabled by default
            htmlToPdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;


            //pathString = System.IO.Path.Combine(@"C:\frankinginvoices\", lotno);

            //// Verify the path that you have constructed.
            //Console.WriteLine("Path to my file: {0}\n", pathString);

            //// Check that the file doesn't already exist. If it doesn't exist, create
            //// the file and write integers 0 - 99 to it.
            //// DANGER: System.IO.File.Create will overwrite the file if it already exists.
            //// This could happen even with random file names, although it is unlikely.
            //if (!System.IO.File.Exists(pathString))
            //{
            //    using (System.IO.FileStream fs = System.IO.File.Create(pathString))
            //    {

            //    }
            //}

            // Convert HTML to PDF using the settings above
            
          
           // string outPdfFile = @"C:\frankinginvoices\" + challanno + ".pdf";
            string outPdfFile = downloadPath +@"\"+ fileName + ".pdf";
            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(webdriver.PageSource);

                var htmlBody = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""tcontent1""]");
                HtmlNode oldChild = htmlBody.ChildNodes[1];
                var date = DateTime.Today.ToShortDateString();

                HtmlNode dateNode = HtmlNode.CreateNode(@String.Format("<p style = \" font-family: 'Ubuntu', sans-serif; font-size: 12px; padding-top: 16px; padding-bottom: 2px; \"> {0}  </p>", date));
                htmlBody.InsertBefore(dateNode, oldChild);

                HtmlNode newChild = HtmlNode.CreateNode(@"<img src=""https://k2.karnataka.gov.in/wps/PA_TestResponse/img/layer_10.png"" id=""logo"" style=""width: 100px; height: 100px; padding-left: 315px; padding-bottom: 3px; align: right; display: block; "">");

                htmlBody.ReplaceChild(newChild, oldChild);

                HtmlNodeCollection children = new HtmlNodeCollection(htmlBody);

                HtmlNode h2Node = HtmlNode.CreateNode(@"<footer style=""float:right; padding-top: 16px;""><p style=""float:right; "">1/1</p></footer>");
                htmlBody.AppendChildren(children);

                children.Add(h2Node);

                htmlBody.AppendChildren(children);

                var htmlNodes = htmlDoc.GetElementbyId("printableRctDtl").InnerHtml;
                htmlToPdfConverter.HtmlViewerZoom = 110;
                htmlToPdfConverter.HtmlViewerWidth = 800;
                htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 40;
                htmlToPdfConverter.PdfDocumentOptions.RightMargin = 40;

                var pdfobj = htmlToPdfConverter.ConvertHtml(htmlNodes, "");

                // Write the memory buffer in a PDF file
                System.IO.File.WriteAllBytes(outPdfFile, pdfobj);
            }
            catch (Exception ex)
            {
                // The HTML to PDF conversion failed
                return;
            }


            // Open the created PDF document in default PDF viewer
            try
            {
                System.Diagnostics.Process.Start(outPdfFile);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string ProcessArticle(ArticleFeedModel model,bool isArticle5E,string downloadPath,string fileName) {

            _bankLogin = new BankAccountDetailsDto
            {
                UserName = "579091011.RGANESH",
                UserPassword = "Rajalara@123"
            };
            //_bankLogin = new BankAccountDetailsDto
            //{
            //    UserName = "579091011.VIJAYALA",
            //    UserPassword = "Sriram@123"
            //};
            
            try
            {
                var webDriver = GetChromeDriver();
                FeedStaticDate(model, isArticle5E);
                var challan = FillArticle5E(webDriver, model);
                ProcessToBank(webDriver);
                GeneratePDF(webDriver, downloadPath, fileName);
                return challan;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        private static string FillArticle5E(IWebDriver webDriver, ArticleFeedModel model) {

            try
            {
                webDriver.Navigate().GoToUrl("https://k2.karnataka.gov.in/wps/portal/Khajane-II/Scope/Remittance/ChallanGeneration/");
                WaitFor(webDriver, 3);

                var curHandle = webDriver.CurrentWindowHandle;
                var tot = webDriver.WindowHandles.Count;

                var tota= webDriver.WindowHandles.Count;


                var firstNameElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:firstName"));
                firstNameElm.SendKeys(model.FirstName);

                var middleNameElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:firstName"));
                middleNameElm.SendKeys(model.MiddleName);

                var lastNameElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:lastName"));
                lastNameElm.SendKeys(model.LastName);

                var addressElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:address"));
                addressElm.SendKeys(model.Address);

                var categoryElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:category"));
                var categoryDDl = new SelectElement(categoryElm);
                categoryDDl.SelectByText(model.Category);
                WaitFor(webDriver, 2);

                WaitForSelectionReady(webDriver, "viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:district");
                var districtElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:district"));
                var districtDDl = new SelectElement(districtElm);
                districtDDl.SelectByText(model.District.ToUpper());
                WaitFor(webDriver, 2);

                var departmentElm = webDriver.FindElement(By.Id("tags"));
                departmentElm.SendKeys(model.Department);
                WaitFor(webDriver, 2);

                var departmentPopupElm = webDriver.FindElement(By.Id("ui-id-3"));
                departmentPopupElm.Click();
                // WaitFor(webDriver, 3);

                WaitForSelectionReady(webDriver, "viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:ddoOfc");
                var ddo_officeElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:ddoOfc"));
                var ddo_officeDDl = new SelectElement(ddo_officeElm);
                ddo_officeDDl.SelectByValue(model.DDO_Office);
                //  WaitFor(webDriver, 3);

                WaitForSelectionReady(webDriver, "viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:purpose");
                var purposeElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:purpose"));
                var dpurposeDDl = new SelectElement(purposeElm);
                dpurposeDDl.SelectByValue(model.Purpose);
                //WaitFor(webDriver, 3);

                WaitForSelectionReady(webDriver, "viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:subprps");
                var subPurposeElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:subprps"));
                var subPurposeDDl = new SelectElement(subPurposeElm);
                subPurposeDDl.SelectByValue(model.SubPurpose);
                WaitFor(webDriver, 2);

                var amountElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:amount2"));
                amountElm.Clear();
                amountElm.SendKeys(model.Amount);
                WaitFor(webDriver, 1);

                //Click Add button
                var addElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:addtemp"));
                addElm.Click();
                WaitFor(webDriver, 4);

                var mopElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:modeOfPayment"));
                var mopDDl = new SelectElement(mopElm);
                mopDDl.SelectByText(model.ModeOfPayment);
                WaitFor(webDriver, 5);

                var ePayElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:gtwyorbankTyp"));
                var ePayDDl = new SelectElement(ePayElm);
                ePayDDl.SelectByText(model.TypeOf_e_payment);
                WaitFor(webDriver, 2);

                WaitForElementReady(webDriver, "viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:gtwyorbank");
                var bankElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:gtwyorbank"));
                var bankDDl = new SelectElement(bankElm);
                bankDDl.SelectByText(model.Bank);
                WaitFor(webDriver, 2);

                //Click submit button
                var submitElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:submit_id"));
                submitElm.Click();
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
                var alert = wait.Until(ExpectedConditions.AlertIsPresent());
                if (alert != null)
                {
                    alert.Accept();
                    WaitFor(webDriver, 1);
                    // Note : first name cleared automatically so fill again
                    firstNameElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:firstName"));
                    firstNameElm.SendKeys(model.FirstName);
                    WaitFor(webDriver, 1);

                    submitElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:submit_id"));
                    submitElm.Click();
                    WaitFor(webDriver, 2);
                }

                var captcha = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:printForm:captchaText")).Text;
                var captchaInputElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:printForm:captchaTextcode"));
                captchaInputElm.SendKeys(captcha);
                WaitFor(webDriver, 4);

                //Click Confirm button
                var confirmElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:printForm:j_id_33"));
                confirmElm.Click();
                WaitFor(webDriver, 2);

                //
                var challanNoTxt = webDriver.FindElement(By.XPath("//*[@id='viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanConfirm']/fieldset/table/tbody/tr[2]/td/p/b")).Text;
                var challanno = challanNoTxt.Replace("\r\n", " ").Split(':')[1].Trim().Split(' ')[0];
                //Click Confirm button
                var okElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanConfirm:j_id_12"));
                okElm.Click();
                WaitFor(webDriver, 1);

                return challanno;
            }
            catch (Exception ex) {
                throw new Exception("Challan filling is Failed.");
            }
        }

        private static void ProcessToBank(IWebDriver webDriver)
        {
            try
            {

                WaitFor(webDriver, 3);
                var payBtn = webDriver.FindElement(By.Id("CIB_11X_PROCEED"));
                payBtn.Click();
                WaitForReady(webDriver);
                WaitFor(webDriver, 3);
                var userIdTxt = webDriver.FindElement(By.Id("login-step1-userid"));
                userIdTxt.SendKeys(_bankLogin.UserName);
                var pwdTxt = webDriver.FindElement(By.Id("AuthenticationFG.ACCESS_CODE"));
                pwdTxt.SendKeys(_bankLogin.UserPassword);
                var proceedBtn = webDriver.FindElement(By.Id("VALIDATE_CREDENTIALS1"));
                proceedBtn.Click();
                WaitForReady(webDriver);
                var continueBtn = webDriver.FindElements(By.Id("CONTINUE_PREVIEW"));
                if (continueBtn.Count > 0)
                {
                    continueBtn[0].Click();
                    WaitForReady(webDriver);
                }

                var submitBtn = webDriver.FindElements(By.Id("CONTINUE_SUMMARY"));
                if (submitBtn.Count > 0)
                {
                    submitBtn[0].Click();
                    WaitForReady(webDriver);
                }

                var returnToPortalBtn = webDriver.FindElement(By.XPath("//input[@onClick='reDirectToPortalURL();']"));
                returnToPortalBtn.Click();
                WaitFor(webDriver, 2);

               // var prinBtn = webDriver.FindElement(By.Id("printChallan"));
                //  prinBtn.Click();

                var jsExecuter = (IJavaScriptExecutor)webDriver;
                jsExecuter.ExecuteScript(" var elm=document.getElementById('printChallan'); setTimeout(function(){elm.click();},100);");

                WaitFor(webDriver, 1);
               // webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
               

                //var printBtn = webDriver.FindElement(By.XPath("//cr-button[@class='action-button']"));
                //printBtn.Click();

                
            }
            catch (Exception ex) {
                throw new Exception("bank process Failed.");
            }
        }

        private static void FeedStaticDate(ArticleFeedModel model,bool isArticle5e) {
            model.Address = "Prestige Falcon Towers 19 Brunton Road Bangalore 560025";
            model.Category = "Government";
            model.District = "Bengaluru Urban";
            model.Department = "Department of Stamps and Registration";
            model.DDO_Office = "100011441";  //actual  "Sub REGISTRAR OFFICE, SHIVAJINAGAR, Bangalore";
            model.Purpose = "48";// "Duty (Stamp Duty)";
            if (isArticle5e)
                model.SubPurpose = "12317";// "Agreement for sale of immoveable property [Article No. 5 (e)]";
            else
                model.SubPurpose = "12352";// "Counterpart or Duplicate [Article No. 22]";
            model.ModeOfPayment = "Netbanking";
            model.TypeOf_e_payment = "Direct Integration with Banks";
            model.Bank = "ICICI Bank";
        }
               
    }

    public class BankAccountDetailsDto
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}
