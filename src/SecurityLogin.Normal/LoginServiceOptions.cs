using System.Text;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public class LoginServiceOptions
    {
        public JsonSerializerOptions JsonOptions { get; set; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
}
