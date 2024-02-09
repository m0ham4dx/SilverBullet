using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Extreme.Net
{
	public class AdvancedWebClient : WebClient
	{
		public int Timeout { get; set; } = 10000;

		public int ReadWriteTimeout { get; set; } = 10000;

		public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;

		public bool ServerCertificateValidation { get; set; }

		protected override WebRequest GetWebRequest(Uri uri)
		{
			WebRequest webRequest = base.GetWebRequest(uri);
			if (webRequest == null)
			{
				throw new NullReferenceException("Null reference: unable to get instance of WebRequest in AdvancedWebClient.");
			}
			webRequest.Timeout = this.Timeout;
			HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
			httpWebRequest.ReadWriteTimeout = this.ReadWriteTimeout;
			httpWebRequest.AutomaticDecompression = this.DecompressionMethods;
			if (!this.ServerCertificateValidation)
			{
				HttpWebRequest httpWebRequest2 = httpWebRequest;
				httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(AdvancedWebClient.ServerCertificateValidationCallback));
			}
			return webRequest;
		}

		private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			return true;
		}
	}
}
