using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using WebAPISecurity.Models;

namespace WebAPISecurity
{
    public class CategoryBudgetProfile: Profile
    {
        public CategoryBudgetProfile()
        {

            CreateMap<BudgetCategoryDto, BudgetCategory>();
            CreateMap<BudgetCategory, BudgetCategoryDto>()
            .ConvertUsing<IdProtectorConverter>();
        }
    }
}
