using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppClientTests.Utilities
{
    public class TempDataDictionaryFactory : ITempDataDictionaryFactory
    {
        public readonly ITempDataProvider _tempDataProvider;
        public TempDataDictionaryFactory(ITempDataProvider tempDataProvider)
        {
            _tempDataProvider = tempDataProvider;
        }
        public ITempDataDictionary CreateTempData(HttpContext context)
        {
            if (_tempDataProvider == null)
            {
                throw new InvalidOperationException($"No {nameof(TempDataDictionary)} was set.");
            }
            return new TempDataDictionary(context, _tempDataProvider);
        }
        public ITempDataDictionary GetTempData(HttpContext context)
        {
            return CreateTempData(context);
        }
    }
}
