using BlossomServer.Application.ViewModels.Sorting;

namespace BlossomServer.Swagger
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class SortableFieldsAttribute<TSortingProvider, TViewModel, TEntity> : SwaggerSortableFieldsAttribute
        where TSortingProvider : ISortingExpressionProvider<TViewModel, TEntity>, new()
    {
        public override IEnumerable<string> GetFields()
        {
            return new TSortingProvider().GetSortingExpressions().Keys;
        }
    }
}
