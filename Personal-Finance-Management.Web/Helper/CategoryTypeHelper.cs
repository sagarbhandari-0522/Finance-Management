using Microsoft.AspNetCore.Mvc.Rendering;
using Personal_Finance_Management.Domain.Enums;

namespace Personal_Finance_Management.Web.Helper
{
    public static class CategoryTypeHelper
    {
        public static List<SelectListItem> GetCategory()
        {
            var typeList = Enum.GetNames(typeof(CategoryType)).ToList();
            var type = typeList.Select(v => new SelectListItem
            {
                Value = v.ToString(),
                Text = v.ToString()
            }).ToList();
            return type;
        }
    }
}
