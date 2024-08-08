using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace SecurityLogin.AspNetCore
{
    public class AppLoginOptions
    {
        public string AppKeyHeader { get; set; } = "app-key";

        public int AppKeyEmptyStatusCode { get; set; } = StatusCodes.Status403Forbidden;

        public string? AppKeyEmptyResponseMsg { get; set; } = "App key is empty";

        public int AppSnatshopEmptyStatusCode { get; set; } = StatusCodes.Status403Forbidden;

        public string? AppSnatshopEmptyResponseMsg { get; set; } = "App session not found";

        public List<PathString> NotNeedToCheck { get; } = new List<PathString>();

        internal IAppLoginProvider CreateProvider()
        {
            if (string.IsNullOrEmpty(AppKeyHeader))
            {
                throw new ArgumentNullException("AppKeyHeader");
            }
            return new DefaultAppLoginProvider((AppLoginOptions)MemberwiseClone());
        }
    }
}
