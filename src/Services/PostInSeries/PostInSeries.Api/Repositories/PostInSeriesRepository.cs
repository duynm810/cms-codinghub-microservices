using System.Text;
using Contracts.Commons.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using PostInSeries.Api.Repositories.Interfaces;

namespace PostInSeries.Api.Repositories;

public class PostInSeriesRepository(
    IDistributedCache redisCacheService,
    ISerializeService serializeService) : IPostInSeriesRepository
{
    public async Task CreatePostToSeries(Guid seriesId, Guid postId, int sortOrder)
    {
        var seriesKey = $"series:{seriesId}:posts";
        var postsBytes = await redisCacheService.GetAsync(seriesKey);
        List<Guid>? postIds;

        if (postsBytes != null)
        {
            // Convert byte[] to string before deserialize
            // Chuyển đổi byte[] thành string trước khi deserialize
            var postsString = Encoding.UTF8.GetString(postsBytes);

            // Check postsString is not null or empty
            // Kiểm tra postsString không phải là null hoặc rỗng
            postIds = !string.IsNullOrEmpty(postsString)
                ? serializeService.Deserialize<List<Guid>>(postsString)
                : new List<Guid>();

            if (postIds != null && !postIds.Contains(postId))
            {
                postIds.Add(postId);
            }
        }
        else
        {
            postIds = new List<Guid> { postId };
        }

        // Serialize and convert the result to byte[] before saving to Redis
        // Serialize và chuyển đổi kết quả thành byte[] trước khi lưu vào Redis
        var updatedPostsString = serializeService.Serialize(postIds);
        var updatedPostsBytes = Encoding.UTF8.GetBytes(updatedPostsString);

        await redisCacheService.SetAsync(seriesKey, updatedPostsBytes);
    }

    public async Task DeletePostToSeries(Guid seriesId, Guid postId)
    {
        var seriesKey = $"series:{seriesId}:posts";
        var postsBytes = await redisCacheService.GetAsync(seriesKey);

        if (postsBytes != null)
        {
            var postsString = Encoding.UTF8.GetString(postsBytes);
            var postIds = serializeService.Deserialize<List<Guid>>(postsString);

            if (postIds != null && postIds.Contains(postId))
            {
                postIds.Remove(postId);
                if (postIds.Count > 0)
                {
                    // Update the list again after deleting the postId
                    // Cập nhật lại danh sách sau khi xóa postId
                    var updatedPostsString = serializeService.Serialize(postIds);
                    var updatedPostsBytes = Encoding.UTF8.GetBytes(updatedPostsString);
                    await redisCacheService.SetAsync(seriesKey, updatedPostsBytes);
                }
                else
                {
                    // If the list is empty, delete the key
                    // Nếu danh sách rỗng, xóa key
                    await redisCacheService.RemoveAsync(seriesKey);
                }
            }
        }
    }
    
    public async Task<IEnumerable<Guid>?> GetPostIdsInSeries(Guid seriesId)
    {
        var seriesKey = $"series:{seriesId}:posts";
        var postsBytes = await redisCacheService.GetAsync(seriesKey);
        if (postsBytes == null)
        {
            return null;
        }

        var postsString = Encoding.UTF8.GetString(postsBytes);
        return string.IsNullOrEmpty(postsString) ? null : serializeService.Deserialize<List<Guid>>(postsString);
    }
}