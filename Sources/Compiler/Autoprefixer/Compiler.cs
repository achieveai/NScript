namespace Autoprefixer
{
    using System;
    using System.Collections.Generic;
    using JavaScriptEngineSwitcher.Core;
    using Microsoft.ClearScript;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public sealed class Compiler : IDisposable
	{
		/// <summary>
		/// Name of resource, which contains the Autoprefixer library
		/// </summary>
		const string LibraryResourceName = "Autoprefixer.Resources.autoprefixer.js";

		/// <summary>
		/// Template of function call, which is responsible for autoprefixing
		/// </summary>
		const string FunctionCallTemplate = @"autoprefixer.apply(this, {1}).process({0}).css;";

		private readonly ScriptEngine _javascriptEngine;

		private readonly object _synchronizer = new object();

		private bool _initialized;

		private bool _disposed;

    	/// <summary>
		/// Constructs instance of Autoprefixer
		/// </summary>
		/// <param name="javascriptEngineFactory">Delegate that creates an instance of JavaScript engine</param>
		public Compiler(Func<Microsoft.ClearScript.ScriptEngine> javascriptEngineFactory)
		{
			_javascriptEngine = javascriptEngineFactory();
		}

		/// <summary>
		/// Initializes Autoprefixer
		/// </summary>
		private void Initialize()
		{
		    if (_initialized) return;
		    var type = GetType();

			string script = null;
			using (var stream = type.Assembly.GetManifestResourceStream(LibraryResourceName))
            using (var streamReader = new System.IO.StreamReader(stream))
            {
				script = streamReader.ReadToEnd();
            }

            _javascriptEngine.Execute(script);

		    _initialized = true;
		}

	    public string Prefix(string content, ICollection<string> browsers, Options options = null)
	    {
            if (options == null)
            {
                options = new Options();
            }

            string currentBrowsersString;
	        if (browsers.Count == 0)
	        {
	            currentBrowsersString = "undefined";
	        }
	        else
	        {
                // ReSharper disable once UseObjectOrCollectionInitializer
	            var arguments = new JArray(browsers);
                arguments.Add(JObject.FromObject(options));
	            currentBrowsersString = arguments.ToString();
	        }

            lock (_synchronizer)
            {
                Initialize();

                try
                {
                    return _javascriptEngine.Evaluate(
                        string.Format(FunctionCallTemplate,
                            JsonConvert.SerializeObject(content),
                            currentBrowsersString)) as string;
                }
                catch (JsRuntimeException e)
                {
                    throw new AutoprefixerException(e.Message);
                }
            }
	    }

        /// <summary>
        /// Vendor-prefix CSS code automatically
        /// </summary>
        /// <param name="content">Text content of CSS-asset</param>
        /// <param name="browserSpecification">The browsers to target</param>
        /// <returns>Vendor-prefixed CSS</returns>
        public string Prefix(string content, BrowserSpecification browserSpecification = null, Options options = null)
		{
            if (browserSpecification == null)
            {
                browserSpecification = new BrowserSpecification();
            }
		    var browsers = browserSpecification.GetBrowserSpecificationStrings();
            return Prefix(content, browsers, options);
		}

	    /// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
	        if (_disposed) return;
	        _disposed = true;

	        if (_javascriptEngine != null)
	        {
	            _javascriptEngine.Dispose();
	        }
		}
	}
}