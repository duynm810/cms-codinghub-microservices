using AutoMapper;
 using Category.Api.Entities;
 using Category.Api.GrpcClients.Interfaces;
 using Category.Api.Repositories.Interfaces;
 using Category.Api.Services.Interfaces;
 using Contracts.Commons.Interfaces;
 using Shared.Constants;
 using Shared.Dtos.Category;
 using Shared.Helpers;
 using Shared.Responses;
 using Shared.Utilities;
 using ILogger = Serilog.ILogger;
 
 namespace Category.Api.Services;
 
 public class CategoryService(
     ICategoryRepository categoryRepository,
     IPostGrpcClient postGrpcClient,
     ICacheService cacheService,
     IMapper mapper,
     ILogger logger) : ICategoryService
 {
     #region CRUD
 
     public async Task<ApiResult<CategoryDto>> CreateCategory(CreateCategoryDto request)
     {
         var result = new ApiResult<CategoryDto>();
         const string methodName = nameof(CreateCategory);
 
         try
         {
             logger.Information("BEGIN {MethodName} - Creating category with name: {CategoryName}", methodName,
                 request.Name);
 
             if (string.IsNullOrEmpty(request.Slug))
             {
                 request.Slug = Utils.ToUnSignString(request.Name);
             }
 
             var category = mapper.Map<CategoryBase>(request);
             await categoryRepository.CreateCategory(category);
 
             var data = mapper.Map<CategoryDto>(category);
             result.Success(data);
 
             // Clear category list cache when a new category is created (Xóa cache danh sách category khi tạo mới)
             await Task.WhenAll(
                 cacheService.RemoveAsync(CacheKeyHelper.Category.GetAllCategoriesKey()),
                 cacheService.RemoveAsync(CacheKeyHelper.CategoryGrpc.GetGrpcAllNonStaticPageCategoriesKey())
             );
 
             logger.Information("END {MethodName} - Category created successfully with ID {CategoryId}", methodName,
                 data.Id);
         }
         catch (Exception e)
         {
             logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
             result.Messages.AddRange(e.GetExceptionList());
             result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
         }
 
         return result;
     }
 
     public async Task<ApiResult<CategoryDto>> UpdateCategory(long id, UpdateCategoryDto request)
     {
         var result = new ApiResult<CategoryDto>();
         const string methodName = nameof(UpdateCategory);
 
         try
         {
             logger.Information("BEGIN {MethodName} - Updating category with ID: {CategoryId}", methodName, id);
 
             var category = await categoryRepository.GetCategoryById(id);
             if (category == null)
             {
                 result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                 result.Failure(StatusCodes.Status404NotFound, result.Messages);
                 return result;
             }
 
             var updateCategory = mapper.Map(request, category);
             await categoryRepository.UpdateCategory(updateCategory);
 
             var data = mapper.Map<CategoryDto>(updateCategory);
             result.Success(data);
 
             // Delete category caches when updating (Xóa cache danh sách category khi cập nhật)
             await Task.WhenAll(
                 cacheService.RemoveAsync(CacheKeyHelper.Category.GetAllCategoriesKey()),
                 cacheService.RemoveAsync(CacheKeyHelper.Category.GetCategoryByIdKey(id)),
                 cacheService.RemoveAsync(CacheKeyHelper.Category.GetCategoryBySlugKey(category.Slug)),
                 cacheService.RemoveAsync(CacheKeyHelper.CategoryGrpc.GetGrpcAllNonStaticPageCategoriesKey()),
                 cacheService.RemoveAsync(CacheKeyHelper.CategoryGrpc.GetGrpcCategoryByIdKey(id)),
                 cacheService.RemoveAsync(CacheKeyHelper.CategoryGrpc.GetGrpcCategoryBySlugKey(category.Slug))
             );
 
             logger.Information("END {MethodName} - Category with ID {CategoryId} updated successfully", methodName, id);
         }
         catch (Exception e)
         {
             logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
             result.Messages.AddRange(e.GetExceptionList());
             result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
         }
 
         return result;
     }
 
     public async Task<ApiResult<bool>> DeleteCategory(List<long> ids)
     {
         var result = new ApiResult<bool>();
         const string methodName = nameof(DeleteCategory);
 
         try
         {
             logger.Information("BEGIN {MethodName} - Deleting categories with IDs: {CategoryIds}", methodName,
                 string.Join(", ", ids));
 
             var tasks = new List<Task>();
             
             foreach (var id in ids)
             {
                 var category = await categoryRepository.GetCategoryById(id);
                 if (category == null)
                 {
                     result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                     result.Failure(StatusCodes.Status404NotFound, result.Messages);
                     return result;
                 }
 
                 var existedPost = await postGrpcClient.HasPostsInCategory(id);
                 if (existedPost)
                 {
                     result.Messages.Add(ErrorMessagesConsts.Category.CategoryContainsPost);
                     result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                     return result;
                 }
 
                 tasks.Add(categoryRepository.DeleteCategory(category));
 
                 // Add cache removal tasks to the list (Thêm tác vụ xóa bộ nhớ cache vào danh sách)
                 tasks.Add(cacheService.RemoveAsync(CacheKeyHelper.Category.GetCategoryByIdKey(id)));
                 tasks.Add(cacheService.RemoveAsync(CacheKeyHelper.Category.GetCategoryBySlugKey(category.Slug)));
                 tasks.Add(cacheService.RemoveAsync(CacheKeyHelper.CategoryGrpc.GetGrpcCategoryByIdKey(id)));
                 tasks.Add(cacheService.RemoveAsync(CacheKeyHelper.CategoryGrpc.GetGrpcCategoryBySlugKey(category.Slug)));
             }
             
             // Execute all tasks in parallel (Thực hiện tất cả các nhiệm vụ song song)
             await Task.WhenAll(tasks);

             // Delete category list cache when deleting (Xóa cache danh sách category khi xóa dữ liệu)
             await Task.WhenAll(
                 cacheService.RemoveAsync(CacheKeyHelper.Category.GetAllCategoriesKey()),
                 cacheService.RemoveAsync(CacheKeyHelper.CategoryGrpc.GetGrpcAllNonStaticPageCategoriesKey())
             );
 
             result.Success(true);
 
             logger.Information("END {MethodName} - Categories with IDs {CategoryIds} deleted successfully",
                 methodName, string.Join(", ", ids));
         }
         catch (Exception e)
         {
             logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
             result.Messages.AddRange(e.GetExceptionList());
             result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
         }
 
         return result;
     }
 
     public async Task<ApiResult<IEnumerable<CategoryDto>>> GetCategories()
     {
         var result = new ApiResult<IEnumerable<CategoryDto>>();
         const string methodName = nameof(GetCategories);
 
         try
         {
             logger.Information("BEGIN {MethodName} - Retrieving all categories", methodName);
 
             var cacheKey = CacheKeyHelper.Category.GetAllCategoriesKey();
             var cachedCategories = await cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey);
             if (cachedCategories != null)
             {
                 result.Success(cachedCategories);
                 logger.Information("END {MethodName} - Successfully retrieved categories from cache", methodName);
                 return result;
             }
 
             var categories = await categoryRepository.GetCategories();
             if (categories.IsNotNullOrEmpty())
             {
                 var data = mapper.Map<List<CategoryDto>>(categories);
                 result.Success(data);
 
                 // Save cache (Lưu cache)
                 await cacheService.SetAsync(cacheKey, data);
 
                 logger.Information("END {MethodName} - Successfully retrieved {CategoryCount} categories", methodName,
                     data.Count);
             }
         }
         catch (Exception e)
         {
             logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
             result.Messages.AddRange(e.GetExceptionList());
             result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
         }
 
         return result;
     }
 
     public async Task<ApiResult<CategoryDto>> GetCategoryById(long id)
     {
         var result = new ApiResult<CategoryDto>();
         const string methodName = nameof(GetCategoryById);
 
         try
         {
             logger.Information("BEGIN {MethodName} - Retrieving category with ID: {CategoryId}", methodName, id);
 
             var cacheKey = CacheKeyHelper.Category.GetCategoryByIdKey(id);
             var cachedCategory = await cacheService.GetAsync<CategoryDto>(cacheKey);
             if (cachedCategory != null)
             {
                 result.Success(cachedCategory);
                 logger.Information("END {MethodName} - Successfully retrieved category with ID {CategoryId} from cache",
                     methodName, id);
                 return result;
             }
 
             var category = await categoryRepository.GetCategoryById(id);
             if (category == null)
             {
                 result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                 result.Failure(StatusCodes.Status404NotFound, result.Messages);
                 return result;
             }
 
             var data = mapper.Map<CategoryDto>(category);
             result.Success(data);
 
             // Save cache (Lưu cache)
             await cacheService.SetAsync(cacheKey, data);
 
             logger.Information("END {MethodName} - Successfully retrieved category with ID {CategoryId}", methodName,
                 id);
         }
         catch (Exception e)
         {
             logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
             result.Messages.AddRange(e.GetExceptionList());
             result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
         }
 
         return result;
     }
 
     #endregion
 
     #region OTHERS
 
     public async Task<ApiResult<PagedResponse<CategoryDto>>> GetCategoriesPaging(int pageNumber, int pageSize)
     {
         var result = new ApiResult<PagedResponse<CategoryDto>>();
         const string methodName = nameof(GetCategoriesPaging);
 
         try
         {
             logger.Information(
                 "BEGIN {MethodName} - Retrieving categories for page {PageNumber} with page size {PageSize}",
                 methodName, pageNumber, pageSize);
 
             var cacheKey = CacheKeyHelper.Category.GetCategoriesPagingKey(pageNumber, pageSize);
             var cachedCategories = await cacheService.GetAsync<PagedResponse<CategoryDto>>(cacheKey);
             if (cachedCategories != null)
             {
                 result.Success(cachedCategories);
                 logger.Information(
                     "END {MethodName} - Successfully retrieved categories for page {PageNumber} from cache", methodName,
                     pageNumber);
                 return result;
             }
 
             var categories = await categoryRepository.GetCategoriesPaging(pageNumber, pageSize);
             var data = new PagedResponse<CategoryDto>()
             {
                 Items = mapper.Map<List<CategoryDto>>(categories.Items),
                 MetaData = categories.MetaData
             };
 
             result.Success(data);
 
             // Save cache (Lưu cache)
             await cacheService.SetAsync(cacheKey, data);
 
             logger.Information(
                 "END {MethodName} - Successfully retrieved {CategoryCount} categories for page {PageNumber} with page size {PageSize}",
                 methodName, data.Items.Count, pageNumber, pageSize);
         }
         catch (Exception e)
         {
             logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
             result.Messages.AddRange(e.GetExceptionList());
             result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
         }
 
         return result;
     }
 
     public async Task<ApiResult<CategoryDto>> GetCategoryBySlug(string slug)
     {
         var result = new ApiResult<CategoryDto>();
         const string methodName = nameof(GetCategoryBySlug);
 
         try
         {
             logger.Information("BEGIN {MethodName} - Retrieving category with slug: {CategorySlug}", methodName, slug);
 
             var cacheKey = CacheKeyHelper.Category.GetCategoryBySlugKey(slug);
             var cachedCategory = await cacheService.GetAsync<CategoryDto>(cacheKey);
             if (cachedCategory != null)
             {
                 result.Success(cachedCategory);
                 logger.Information(
                     "END {MethodName} - Successfully retrieved category with slug {CategorySlug} from cache",
                     methodName, slug);
                 return result;
             }
 
             var category = await categoryRepository.GetCategoryBySlug(slug);
             if (category == null)
             {
                 result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                 result.Failure(StatusCodes.Status404NotFound, result.Messages);
                 return result;
             }
 
             var data = mapper.Map<CategoryDto>(category);
             result.Success(data);
 
             // Save cache (Lưu cache)
             await cacheService.SetAsync(cacheKey, data);
 
             logger.Information("END {MethodName} - Successfully retrieved category with slug {CategorySlug}",
                 methodName,
                 slug);
         }
         catch (Exception e)
         {
             logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
             result.Messages.AddRange(e.GetExceptionList());
             result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
         }
 
         return result;
     }
 
     #endregion
 }