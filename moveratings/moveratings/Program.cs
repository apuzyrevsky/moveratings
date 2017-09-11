using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace moveratings
{
    class Program
    {
        static void Main(string[] args)
        {
            string moviesPath = @"..\..\movies.txt";
            string ratingsPath = @"..\..\ratings.txt";
            List<string> movies = CreateListFromTxt(moviesPath);
            List<string> ratings = CreateListFromTxt(ratingsPath);

            Console.WriteLine("Please enter an email for login:");
            string email = Console.ReadLine();
            Console.WriteLine("Please enter a password for your account:");
            string password = Console.ReadLine();

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            IWebDriver driver = new ChromeDriver(options)
            {
                Url = "http://imdb.com"
            };
            

            LoginToIMDB(driver, email, password);
            AddRating(driver, movies, ratings);

            




        }

        static void AddRating(IWebDriver driver, List<string> movies, List<string> ratings)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                int rateNumber = Convert.ToInt32(ratings[i]);
                string movie = movies[i];
                var searchBox = driver.FindElement(By.Id("navbar-query"));
                searchBox.SendKeys(movie);
                driver.FindElement(By.Id("navbar-submit-button")).Click();
                bool isMovieFound = IsMovieFound(driver, movie);
                if (isMovieFound)
                {
                    AddNotFoundMovie(movie, rateNumber);
                    searchBox = driver.FindElement(By.Id("navbar-query"));
                    searchBox.Clear();
                    AddNotFoundMovie(movie, rateNumber);
                    continue;
                }
                else
                {
                    driver.FindElement(By.LinkText(movie)).Click();
                    var rate = driver.FindElement(By.ClassName("star-rating-button"));
                    rate.Click();
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    var div = rate.FindElement(By.XPath("div[contains(@class, 'star-rating')]"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    var stars = div.FindElements(By.TagName("a"));
                    stars[rateNumber].Click();
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                }
            }
        }
        
        static bool IsMovieFound(IWebDriver driver, string movie)
        {
            try
            {
                var existingMovie = driver.FindElement(By.LinkText(movie));
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Failed to find {0} movie", movie);
                return true;
            }
            return false;
        }
     
        static void AddNotFoundMovie(string movie, int rateNumber)
        {
            StringBuilder csv = new StringBuilder();
            string notfound = @"..\..\notfound.csv";
            var title = string.Format("{0},{1}", movie, rateNumber);
            csv.AppendLine(title);
            File.AppendAllText(notfound, csv.ToString());
        }


        static List<string> CreateListFromTxt(string filename)
        {
            List<string> list = new List<string>();
            StreamReader reader = new StreamReader(filename);
            using (reader)
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
            return list;
        }

       static void LoginToIMDB(IWebDriver driver, string email, string password)
        {
            driver.FindElement(By.Id("nblogin")).Click();
            driver.FindElement(By.LinkText("Sign in with IMDb")).Click();
            driver.FindElement(By.Id("ap_email")).Clear();
            driver.FindElement(By.Id("ap_email")).SendKeys(email);
            driver.FindElement(By.Id("ap_password")).Clear();
            driver.FindElement(By.Id("ap_password")).SendKeys(password);
            driver.FindElement(By.Id("signInSubmit")).Click();
            driver.FindElement(By.Id("navbar-query")).Click();
            driver.FindElement(By.Id("navbar-query")).Clear();
        }


    }
}
