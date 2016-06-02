using Microsoft.AspNetCore.Builder;

namespace AspNetCoreFileManager.ThumbnailProcessor
{
    public static class ThumbnailExtensions
    {
        public static IApplicationBuilder UseThumbnailProcessor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ThumbnailProcessorMiddleware>();
        }
    }
}