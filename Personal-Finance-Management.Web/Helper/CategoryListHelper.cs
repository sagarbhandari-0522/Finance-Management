using Microsoft.AspNetCore.Mvc.Rendering;
using Personal_Finance_Management.Domain.Entities;

namespace Personal_Finance_Management.Web.Helper
{
    public static class CategoryListHelper
    {
        public static List<SelectListItem> CategoryList(List<Category> categoryList)
        {
            var categorySelectList = categoryList.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.Name
            }).ToList();
            return categorySelectList;
        }

    }
}
