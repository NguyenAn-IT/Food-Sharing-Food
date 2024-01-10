using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace Food_Sharing_Food.TESTCASE
{
    [TestFixture]
    public class LoginTest
    {
        private IWebDriver driver;
        private string baseUrl = "https://localhost:44368/";

        [SetUp]
        public void Setup()
        {
            // Khởi tạo trình duyệt (Ví dụ: Chrome)
            driver = new EdgeDriver("D:\\CHUYÊN NGÀNH");
        }

        [Test]
        public void TestSuccessfulLogin()
        {
            driver.Navigate().GoToUrl(baseUrl + "/Account/Login");

            // Nhập thông tin đăng nhập hợp lệ
            IWebElement emailInput = driver.FindElement(By.Id("Email"));
            IWebElement passwordInput = driver.FindElement(By.Id("Password"));
            IWebElement submitButton = driver.FindElement(By.CssSelector(".btn-remember input[type='submit']"));
            emailInput.SendKeys("admin@gmail.com");
            passwordInput.SendKeys("admin@123");
            submitButton.Click();

            // Wait for the page to load (you can customize the wait based on your application)
            System.Threading.Thread.Sleep(2000);
            Assert.That(driver.Url, Is.EqualTo(baseUrl + "/Home/Index"));

        }

        [Test]
        public void TestInvalidLogin()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Login");

            IWebElement emailInput = driver.FindElement(By.Id("Email"));
            IWebElement passwordInput = driver.FindElement(By.Id("Password"));
            IWebElement submitButton = driver.FindElement(By.CssSelector(".btn-remember input[type='submit']"));

            emailInput.SendKeys("An321@gmail.com");
            passwordInput.SendKeys("An321");
            submitButton.Click();

            System.Threading.Thread.Sleep(2000);

            // Assert

            IWebElement errorElement = driver.FindElement(By.CssSelector(".text-danger"));
            Assert.That(errorElement.Displayed);


        }

        [TearDown]
        public void Teardown()
        {
            // Đóng trình duyệt sau khi hoàn thành mỗi test case
            driver.Quit();
        }
    }
}