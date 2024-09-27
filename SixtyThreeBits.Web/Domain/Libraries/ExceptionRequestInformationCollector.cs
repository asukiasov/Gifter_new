using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Domain.Libraries
{
	public class ExceptionRequestInformationCollector
	{
		#region Properties
		readonly HttpRequest _request;
		readonly Exception _ex;
		readonly StringBuilder _logStringBuilder = new();
		#endregion

		#region Constructors
		ExceptionRequestInformationCollector(HttpRequest request, Exception exception)
		{
			_request = request;
			_ex = exception;
		}
		#endregion

		#region Methods
		public static ExceptionRequestInformationCollector Create(HttpRequest request, Exception exception)
		{
			return new ExceptionRequestInformationCollector(request, exception);
		}

		public async Task<string> Collect()
		{
			collectUrlData();
			collectErrorMessage();
			collectFormData();
			await collectBodyData();
			collectStackTrace();

			return _logStringBuilder.ToString();
		}

		void collectUrlData()
		{
			var requestUrl = $"{_request.Scheme}://{_request.Host}{_request.Path}{_request.QueryString}";
			_logStringBuilder.Append("Url: ").Append(requestUrl).AppendLine().AppendLine();
		}
		void collectErrorMessage()
		{
			_logStringBuilder.Append("Exception: ").Append(_ex.Message).AppendLine();
			if (_ex.InnerException != null)
			{
				_logStringBuilder.Append("InnerException: ").Append(_ex.InnerException.Message).AppendLine().AppendLine();
			}
			else
			{
				_logStringBuilder.AppendLine();
			}

		}
		void collectFormData()
		{
			if (_request.HasFormContentType)
			{
				var hasFormValues = _request.Form.Keys.Count > 0;
				if (hasFormValues)
				{
					_logStringBuilder.AppendLine("Form:");
					foreach (var key in _request.Form.Keys)
					{
						_logStringBuilder.AppendLine($"{key}={_request.Form[key]}");
					}
					_logStringBuilder.AppendLine();
				}
			}
		}
		async Task collectBodyData()
		{
			using (var reader = new StreamReader(_request.Body, Encoding.UTF8, true, 1024, true))
			{
				var body = await reader.ReadToEndAsync();
				if (!string.IsNullOrWhiteSpace(body))
				{
					_logStringBuilder.AppendLine("Body:");
					_logStringBuilder.AppendLine(body);
					_logStringBuilder.AppendLine();
				}
			}
		}
		void collectStackTrace()
		{
			_logStringBuilder.Append("StackTrace:").Append(Environment.NewLine).Append(_ex.StackTrace);
		}
		#endregion
	}
}
