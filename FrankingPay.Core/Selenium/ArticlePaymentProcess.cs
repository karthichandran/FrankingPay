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
using System.Collections.Generic;
using Anticaptcha_example.Api;

namespace FrankingPay.Core.Selenium
{
   public class ArticlePaymentProcess:Base
    {
        static BankAccountDetailsDto _bankLogin;
        
        private static string GeneratePDF(IWebDriver webdriver, string downloadPath, string fileName)
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
            string outPdfFile = downloadPath + @"\" + fileName + ".pdf";
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

                string transactionNo;

                using (UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(pdfobj))
                {
                    var page = document.GetPage(1);
                    string text = string.Join(" ", page.GetWords());
                    var pattern = string.Format(@"\b\w*" + "Transaction No." + @"\w*\s+\w+\b");
                    string match = Regex.Match(text, @pattern).Groups[0].Value;
                    string[] words = match.Split(' ');
                    transactionNo = words[words.Length - 1];
                }
                return transactionNo;
            }
            catch (Exception ex)
            {
                // The HTML to PDF conversion failed
                return "";
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

        //private static string GeneratePDF(IWebDriver webdriver, string downloadPath, string fileName)
        //{
        //    // Create a HTML to PDF converter object with default settings
        //    HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

        //    // Set license key received after purchase to use the converter in licensed mode
        //    // Leave it not set to use the converter in demo mode
        //    htmlToPdfConverter.LicenseKey = "qiQ3JTc2JWdqZHdhMWRpaStnbH8lPDwlNjQrNDcrPDw8PA==";

        //    // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
        //    // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
        //    htmlToPdfConverter.ConversionDelay = 2;

        //    // Set a property to enable the conversion of URI links from HTML to PDF
        //    // If you leave the property not set conversion of URI links from HTML to PDF is enabled by default
        //    htmlToPdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;


        //    //pathString = System.IO.Path.Combine(@"C:\frankinginvoices\", lotno);

        //    //// Verify the path that you have constructed.
        //    //Console.WriteLine("Path to my file: {0}\n", pathString);

        //    //// Check that the file doesn't already exist. If it doesn't exist, create
        //    //// the file and write integers 0 - 99 to it.
        //    //// DANGER: System.IO.File.Create will overwrite the file if it already exists.
        //    //// This could happen even with random file names, although it is unlikely.
        //    //if (!System.IO.File.Exists(pathString))
        //    //{
        //    //    using (System.IO.FileStream fs = System.IO.File.Create(pathString))
        //    //    {

        //    //    }
        //    //}

        //    // Convert HTML to PDF using the settings above


        //    // string outPdfFile = @"C:\frankinginvoices\" + challanno + ".pdf";
        //    string outPdfFile = downloadPath + @"\" + fileName + ".pdf";
        //    try
        //    {
        //        HtmlDocument htmlDoc = new HtmlDocument();
        //        htmlDoc.LoadHtml(webdriver.PageSource);

        //        var htmlBody = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""tcontent1""]");
        //        HtmlNode oldChild = htmlBody.ChildNodes[1];
        //        var date = DateTime.Today.ToShortDateString();

        //        HtmlNode dateNode = HtmlNode.CreateNode(@String.Format("<p style = \" font-family: 'Ubuntu', sans-serif; font-size: 12px; padding-top: 16px; padding-bottom: 2px; \"> {0}  </p>", date));
        //        htmlBody.InsertBefore(dateNode, oldChild);

        //        HtmlNode newChild = HtmlNode.CreateNode(@"<img src=""https://k2.karnataka.gov.in/wps/PA_TestResponse/img/layer_10.png"" id=""logo"" style=""width: 100px; height: 100px; padding-left: 315px; padding-bottom: 3px; align: right; display: block; "">");

        //        htmlBody.ReplaceChild(newChild, oldChild);

        //        HtmlNodeCollection children = new HtmlNodeCollection(htmlBody);

        //        HtmlNode h2Node = HtmlNode.CreateNode(@"<footer style=""float:right; padding-top: 16px;""><p style=""float:right; "">1/1</p></footer>");
        //        htmlBody.AppendChildren(children);

        //        children.Add(h2Node);

        //        htmlBody.AppendChildren(children);

        //        // var htmlNodes = htmlDoc.GetElementbyId("printableRctDtl").InnerHtml;
        //        var htmlNodes = htmlDoc.GetElementbyId("viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:printForm:printChallan").InnerHtml;
        //        htmlToPdfConverter.HtmlViewerZoom = 110;
        //        htmlToPdfConverter.HtmlViewerWidth = 800;
        //        htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 40;
        //        htmlToPdfConverter.PdfDocumentOptions.RightMargin = 40;

        //        var pdfobj = htmlToPdfConverter.ConvertHtml(htmlNodes, "");


        //        // Write the memory buffer in a PDF file
        //        System.IO.File.WriteAllBytes(outPdfFile, pdfobj);

        //        string transactionNo;

        //        using (UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(pdfobj))
        //        {
        //            var page = document.GetPage(1);
        //            string text = string.Join(" ", page.GetWords());
        //            var pattern = string.Format(@"\b\w*" + "Transaction No." + @"\w*\s+\w+\b");
        //            string match = Regex.Match(text, @pattern).Groups[0].Value;
        //            string[] words = match.Split(' ');
        //            transactionNo = words[words.Length - 1];
        //        }
        //        return transactionNo;
        //    }
        //    catch (Exception ex)
        //    {
        //        // The HTML to PDF conversion failed
        //        return "";
        //    }


        //    // Open the created PDF document in default PDF viewer
        //    //try
        //    //{
        //    //    System.Diagnostics.Process.Start(outPdfFile);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw;
        //    //}
        //}

        //private static string GeneratePDF(IWebDriver webdriver, string downloadPath, string fileName, string copyName)
        //{
        //    // Create a HTML to PDF converter object with default settings
        //    HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

        //    // Set license key received after purchase to use the converter in licensed mode
        //    // Leave it not set to use the converter in demo mode
        //    htmlToPdfConverter.LicenseKey = "qiQ3JTc2JWdqZHdhMWRpaStnbH8lPDwlNjQrNDcrPDw8PA==";

        //    // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
        //    // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
        //    htmlToPdfConverter.ConversionDelay = 2;

        //    // Set a property to enable the conversion of URI links from HTML to PDF
        //    // If you leave the property not set conversion of URI links from HTML to PDF is enabled by default
        //    htmlToPdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;


        //    string outPdfFile = downloadPath + @"\" + fileName + ".pdf";
        //    try
        //    {
        //        HtmlDocument htmlDoc = new HtmlDocument();
        //        htmlDoc.LoadHtml(webdriver.PageSource);

        //        var htmlBody = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:printForm""]");

        //        HtmlNode oldChild = htmlBody.ChildNodes[1];
        //        var date = DateTime.Today.ToShortDateString();

        //        HtmlNode dateNode = HtmlNode.CreateNode(@String.Format("<p style = \" font-family: 'Ubuntu', sans-serif; font-size: 12px; padding-top: 2px; padding-bottom: 2px; \"> {0}  </p>", date));
        //        htmlBody.InsertBefore(dateNode, oldChild);

        //        HtmlNode newChild = HtmlNode.CreateNode(@String.Format("<p style = \" font-family: 'Ubuntu', sans-serif; font-size: 12px; padding-top: 2px; padding-bottom: 2px; \"> {0}  </p>", copyName));

        //        htmlBody.ReplaceChild(newChild, oldChild);
        //        htmlBody.SelectSingleNode(
        //                @"//*[@id=""viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:printForm:close""]").Remove();
        //        htmlBody.SelectSingleNode(
        //            @"//*[@id=""viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:printForm:printChallan""]").Remove();

        //        //HtmlNodeCollection children = new HtmlNodeCollection(htmlBody);

        //        //todo this line is going to new page, need to keep withing the same page
        //        //HtmlNode h2Node = HtmlNode.CreateNode(@"<div style=""float:right; padding-top: 2px;""><p style=""float:right; "">1/2</p></div>");
        //        //htmlBody.AppendChildren(children);

        //        //children.Add(h2Node);

        //        //htmlBody.AppendChildren(children);

        //        var htmlNodes = htmlDoc.GetElementbyId("viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:printForm").InnerHtml;
        //        htmlToPdfConverter.HtmlViewerZoom = 100;
        //        htmlToPdfConverter.HtmlViewerWidth = 800;
        //        htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 40;
        //        htmlToPdfConverter.PdfDocumentOptions.RightMargin = 40;

        //        var pdfobj = htmlToPdfConverter.ConvertHtml(htmlNodes, "");


        //        // Write the memory buffer in a PDF file
        //        System.IO.File.WriteAllBytes(outPdfFile, pdfobj);

        //        string transactionNo;

        //        using (UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(pdfobj))
        //        {
        //            var page = document.GetPage(1);
        //            string text = string.Join(" ", page.GetWords());
        //            var pattern = string.Format(@"\b\w*" + "Transaction No." + @"\w*\s+\w+\b");
        //            string match = Regex.Match(text, @pattern).Groups[0].Value;
        //            string[] words = match.Split(' ');
        //            transactionNo = words[words.Length - 1];
        //        }
        //        return transactionNo;
        //    }
        //    catch (Exception ex)
        //    {
        //        // The HTML to PDF conversion failed
        //        return "";
        //    }

        //}

        //public static string ProcessArticle(ArticleFeedModel model,bool isArticle5E,string downloadPath,string fileName) {

        //    _bankLogin = new BankAccountDetailsDto
        //    {
        //        UserName = "579091011.RGANESH",
        //        UserPassword = "Rajalara@123"
        //    };
        //    //_bankLogin = new BankAccountDetailsDto
        //    //{
        //    //    UserName = "579091011.VIJAYALA",
        //    //    UserPassword = "Sriram@123"
        //    //};
        //    var webDriver = GetChromeDriver();
        //    try
        //    {

        //        FeedStaticData(model, isArticle5E);
        //        var challan = FillArticle5E(webDriver, model);
        //        ProcessToBank(webDriver);

        //        //  var challan = "CR0721003000526885";

        //        //NavigateToPrint(webDriver, challan);
        //        //fileName = fileName+"_"+challan;
        //        ////  string transactionNo= GeneratePDF(webDriver, downloadPath, fileName);
        //        //GeneratePDF(webDriver, downloadPath, fileName + "bank_copy", "Bank's copy");
        //        //string transactionNo = GeneratePDF(webDriver, downloadPath, fileName + "remitters_copy", "Remitter's copy");
        //        //webDriver.Quit();

        //        //Dictionary<string, string> challanDet = new Dictionary<string, string>();
        //        //challanDet.Add("challan", challan);
        //        //challanDet.Add("transactionNo", transactionNo);

        //        return challan;
        //    }
        //    catch (Exception ex) {
        //        throw ex;
        //    }
        //}

        public static Dictionary<string, string> ProcessArticle(ArticleFeedModel model, bool isArticle5E, string downloadPath, string fileName,int frankingID)
        {
            _bankLogin = new BankAccountDetailsDto
            {
                UserName = "582266194.RGANESH",
                UserPassword = "Rajalara@321"
            };

            //_bankLogin = new BankAccountDetailsDto
            //{
            //    UserName = "579091011.RGANESH",
            //    UserPassword = "Rajalara@123"
            //};
            //_bankLogin = new BankAccountDetailsDto
            //{
            //    UserName = "579091011.VIJAYALA",
            //    UserPassword = "Sriram@123"
            //};
            var webDriver = GetChromeDriver();
            try
            {

                FeedStaticData(model, isArticle5E);
                string remark = (isArticle5E) ? frankingID + "_Article_5E" : frankingID + "_Article_22";
                var challan = FillArticle5E(webDriver, model);
                ProcessToBank(webDriver, remark);

                //var challan = "CR0721003000526885";
                //NavigateToPrint(webDriver, challan);
                fileName = fileName + "_" + challan;
                string transactionNo = GeneratePDF(webDriver, downloadPath, fileName);
                webDriver.Close();
                webDriver.Quit();
                

                Dictionary<string, string> challanDet = new Dictionary<string, string>();
                challanDet.Add("challan", challan);
                challanDet.Add("transactionNo", transactionNo);

                return challanDet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public static string DownloadChallan(string challanNo, string downloadPath,string fileName) {
        //    var webDriver = GetChromeDriver();
        //    try
        //    {

        //        //FeedStaticDate(model, isArticle5E);
        //        //var challan = FillArticle5E(webDriver, model);
        //        //ProcessToBank(webDriver);

        //        var challan = "CR0721003000526885";
        //        NavigateToPrint(webDriver, challan);
        //        fileName = fileName + "_" + challan;
        //        //  string transactionNo= GeneratePDF(webDriver, downloadPath, fileName);
        //        GeneratePDF(webDriver, downloadPath, fileName + "bank_copy", "Bank's copy");
        //      string transctionNo=  GeneratePDF(webDriver, downloadPath, fileName + "remitters_copy", "Remitter's copy");
        //        webDriver.Quit();
        //        return transctionNo;


        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private static void NavigateToPrint(IWebDriver webDriver, string challanNo) {
            webDriver.Navigate().GoToUrl("https://k2.karnataka.gov.in/wps/portal/Khajane-II/Scope/Remittance/SearchChallan/!ut/p/z1/04_Sj9CPykssy0xPLMnMz0vMAfIjo8ziTSycnQ39nQ38LVx8LA0C_f3DQn28PAwNjI31w8EKDHAARwP9KGL041EQhd_4cP0ovFa4GBJQYGFEQIGBAVQBHlcU5IZGGGR6pgMA6Ql2sA!!/dz/d5/L2dBISEvZ0FBIS9nQSEh/");
            WaitFor(webDriver, 3);
            var addressElm = webDriver.FindElement(By.Id("viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:searchForm:refNo"));
            addressElm.SendKeys(challanNo);
            MessageBox.Show("Fill Captcha then press OK ", "Confirmation", MessageBoxButton.OK);

            var searchBtn = webDriver.FindElement(By.Id("viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:searchForm:Search"));
            searchBtn.Click();
            WaitFor(webDriver, 2);
            var searcchallanLinkBtn = webDriver.FindElement(By.Id("viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:searchForm:tableEx1:0:link1"));
            searcchallanLinkBtn.Click();
            WaitFor(webDriver, 3);
            var printLinkBtn = webDriver.FindElement(By.Id("viewns_Z7_I2K611S0OGNNC0QA0KEELJ20G3_:searchForm:j_id_6f"));
            printLinkBtn.Click();
            WaitFor(webDriver, 3);
           
        }

        private static string FillArticle5E(IWebDriver webDriver, ArticleFeedModel model) {

            try
            {
                webDriver.Navigate().GoToUrl("https://k2.karnataka.gov.in/wps/portal/Khajane-II/Scope/Remittance/ChallanGeneration/");
                WaitFor(webDriver, 3);

                var curHandle = webDriver.CurrentWindowHandle;
                var tot = webDriver.WindowHandles.Count;

                var tota= webDriver.WindowHandles.Count;
                WaitFor(webDriver, 2);
               
                var firstNameElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:firstName"));
                firstNameElm.SendKeys(model.FirstName.Trim());
                WaitFor(webDriver, 1);

                if (!string.IsNullOrEmpty(model.MiddleName))
                {
                    var middleNameElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:middleName"));
                    middleNameElm.SendKeys(model.MiddleName.Trim());
                    WaitFor(webDriver, 1);
                }
                var lastNameElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:lastName"));
                lastNameElm.SendKeys(model.LastName.Trim());

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
                ////*[@id="ui-id-436"]
                var departmentPopupElm = webDriver.FindElement(By.XPath("//div[contains(.,'DEPARTMENT OF STAMPS AND REGISTRATION')]"));
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
                try
                {
                    WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
                      var alert = wait.Until(ExpectedConditions.AlertIsPresent());
                    if (alert != null)
                    {
                        alert.Accept();
                        WaitFor(webDriver, 1);
                        // Note : first name cleared automatically so fill again
                        firstNameElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:firstName"));
                        firstNameElm.Clear();
                        firstNameElm.SendKeys(model.FirstName.Trim());
                        WaitFor(webDriver, 1);

                        amountElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:amount2"));
                        amountElm.Clear();
                        amountElm.SendKeys(model.Amount);
                        WaitFor(webDriver, 1);

                        submitElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanForm:submit_id"));
                        submitElm.Click();
                        WaitFor(webDriver, 4);
                    }
                }
                catch (Exception ex) { }

                //var captcha = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:printForm:captchaText")).Text;

                var captcha = ReadCaptcha(webDriver);
                if (captcha == "")
                {
                    MessageBoxResult result = MessageBox.Show("Please fill the captcha and press OK button.", "Confirmation",
                                                     MessageBoxButton.OK, MessageBoxImage.Asterisk,
                                                     MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {

                    var captchaInputElm = webDriver.FindElement(By.Id("txt_Captcha"));
                    captchaInputElm.SendKeys(captcha);
                    WaitFor(webDriver, 2);
                }            

                //Click Confirm button
                var confirmElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:printForm:j_id_33"));
                confirmElm.Click();
                WaitFor(webDriver, 2);

                //
                var challanNoTxt = webDriver.FindElement(By.XPath("//*[@id='viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanConfirm']/fieldset/table/tbody/tr[2]/td/p/b")).Text;
                var challanno = challanNoTxt.Replace("\r\n", " ").Split(':')[1].Trim().Split(' ')[0];
                //Click Confirm button
                var okElm = webDriver.FindElement(By.Id("viewns_Z7_48CC1OC0O0VID0QCG60F962085_:challanConfirm:j_id_13"));
                okElm.Click();
                WaitFor(webDriver, 1);

                return challanno;
            }
            catch (Exception ex) {
                throw new Exception("Challan filling  Failed");
            }
        }

        private static void ProcessToBank(IWebDriver webDriver,string remark="")
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

                //Applr remarks
                WaitFor(webDriver, 1);
                if (!string.IsNullOrEmpty(remark))
                {
                    var remarkTxt = webDriver.FindElement(By.Id("TranRequestManagerFG.PMT_RMKS"));
                    remarkTxt.SendKeys(remark);
                }
                //End remarks


                var gridAuth = webDriver.FindElements(By.Id("TranRequestManagerFG.AUTH_MODES"));
                gridAuth[1].Click();

                var continueBtn = webDriver.FindElements(By.Id("CONTINUE_PREVIEW"));
                if (continueBtn.Count > 0)
                {
                    continueBtn[0].Click();
                    WaitForReady(webDriver);
                }
                ProcessGridData(webDriver);
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

                //var jsExecuter = (IJavaScriptExecutor)webDriver;
                //jsExecuter.ExecuteScript(" var elm=document.getElementById('printChallan'); setTimeout(function(){elm.click();},100);");

                //WaitFor(webDriver, 1);
               // webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
               

                //var printBtn = webDriver.FindElement(By.XPath("//cr-button[@class='action-button']"));
                //printBtn.Click();

                
            }
            catch (Exception ex) {
                throw new Exception("bank process Failed.");
            }
        }
        private static void FillGridValue() { 
        
        }
        private static void FeedStaticData(ArticleFeedModel model,bool isArticle5e) {
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
        private static void ProcessGridData(IWebDriver webDriver)
        {
            Dictionary<string, string> grid = new Dictionary<string, string>();
            //582266194.RGANESH
            grid.Add("A", "57");
            grid.Add("B", "12");
            grid.Add("C", "22");
            grid.Add("D", "67");
            grid.Add("E", "33");
            grid.Add("F", "10");
            grid.Add("G", "49");
            grid.Add("H", "43");
            grid.Add("I", "20");
            grid.Add("J", "31");
            grid.Add("K", "89");
            grid.Add("L", "28");
            grid.Add("M", "42");
            grid.Add("N", "05");
            grid.Add("O", "88");
            grid.Add("P", "04");

            ////579091011.RGANESH
            //grid.Add("A", "20");
            //grid.Add("B", "43");
            //grid.Add("C", "12");
            //grid.Add("D", "32");
            //grid.Add("E", "64");
            //grid.Add("F", "76");
            //grid.Add("G", "04");
            //grid.Add("H", "42");
            //grid.Add("I", "47");
            //grid.Add("J", "67");
            //grid.Add("K", "71");
            //grid.Add("L", "03");
            //grid.Add("M", "41");
            //grid.Add("N", "71");
            //grid.Add("O", "93");
            //grid.Add("P", "11");

            ////579091011.VIJAYALAKSHMI
            //grid.Add("A", "38");
            //grid.Add("B", "94");
            //grid.Add("C", "84");
            //grid.Add("D", "08");
            //grid.Add("E", "51");
            //grid.Add("F", "47");
            //grid.Add("G", "23");
            //grid.Add("H", "81");
            //grid.Add("I", "21");
            //grid.Add("J", "81");
            //grid.Add("K", "16");
            //grid.Add("L", "91");
            //grid.Add("M", "63");
            //grid.Add("N", "13");
            //grid.Add("O", "11");
            //grid.Add("P", "55");

            ////Repro Sri
            //grid.Add("A", "90");
            //grid.Add("B", "82");
            //grid.Add("C", "45");
            //grid.Add("D", "71");
            //grid.Add("E", "42");
            //grid.Add("F", "57");
            //grid.Add("G", "54");
            //grid.Add("H", "01");
            //grid.Add("I", "83");
            //grid.Add("J", "10");
            //grid.Add("K", "60");
            //grid.Add("L", "82");
            //grid.Add("M", "21");
            //grid.Add("N", "92");
            //grid.Add("O", "53");
            //grid.Add("P", "34");

            ////RGAN31
            //grid.Add("A", "17");
            //grid.Add("B", "45");
            //grid.Add("C", "32");
            //grid.Add("D", "41");
            //grid.Add("E", "64");
            //grid.Add("F", "28");
            //grid.Add("G", "50");
            //grid.Add("H", "86");
            //grid.Add("I", "32");
            //grid.Add("J", "93");
            //grid.Add("K", "06");
            //grid.Add("L", "93");
            //grid.Add("M", "40");
            //grid.Add("N", "51");
            //grid.Add("O", "47");
            //grid.Add("P", "29");

            var gridElms = webDriver.FindElements(By.ClassName("gridauth_input_cell_style"));
            var firstLetter = gridElms[0].Text;
            var secondLetter = gridElms[1].Text;
            var thirdLetter = gridElms[2].Text;

            var firstInput = webDriver.FindElement(By.Id("TranRequestManagerFG.GRID_CARD_AUTH_VALUE_1__"));
            firstInput.SendKeys(grid[firstLetter]);
            var secondInput = webDriver.FindElement(By.Id("TranRequestManagerFG.GRID_CARD_AUTH_VALUE_2__"));
            secondInput.SendKeys(grid[secondLetter]);
            var thirstInput = webDriver.FindElement(By.Id("TranRequestManagerFG.GRID_CARD_AUTH_VALUE_3__"));
            thirstInput.SendKeys(grid[thirdLetter]);

        }

        private static string ReadCaptcha(IWebDriver webDriver) {

            var jsExecuter = (IJavaScriptExecutor)webDriver;
            var base64string = jsExecuter.ExecuteScript(@"
    var c = document.createElement('canvas');
    var ctx = c.getContext('2d');
    var img = document.getElementById('viewns_Z7_48CC1OC0O0VID0QCG60F962085_:printForm:captcha');
    c.height=img.naturalHeight;
    c.width=img.naturalWidth;
    ctx.drawImage(img, 0, 0,img.naturalWidth, img.naturalHeight);
    var base64String = c.toDataURL();
    return base64String;
    ") as string;


            var base64 = base64string.Split(',').Last();
            // var ClientKey = "db027000f08afb6176183e15a137b13a";  //test
            var ClientKey = "f35d396e27db69a278ead2739cb85e99";
            var captcha = "";
            var api = new ImageToText
            {
                ClientKey = ClientKey,
                BodyBase64 = base64
            };

            if (!api.CreateTask())
            {
                MessageBox.Show(api.ErrorMessage, "Error");
            }
            else if (!api.WaitForResult())
            {
                MessageBox.Show("Could not solve the captcha.", "Error");
                //  DebugHelper.Out("Could not solve the captcha.", DebugHelper.Type.Error);
            }
            else
            {
                captcha = api.GetTaskSolution().Text;
                // DebugHelper.Out("Result: " + api.GetTaskSolution().Text, DebugHelper.Type.Success);
            }
            return captcha;
        }
    }

    public class BankAccountDetailsDto
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}
