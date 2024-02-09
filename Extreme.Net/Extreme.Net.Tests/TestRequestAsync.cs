using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Extreme.Net;

namespace Extreme.Net.Tests
{
    [TestClass]
    public class TestRequestAsync
    {
		private readonly string url = "http://httpbin.org";
        private readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
        ///<summary>
        /// Проверяет валидность асинхронной отправки Get аргументов и значений 
        ///</summary>
        [TestMethod]
        public async void TestMethodGetAsync()
        {
			string getArgument = "getArgument";
			string getValue = "getValue";
			
			var request = new HttpRequest();
			var uri = $"{url}/get?{getArgument}={getValue}";
			
			var response = await request.GetAsync(uri);
			var source = response.ToString();
			
			StringAssert.Contains(source, getArgument);
			StringAssert.Contains(source, getValue);
        }
		
		///<summary>
		/// Проверяет валидность асинхрнной отправки Post аргументов и значений 
		///</summary>
		[TestMethod]
		public async void TestMethodPostAsync()
		{
			string postArgument = "postArgument";
			string postValue = "postValue";
			
			var request = new HttpRequest();
			var uri  = $"{url}/post";
			var data = $"{postArgument}={postValue}";
			
			var response = await request.PostAsync(uri, data);
			var source = response.ToString();
			
			StringAssert.Contains(source, postArgument);
			StringAssert.Contains(source, postValue);
		}
		
		///<summary>
		/// Проверяет валидность асинхронно отправляемых заголовков 
		///</summary>
		[TestMethod]
		public async void TestHeadersAsync()
		{
			
			
			var request = new HttpRequest();
			request.UserAgent = userAgent;
			
			var uri = $"{url}/user-agent";
			var response = await request.GetAsync(uri);
			
			var source = response.ToString();
			
			StringAssert.Contains(source, userAgent);
		}
		
		///<summary>
		/// Проверяет валидность асинхронно отправляемых Cookies 
		///</summary>
		[TestMethod]
		public async void TestCoockieAsync()
		{
			var cookies = new CookieDictionary();
			
			var key   = "qwe";
			var value = "rty";
			
			cookies.Add(key, value);
			
			var request = new HttpRequest();
			request.Cookies   = cookies;
			request.UserAgent = userAgent;
			
			var uri = $"{url}/cookies";
			var response = await request.GetAsync(uri);
			
			var source = response.ToString();
			
			StringAssert.Contains(source, key);
			StringAssert.Contains(source, value);
		}
    }
}
